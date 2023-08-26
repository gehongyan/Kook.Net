public static ServiceCollection RegisterImplicitServices(this ServiceCollection collection, Type interfaceType, Type activatorType)
{
    // 获取当前程序集中的所有类型。有很多方法可以做到这一点，但这是最快的。
    foreach (var type in typeof(Program).Assembly.GetTypes())
    {
        if (interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
            collection.AddSingleton(interfaceType, type);
    }

    // 注册可以激活这些实例的类，以便您可以激活这些实例。
    collection.AddSingleton(activatorType);
}
