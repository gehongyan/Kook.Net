namespace KaiHeiLa.API;

internal enum SocketFrameType
{
    Event,
    Hello,
    Ping,
    Pong,
    Resume,
    Reconnect,
    ResumeAck
}