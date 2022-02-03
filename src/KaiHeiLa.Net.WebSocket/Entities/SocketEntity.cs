namespace KaiHeiLa.WebSocket
{
    public abstract class SocketEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        internal KaiHeiLaSocketClient KaiHeiLa { get; }
        
        /// <inheritdoc />
        public T Id { get; }

        internal SocketEntity(KaiHeiLaSocketClient kaiHeiLa, T id)
        {
            KaiHeiLa = kaiHeiLa;
            Id = id;
        }
    }
}