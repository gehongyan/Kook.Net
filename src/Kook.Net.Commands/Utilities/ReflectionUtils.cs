using System.Reflection;

namespace Kook.Commands;

internal static class ReflectionUtils
{
    private static readonly TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();

    internal static T CreateObject<T>(TypeInfo typeInfo, CommandService commands, IServiceProvider services) =>
        CreateBuilder<T>(typeInfo, commands)(services);

    internal static Func<IServiceProvider, T> CreateBuilder<T>(TypeInfo typeInfo, CommandService commands)
    {
        ConstructorInfo constructor = GetConstructor(typeInfo);
        System.Reflection.ParameterInfo[] parameters = constructor.GetParameters();
        PropertyInfo[] properties = GetProperties(typeInfo);

        return services =>
        {
            object[] args = parameters
                .Select(x => GetMember(commands, services, x.ParameterType, typeInfo))
                .ToArray();

            T obj = InvokeConstructor<T>(constructor, args, typeInfo);
            foreach (PropertyInfo property in properties) property.SetValue(obj, GetMember(commands, services, property.PropertyType, typeInfo));
            return obj;
        };
    }

    private static T InvokeConstructor<T>(ConstructorInfo constructor, object[] args, TypeInfo ownerType)
    {
        try
        {
            return (T)constructor.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create \"{ownerType.FullName}\".", ex);
        }
    }

    private static ConstructorInfo GetConstructor(TypeInfo ownerType)
    {
        ConstructorInfo[] constructors = ownerType.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
        return constructors.Length switch
        {
            0 => throw new InvalidOperationException($"No constructor found for \"{ownerType.FullName}\"."),
            > 1 => throw new InvalidOperationException($"Multiple constructors found for \"{ownerType.FullName}\"."),
            _ => constructors[0]
        };
    }

    private static PropertyInfo[] GetProperties(TypeInfo? ownerType)
    {
        List<PropertyInfo> result = [];
        while (ownerType != ObjectTypeInfo)
        {
            foreach (PropertyInfo prop in ownerType?.DeclaredProperties ?? [])
            {
                if (prop.SetMethod is { IsStatic: false, IsPublic: true }
                    && prop.GetCustomAttribute<DontInjectAttribute>() == null)
                    result.Add(prop);
            }
            ownerType = ownerType?.BaseType?.GetTypeInfo();
        }

        return result.ToArray();
    }

    private static object GetMember(CommandService commands, IServiceProvider services, Type memberType, TypeInfo ownerType)
    {
        if (memberType == typeof(CommandService)) return commands;
        if (memberType == typeof(IServiceProvider) || memberType == services.GetType()) return services;
        object? service = services.GetService(memberType);
        if (service != null) return service;
        throw new InvalidOperationException($"Failed to create \"{ownerType.FullName}\", dependency \"{memberType.Name}\" was not found.");
    }
}
