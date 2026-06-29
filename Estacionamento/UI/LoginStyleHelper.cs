using System.Drawing;
using System.Windows.Forms;

namespace Estacionamento.UI
{
    internal static class LoginStyleHelper
    {
        public static void AplicarEstilo(LoginForm form, Label lblTitulo, TextBox txtEmail, TextBox txtSenha, Button btnEntrar)
        {
            form.BackColor = AppBranding.Background;
            form.ClientSize = new Size(440, 420);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Text = "ParkSystem - Login";
            form.Font = new Font("Segoe UI", 9F);

            var appIcon = AppBranding.LoadAppIcon();
            if (appIcon != null)
            {
                form.Icon = appIcon;
            }

            lblTitulo.Visible = false;

            var card = new Panel
            {
                Location = new Point(40, 30),
                Size = new Size(360, 340),
                BackColor = AppBranding.Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            var logo = AppBranding.LoadLogo(72);
            if (logo != null)
            {
                card.Controls.Add(new PictureBox
                {
                    Image = logo,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(144, 24),
                    Size = new Size(72, 72)
                });
            }

            card.Controls.Add(new Label
            {
                Text = "ParkSystem",
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                ForeColor = AppBranding.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, logo != null ? 104 : 24),
                Size = new Size(320, 32)
            });

            card.Controls.Add(new Label
            {
                Text = "Acesse sua conta para continuar",
                Font = new Font("Segoe UI", 9F),
                ForeColor = AppBranding.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, logo != null ? 136 : 56),
                Size = new Size(320, 20)
            });

            ConfigurarCampo(card, "E-mail ou usuário", txtEmail, 170);
            ConfigurarCampo(card, "Senha", txtSenha, 240, true);

            btnEntrar.Location = new Point(40, 290);
            btnEntrar.Size = new Size(280, 40);
            ButtonStyleHelper.AplicarBotaoAcao(btnEntrar, "Entrar", AppBranding.Success, IconHelper.Entrada);

            card.Controls.Add(txtEmail);
            card.Controls.Add(txtSenha);
            card.Controls.Add(btnEntrar);
            form.Controls.Add(card);

            form.AcceptButton = btnEntrar;
            txtEmail.TabIndex = 0;
            txtSenha.TabIndex = 1;
            btnEntrar.TabIndex = 2;
        }

        private static void ConfigurarCampo(Panel parent, string label, TextBox textBox, int y, bool password = false)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AppBranding.TextPrimary,
                Location = new Point(40, y),
                Size = new Size(280, 18)
            });

            textBox.Location = new Point(40, y + 22);
            textBox.Size = new Size(280, 28);
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            if (password)
            {
                textBox.PasswordChar = '●';
            }
        }
    }
}
