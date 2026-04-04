using Estacionamento.Abstractions;
using Estacionamento.Infrastructure;
using Estacionamento.Repositories;
using Estacionamento.Services;
using System;
using System.Windows.Forms;

namespace Estacionamento
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IClock clock = new SystemClock();
            ILogService log = new FileLogService();
            IVeiculoRepository veiculoRepository = new VeiculoRepositorySQLite();
            IUsuarioRepository usuarioRepository = new UsuarioRepositorySQLite();

            var estacionamentoService = new EstacionamentoService(veiculoRepository, clock);
            var relatorioService = new RelatorioService();
            var authenticationService = new AuthenticationService(usuarioRepository, log);
            authenticationService.SeedDefaultAdminIfMissing();

            Application.Run(new LoginForm(authenticationService, () => new Form1(estacionamentoService, relatorioService, log), log));
        }
    }
}
