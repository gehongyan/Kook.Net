using System.Reflection;

namespace Kook.Commands.Builders;

/// <summary>
///     Represents a parameter builder.
/// </summary>
public class ParameterBuilder
{
    #region ParameterBuilder

    private readonly List<ParameterPreconditionAttribute> _preconditions;
    private readonly List<Attribute> _attributes;

    /// <summary>
    ///     Gets the command builder that this parameter builder belongs to.
    /// </summary>
    public CommandBuilder Command { get; }

    /// <summary>
    ///     Gets the name of this parameter.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     Gets the type of this parameter.
    /// </summary>
    public Type? ParameterType { get; internal set; }

    /// <summary>
    ///     Gets the type reader of this parameter.
    /// </summary>
    public TypeReader? TypeReader { get; set; }

    /// <summary>
    ///     Gets or sets a value that indicates whether this parameter is an optional parameter or not.
    /// </summary>
    public bool IsOptional { get; set; }

    /// <summary>
    ///     Gets or sets a value that indicates whether this parameter is a remainder parameter or not.
    /// </summary>
    public bool IsRemainder { get; set; }

    /// <summary>
    ///     Gets or sets a value that indicates whether this parameter is a multiple parameter or not.
    /// </summary>
    public bool IsMultiple { get; set; }

    /// <summary>
    ///     Gets or sets the default value of this parameter.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    ///     Gets or sets the summary of this parameter.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    ///     Gets a read-only collection containing the preconditions of this parameter.
    /// </summary>
    public IReadOnlyList<ParameterPreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     Gets a read-only collection containing the attributes of this parameter.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    #endregion

    #region Automatic

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParameterBuilder"/> class.
    /// </summary>
    /// <param name="command"> The command builder that this parameter builder belongs to. </param>
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
    ///     Sets the summary of this parameter.
    /// </summary>
    /// <param name="summary"> The summary of this parameter. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///     Sets the default value of this parameter.
    /// </summary>
    /// <param name="defaultValue"> The default value of this parameter. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder WithDefault(object defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    /// <summary>
    ///     Sets whether this parameter is an optional parameter or not.
    /// </summary>
    /// <param name="isOptional"> Whether this parameter is an optional parameter or not. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder WithIsOptional(bool isOptional)
    {
        IsOptional = isOptional;
        return this;
    }

    /// <summary>
    ///     Sets whether this parameter is a remainder parameter or not.
    /// </summary>
    /// <param name="isRemainder"> Whether this parameter is a remainder parameter or not. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder WithIsRemainder(bool isRemainder)
    {
        IsRemainder = isRemainder;
        return this;
    }

    /// <summary>
    ///     Sets whether this parameter is a multiple parameter or not.
    /// </summary>
    /// <param name="isMultiple"> Whether this parameter is a multiple parameter or not. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder WithIsMultiple(bool isMultiple)
    {
        IsMultiple = isMultiple;
        return this;
    }

    /// <summary>
    ///     Adds attributes to this parameter.
    /// </summary>
    /// <param name="attributes"> An array containing the attributes to add. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     Adds a precondition to this parameter.
    /// </summary>
    /// <param name="precondition"> The precondition to add. </param>
    /// <returns> This parameter builder. </returns>
    public ParameterBuilder AddPrecondition(ParameterPreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    /// <summary>
    ///     Builds this parameter builder.
    /// </summary>
    /// <param name="info"> The command info that this parameter belongs to. </param>
    /// <returns> The built parameter info. </returns>
    /// <exception cref="InvalidOperationException"> No type reader was found for this parameter, which must be specified. </exception>
    internal ParameterInfo Build(CommandInfo info)
    {
        if ((TypeReader ??= GetReader(ParameterType)) is null)
            throw new InvalidOperationException($"No type reader found for type {ParameterType?.Name}, one must be specified");
        return new ParameterInfo(this, info, Command.Module.Service);
    }

    #endregion
}
