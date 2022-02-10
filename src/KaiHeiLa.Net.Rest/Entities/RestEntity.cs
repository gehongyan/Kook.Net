namespace KaiHeiLa.Rest
{
    public abstract class RestEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        internal BaseKaiHeiLaClient KaiHeiLa { get; }
        
        /// <inheritdoc />
        public T Id { get; }

        internal RestEntity(BaseKaiHeiLaClient kaiHeiLa, T id)
        {
            KaiHeiLa = kaiHeiLa;
            Id = id;
        }
    }
}