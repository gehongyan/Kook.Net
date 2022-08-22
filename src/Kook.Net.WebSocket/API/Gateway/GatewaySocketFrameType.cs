namespace Kook.API.Gateway;

internal enum GatewaySocketFrameType
{
    Event,
    Hello,
    Ping,
    Pong,
    Resume,
    Reconnect,
    ResumeAck
}