using System.Reflection;

namespace Kook.Commands.Builders;

/// <summary>
///     表示一个参数构建器。
/// </summary>
public class ParameterBuilder
{
    #region ParameterBuilder

    private readonly List<ParameterPreconditionAttribute> _preconditions;
    private readonly List<Attribute> _attributes;

    /// <summary>
    ///     获取此参数构建器所属的命令构建器。
    /// </summary>
    public CommandBuilder Command { get; }

    /// <summary>
    ///     获取或设置此参数的名称。
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     获取或设置此参数的类型。
    /// </summary>
    public Type? ParameterType { get; internal set; }

    /// <summary>
    ///     获取或设置此参数的类型读取器。
    /// </summary>
    public TypeReader? TypeReader { get; set; }

    /// <summary>
    ///     获取或设置此参数是否为可选参数。
    /// </summary>
    public bool IsOptional { get; set; }

    /// <summary>
    ///     获取或设置此参数是否接收全部剩余参数。
    /// </summary>
    public bool IsRemainder { get; set; }

    /// <summary>
    ///     获取或设置此参数是否为多值参数。
    /// </summary>
    public bool IsMultiple { get; set; }

    /// <summary>
    ///     获取或设置此参数的默认值。
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    ///     获取或设置此参数的摘要。
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    ///     获取此参数的先决条件。
    /// </summary>
    public IReadOnlyList<ParameterPreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     获取此参数的特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    #endregion

    #region Automatic

    internal ParameterBuilder(CommandBuilder command)
    {
        _preconditions = [];
        _attributes = [];
        Name = string.Empty;
        Command = command;
    }

    #endregion

    #region User-defined

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParameterBuilder"/> class.
    /// </summary>
    /// <param name="command"> The command builder that this parameter builder belongs to. </param>
    /// <param name="name"> The name of this parameter. </param>
    /// <param name="type"> The type of this parameter. </param>
    internal ParameterBuilder(CommandBuilder command, string name, Type type)
        : this(command)
    {
        Kook.Preconditions.NotNull(name, nameof(name));
        Name = name;
        SetType(type);
    }

    /// <summary>
    ///     Sets the type of this parameter.
    /// </summary>
    /// <param name="type"> The type of this parameter. </param>
    internal void SetType(Type type)
    {
        TypeReader = GetReader(type);
        if (type.GetTypeInfo().IsValueType)
            DefaultValue = Activator.CreateInstance(type);
        else if (type.IsArray && type.GetElementType() is { } elementType)
            DefaultValue = Array.CreateInstance(elementType, 0);
        ParameterType = type;
    }

    /// <summary>
    ///     Gets the type reader of this parameter.
    /// </summary>
    /// <param name="type"> The type of this parameter. </param>
    /// <returns> The type reader of this parameter. </returns>
    /// <exception cref="InvalidOperationException"> The type for the command must be a class with a public parameterless constructor to use as a NamedArgumentType. </exception>
    private TypeReader? GetReader(Type? type)
    {
        if (type is null) return null;
        CommandService commands = Command.Module.Service;
        if (type.GetTypeInfo().GetCustomAttribute<NamedArgumentTypeAttribute>() != null)
        {
            IsRemainder = true;
            TypeReader? reader = commands.GetTypeReaders(type)?.FirstOrDefault().Value;
            if (reader == null)
            {
                Type readerType;
                try
                {
                    readerType = typeof(NamedArgumentTypeReader<>).MakeGenericType(type);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(
                        $"Parameter type '{type.Name}' for command '{Command.Name}' must be a class with a public parameterless constructor to use as a NamedArgumentType.",
                        ex);
                }

                reader = (TypeReader?)Activator.CreateInstance(readerType, commands);
                if (reader != null)
                    commands.AddTypeReader(type, reader);
            }

            return reader;
        }

        IDictionary<Type, TypeReader>? readers = commands.GetTypeReaders(type);
        return readers != null ? readers.FirstOrDefault().Value : commands.GetDefaultTypeReader(type);
    }

    /// <summary>
    ///     设置此参数的摘要。
    /// </summary>
    /// <param name="summary"> 此参数的摘要。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///     设置此参数的默认值。
    /// </summary>
    /// <param name="defaultValue"> 此参数的默认值。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder WithDefault(object defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    /// <summary>
    ///     设置此参数是否为可选参数。
    /// </summary>
    /// <param name="isOptional"> 此参数是否为可选参数。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder WithIsOptional(bool isOptional)
    {
        IsOptional = isOptional;
        return this;
    }

    /// <summary>
    ///     设置此参数是否接收全部剩余参数。
    /// </summary>
    /// <param name="isRemainder"> 此参数是否接收全部剩余参数。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder WithIsRemainder(bool isRemainder)
    {
        IsRemainder = isRemainder;
        return this;
    }

    /// <summary>
    ///     设置此参数是否为多值参数。
    /// </summary>
    /// <param name="isMultiple"> 此参数是否为多值参数。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder WithIsMultiple(bool isMultiple)
    {
        IsMultiple = isMultiple;
        return this;
    }

    /// <summary>
    ///     添加特性到此参数。
    /// </summary>
    /// <param name="attributes"> 要添加到此参数的特性数组。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     添加先决条件到此参数。
    /// </summary>
    /// <param name="precondition"> 要添加到此参数的先决条件。 </param>
    /// <returns> 此参数构建器。 </returns>
    public ParameterBuilder AddPrecondition(ParameterPreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    internal ParameterInfo Build(CommandInfo info)
    {
        if ((TypeReader ??= GetReader(ParameterType)) is null)
            throw new InvalidOperationException($"No type reader found for type {ParameterType?.Name}, one must be specified");
        return new ParameterInfo(this, info, Command.Module.Service);
    }

    #endregion
}
