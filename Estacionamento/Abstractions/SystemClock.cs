namespace Estacionamento.Abstractions
{
    public sealed class SystemClock : IClock
    {
        public System.DateTime UtcNow => System.DateTime.UtcNow;
    }
}
