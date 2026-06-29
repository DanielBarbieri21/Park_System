using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Services;
using Estacionamento.UI;
using System;
using System.Windows.Forms;

namespace Estacionamento
{
    public partial class LoginForm : Form
    {
        private readonly AuthenticationService _authenticationService;
        private readonly Func<Usuario, Form1> _mainFormFactory;
        private readonly ILogService _log;

        public LoginForm(AuthenticationService authenticationService, Func<Usuario, Form1> mainFormFactory, ILogService log)
        {
            _authenticationService = authenticationService;
            _mainFormFactory = mainFormFactory;
            _log = log;

            InitializeComponent();
            LoginStyleHelper.AplicarEstilo(this, lblTitulo, txtEmail, txtSenha, btnEntrar);
            lblEmail.Visible = false;
            lblSenha.Visible = false;
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            var identificador = txtEmail.Text.Trim();
            var senha = txtSenha.Text;
            var usuario = _authenticationService.Autenticar(identificador, senha);
            if (usuario != null)
            {
                Hide();
                _log.Info($"Login realizado: {usuario.Login} ({usuario.Tipo})");
                using var formPrincipal = _mainFormFactory(usuario);
                formPrincipal.ShowDialog();
                Close();
                return;
            }

            MessageBox.Show("Usuario ou senha invalidos.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
