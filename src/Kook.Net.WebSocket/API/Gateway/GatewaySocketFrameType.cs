namespace Kook.API.Gateway;

internal enum GatewaySocketFrameType
{
    Event = 0,
    Hello = 1,
    Ping = 2,
    Pong = 3,
    Resume = 4,
    Reconnect = 5,
    ResumeAck = 6
}
