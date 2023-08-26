// 带有服务接口类型
collection.AddScoped<IScopedService, ScopedService>();

// 不带有服务接口类型
collection.AddScoped<ScopedService>();
