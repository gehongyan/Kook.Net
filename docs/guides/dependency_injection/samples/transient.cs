// 带有服务接口类型
collection.AddTransient<ITransientService, TransientService>();

// 不带有服务接口类型
collection.AddTransient<TransientService>();
