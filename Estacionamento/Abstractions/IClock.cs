namespace Estacionamento.Abstractions
{
    public interface IClock
    {
        System.DateTime UtcNow { get; }
    }
}
