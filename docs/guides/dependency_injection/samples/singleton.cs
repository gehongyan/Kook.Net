// 带有服务接口类型
collection.AddSingleton<ISingletonService, SingletonService>();

// 不带有服务接口类型
collection.AddSingleton<SingletonService>();
