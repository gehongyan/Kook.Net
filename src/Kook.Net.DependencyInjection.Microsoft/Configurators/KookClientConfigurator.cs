﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <inheritdoc cref="T:Kook.Net.DependencyInjection.Microsoft.IKookClientConfigurator`2" />
public abstract class KookClientConfigurator<TClient, TConfig>
    : IKookClientConfigurator<TClient, TConfig>, IKookClientConfiguratorCompleter
    where TClient : IKookClient
    where TConfig : KookConfig
{
    private readonly Action<TConfig> _baseConfigure;
    private Action<IServiceProvider, TConfig>? _appendedConfigure;
    private Action<IServiceCollection>? _appendedService;

    internal KookClientConfigurator(IServiceCollection services, Action<TConfig> baseConfigure)
    {
        ServiceCollection = services;
        _baseConfigure = baseConfigure;
    }

    /// <inheritdoc />
    public IServiceCollection ServiceCollection { get; }

    /// <inheritdoc />
    public IKookClientConfigurator<TClient, TConfig> AppendConfigure(Action<IServiceProvider, TConfig> configure)
    {
        _appendedConfigure += configure;
        return this;
    }

    /// <inheritdoc />
    public IKookClientConfigurator<TClient, TConfig> AppendService(Action<IServiceCollection> service)
    {
        _appendedService += service;
        return this;
    }

    /// <inheritdoc />
    public void Complete()
    {
        ServiceCollection.AddOptions();
        ServiceCollection.AddSingleton<IConfigureOptions<TConfig>>(provider =>
            new ConfigureNamedOptions<TConfig>(Options.DefaultName, config =>
            {
                _baseConfigure(config);
                _appendedConfigure?.Invoke(provider, config);
            }));
        _appendedService?.Invoke(ServiceCollection);
    }
}
