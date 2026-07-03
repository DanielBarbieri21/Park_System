using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Repositories;
using Estacionamento.Security;
using Estacionamento.UI;
using Microsoft.Data.Sqlite;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Estacionamento
{
    public sealed class UserManagementForm : Form
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly Usuario _usuarioAtual;
        private readonly ILogService _log;

        private readonly DataGridView dgvUsuarios = new();
        private readonly TextBox txtNome = new();
        private readonly TextBox txtLogin = new();
        private readonly TextBox txtEmail = new();
        private readonly TextBox txtSenha = new();
        private readonly ComboBox cmbTipo = new();
        private readonly Button btnNovo = new();
        private readonly Button btnSalvar = new();
        private readonly Button btnExcluir = new();

        private int? usuarioSelecionadoId;

        public UserManagementForm(IUsuarioRepository usuarioRepository, Usuario usuarioAtual, ILogService log)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioAtual = usuarioAtual;
            _log = log;

            Text = "Gerenciar Usuários";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(860, 520);
            Size = new Size(920, 560);
            BackColor = Color.FromArgb(248, 249, 250);
            Font = new Font("Segoe UI", 9F);

            CriarLayout();
            CarregarUsuarios();
            LimparFormulario();
        }

        private void CriarLayout()
        {
            var header = new Label
            {
                Text = "  Administração de Usuários",
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(32, 100, 140),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvUsuarios.Location = new Point(20, 68);
            dgvUsuarios.Size = new Size(520, 400);
            dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.FixedSingle;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Usuario.Id), HeaderText = "ID", FillWeight = 35 });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Usuario.Nome), HeaderText = "Nome", FillWeight = 120 });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Usuario.Login), HeaderText = "Login", FillWeight = 90 });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Usuario.Email), HeaderText = "E-mail", FillWeight = 130 });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Usuario.Tipo), HeaderText = "Tipo", FillWeight = 70 });
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;

            var panelFormulario = new Panel
            {
                Location = new Point(560, 68),
                Size = new Size(320, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            AdicionarCampo(panelFormulario, "Nome:", txtNome, 20);
            AdicionarCampo(panelFormulario, "Login:", txtLogin, 76);
            AdicionarCampo(panelFormulario, "E-mail:", txtEmail, 132);
            AdicionarCampo(panelFormulario, "Senha:", txtSenha, 188);
            txtSenha.PasswordChar = '*';

            var lblSenhaAjuda = new Label
            {
                Text = "Deixe em branco para manter a senha atual.",
                Location = new Point(22, 238),
                Size = new Size(270, 20),
                ForeColor = Color.FromArgb(90, 90, 90)
            };

            var lblTipo = new Label
            {
                Text = "Perfil:",
                Location = new Point(22, 262),
                Size = new Size(270, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            cmbTipo.Location = new Point(22, 284);
            cmbTipo.Size = new Size(270, 25);
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.DataSource = Enum.GetValues(typeof(TipoUsuario));

            btnNovo.Text = "Novo";
            btnNovo.Location = new Point(22, 335);
            btnNovo.Size = new Size(80, 34);
            btnNovo.Click += (_, _) => LimparFormulario();

            btnSalvar.Text = "Salvar";
            btnSalvar.Location = new Point(112, 335);
            btnSalvar.Size = new Size(80, 34);
            btnSalvar.BackColor = Color.FromArgb(46, 204, 113);
            btnSalvar.ForeColor = Color.White;
            btnSalvar.FlatStyle = FlatStyle.Flat;
            btnSalvar.Click += BtnSalvar_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Location = new Point(202, 335);
            btnExcluir.Size = new Size(90, 34);
            btnExcluir.BackColor = Color.FromArgb(231, 76, 60);
            btnExcluir.ForeColor = Color.White;
            btnExcluir.FlatStyle = FlatStyle.Flat;
            btnExcluir.Click += BtnExcluir_Click;

            panelFormulario.Controls.AddRange(new Control[] { lblSenhaAjuda, lblTipo, cmbTipo, btnNovo, btnSalvar, btnExcluir });

            Controls.AddRange(new Control[] { header, dgvUsuarios, panelFormulario });
        }

        private static void AdicionarCampo(Control parent, string label, TextBox textBox, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(22, y),
                Size = new Size(270, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            textBox.Location = new Point(22, y + 22);
            textBox.Size = new Size(270, 25);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            parent.Controls.Add(textBox);
        }

        private void CarregarUsuarios()
        {
            dgvUsuarios.DataSource = _usuarioRepository.Listar().ToList();
        }

        private void DgvUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is not Usuario usuario)
            {
                return;
            }

            usuarioSelecionadoId = usuario.Id;
            txtNome.Text = usuario.Nome;
            txtLogin.Text = usuario.Login;
            txtEmail.Text = usuario.Email;
            txtSenha.Clear();
            cmbTipo.SelectedItem = usuario.Tipo;
            btnExcluir.Enabled = usuario.Id != _usuarioAtual.Id;
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                var usuario = MontarUsuarioValidado();
                if (usuario == null)
                {
                    return;
                }

                if (usuarioSelecionadoId.HasValue)
                {
                    AtualizarUsuario(usuario);
                }
                else
                {
                    CriarUsuario(usuario);
                }

                CarregarUsuarios();
                LimparFormulario();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Já existe um usuário com este login ou e-mail.", "Usuário duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao salvar usuário.", ex);
                MessageBox.Show(ex.Message, "Erro ao salvar usuário", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Usuario? MontarUsuarioValidado()
        {
            var nome = txtNome.Text.Trim();
            var login = txtLogin.Text.Trim();
            var email = txtEmail.Text.Trim();
            var senha = txtSenha.Text;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Informe nome, login e e-mail.", "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (!usuarioSelecionadoId.HasValue && string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Informe uma senha para o novo usuário.", "Senha obrigatória", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(senha) && senha.Length < 6)
            {
                MessageBox.Show("A senha deve ter pelo menos 6 caracteres.", "Senha fraca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new Usuario
            {
                Id = usuarioSelecionadoId ?? 0,
                Nome = nome,
                Login = login,
                Email = email,
                Tipo = (TipoUsuario)cmbTipo.SelectedItem!,
                Senha = senha
            };
        }

        private void CriarUsuario(Usuario usuario)
        {
            var credentials = PasswordHasher.HashPassword(usuario.Senha);
            usuario.SenhaHash = credentials.Hash;
            usuario.SenhaSalt = credentials.Salt;
            usuario.Senha = string.Empty;

            _usuarioRepository.Adicionar(usuario);
            _log.Info($"Usuário criado: {usuario.Login} ({usuario.Tipo}).");
            MessageBox.Show("Usuário criado com sucesso.", "Usuários", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AtualizarUsuario(Usuario usuario)
        {
            var usuarioOriginal = _usuarioRepository.ObterPorId(usuario.Id);
            if (usuarioOriginal == null)
            {
                MessageBox.Show("Usuário não encontrado.", "Usuários", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (usuarioOriginal.Tipo == TipoUsuario.Admin
                && usuario.Tipo != TipoUsuario.Admin
                && _usuarioRepository.ContarAdmins() <= 1)
            {
                MessageBox.Show("Não é permitido remover o perfil do último administrador.", "Proteção administrativa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _usuarioRepository.Atualizar(usuario);
            if (!string.IsNullOrWhiteSpace(usuario.Senha))
            {
                var credentials = PasswordHasher.HashPassword(usuario.Senha);
                _usuarioRepository.AtualizarCredenciais(usuario.Id, credentials.Hash, credentials.Salt);
            }

            _log.Info($"Usuário atualizado: {usuario.Login} ({usuario.Tipo}).");
            MessageBox.Show("Usuário atualizado com sucesso.", "Usuários", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            if (!usuarioSelecionadoId.HasValue)
            {
                return;
            }

            if (usuarioSelecionadoId.Value == _usuarioAtual.Id)
            {
                MessageBox.Show("Você não pode excluir o próprio usuário logado.", "Proteção administrativa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuario = _usuarioRepository.ObterPorId(usuarioSelecionadoId.Value);
            if (usuario == null)
            {
                return;
            }

            if (usuario.Tipo == TipoUsuario.Admin && _usuarioRepository.ContarAdmins() <= 1)
            {
                MessageBox.Show("Não é permitido excluir o último administrador.", "Proteção administrativa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacao = MessageBox.Show($"Excluir o usuário '{usuario.Login}'?", "Confirmar exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacao != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _usuarioRepository.Excluir(usuario.Id);
                _log.Info($"Usuário excluído: {usuario.Login} ({usuario.Tipo}).");
                CarregarUsuarios();
                LimparFormulario();
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao excluir usuário.", ex);
                MessageBox.Show(ex.Message, "Erro ao excluir usuário", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparFormulario()
        {
            usuarioSelecionadoId = null;
            txtNome.Clear();
            txtLogin.Clear();
            txtEmail.Clear();
            txtSenha.Clear();
            cmbTipo.SelectedItem = TipoUsuario.Operador;
            btnExcluir.Enabled = false;
            dgvUsuarios.ClearSelection();
            txtNome.Focus();
        }
    }
}
