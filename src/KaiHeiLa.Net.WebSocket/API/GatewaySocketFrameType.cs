namespace KaiHeiLa.API;

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