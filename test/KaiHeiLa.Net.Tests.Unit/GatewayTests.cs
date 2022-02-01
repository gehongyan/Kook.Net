using System;
using System.Threading.Tasks;
using KaiHeiLa.API;
using KaiHeiLa.WebSocket;
using Xunit;
using Xunit.Sdk;

namespace KaiHeiLa;

public class GatewayTests
{
    private readonly string _token;

    public GatewayTests()
    {
        _token = Environment.GetEnvironmentVariable("KaiHeiLaDebugToken", EnvironmentVariableTarget.User) 
                 ?? throw new ArgumentNullException(nameof(_token));
    }
}