namespace Estacionamento.Abstractions
{
    public interface ILogService
    {
        void Info(string message);
        void Error(string message, System.Exception? exception = null);
    }
}
