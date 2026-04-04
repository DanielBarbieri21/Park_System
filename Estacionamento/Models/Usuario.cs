namespace Estacionamento.Models
{
    public enum TipoUsuario
    {
        Admin,
        Operador
    }

    public class Usuario
    {
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string SenhaSalt { get; set; } = string.Empty;
    public TipoUsuario Tipo { get; set; }
    }
}
