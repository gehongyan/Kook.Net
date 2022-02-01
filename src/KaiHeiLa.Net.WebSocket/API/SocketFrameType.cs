namespace KaiHeiLa.API;

public enum SocketFrameType
{
    Event,
    Hello,
    Ping,
    Pong,
    Resume,
    Reconnect,
    ResumeAck
}