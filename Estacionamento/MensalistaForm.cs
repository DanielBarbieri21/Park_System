using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Repositories;
using Estacionamento.UI;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Estacionamento
{
    public sealed class MensalistaForm : Form
    {
        private readonly IMensalistaRepository _mensalistaRepository;
        private readonly ILogService _log;

        private readonly TabControl tabControl = new();
        private readonly TabPage tabCadastro = new();
        private readonly TabPage tabConsulta = new();

        // Controles de Cadastro
        private readonly TextBox txtNome = new();
        private readonly ComboBox cmbDependente = new();
        private readonly TextBox txtCpfCnpj = new();
        private readonly TextBox txtMatricula = new();
        private readonly TextBox txtChave = new();
        private readonly ComboBox cmbGrupo = new();
        private readonly ComboBox cmbTabelaPrecos = new();
        private readonly NumericUpDown numVagas = new();
        private readonly TextBox txtValor = new();
        private readonly NumericUpDown numDiaVencimento = new();
        private readonly NumericUpDown numRecorrencias = new();
        private readonly ComboBox cmbRegraTempoExcedente = new();
        private readonly TextBox txtTempoExcedente = new();
        private readonly CheckBox chkControlaVaga = new();
        private readonly TextBox txtValidadeLiberacao = new();
        private readonly CheckBox chkAtivo = new();

        private readonly Button btnNovo = new();
        private readonly Button btnSalvar = new();

        // Controles de Consulta
        private readonly DataGridView dgvMensalistas = new();
        private readonly TextBox txtBusca = new();
        private readonly Button btnBuscar = new();
        private readonly Button btnLimpar = new();
        private readonly Button btnExcluir = new();

        private int? mensalistaSelecionadoId;

        public MensalistaForm(IMensalistaRepository mensalistaRepository, ILogService log)
        {
            _mensalistaRepository = mensalistaRepository;
            _log = log;

            Text = "Cadastro e edição de credenciados - Mensalista";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 660);
            Size = new Size(880, 720);
            BackColor = Color.FromArgb(248, 249, 250);
            Font = new Font("Segoe UI", 9F);

            // Carregar o ícone do app
            var appIcon = AppBranding.LoadAppIcon();
            if (appIcon != null)
            {
                Icon = appIcon;
            }

            CriarLayout();
            CarregarDados();
            NovoRegistro();
        }

        private void CriarLayout()
        {
            // Header
            var header = new Label
            {
                Text = "  Cadastro e Edição de Credenciados — Mensalista",
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(32, 100, 140),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            tabControl.Dock = DockStyle.Fill;
            tabControl.Padding = new Point(12, 4);

            tabCadastro.Text = "Cadastro/ Edição de Mensalista";
            tabCadastro.BackColor = Color.White;

            tabConsulta.Text = "Mensalistas cadastrados";
            tabConsulta.BackColor = Color.White;

            ConfigurarAbaCadastro();
            ConfigurarAbaConsulta();

            tabControl.TabPages.Add(tabCadastro);
            tabControl.TabPages.Add(tabConsulta);

            var panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            panelMain.Controls.Add(tabControl);

            Controls.Add(panelMain);
            Controls.Add(header);
        }

        private void ConfigurarAbaCadastro()
        {
            var panelForm = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            int y = 20;
            int ySpacing = 32;

            AdicionarLinhaForm(panelForm, "Nome do mensalista", txtNome, y);
            y += ySpacing;

            cmbDependente.DropDownStyle = ComboBoxStyle.DropDownList;
            AdicionarLinhaForm(panelForm, "Dependente de credenciado", cmbDependente, y);
            y += ySpacing;

            AdicionarLinhaForm(panelForm, "CPF/CNPJ", txtCpfCnpj, y, " [Apenas números]");
            y += ySpacing;

            AdicionarLinhaForm(panelForm, "Matrícula (número interno)", txtMatricula, y);
            y += ySpacing;

            AdicionarLinhaForm(panelForm, "Chave (identificador único)", txtChave, y);
            y += ySpacing;

            cmbGrupo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGrupo.Items.AddRange(new object[] { "Grupo A", "Grupo B", "Grupo C", "Grupo D" });
            cmbGrupo.SelectedIndex = 2; // Grupo C padrão
            AdicionarLinhaForm(panelForm, "Grupo do mensalista", cmbGrupo, y);
            y += ySpacing;

            cmbTabelaPrecos.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTabelaPrecos.Items.AddRange(new object[] { "MENSALISTA CARRO", "MENSALISTA MOTO", "MENSALISTA ESPECIAL" });
            cmbTabelaPrecos.SelectedIndex = 0;
            AdicionarLinhaForm(panelForm, "Tabela de preços", cmbTabelaPrecos, y);
            y += ySpacing;

            numVagas.Minimum = 1;
            numVagas.Maximum = 1000;
            numVagas.Value = 1;
            AdicionarLinhaForm(panelForm, "Quantidade de vagas", numVagas, y);
            y += ySpacing;

            txtValor.Text = "195,00";
            AdicionarLinhaForm(panelForm, "Valor R$", txtValor, y);
            y += ySpacing;

            numDiaVencimento.Minimum = 1;
            numDiaVencimento.Maximum = 31;
            numDiaVencimento.Value = 7;
            AdicionarLinhaForm(panelForm, "Dia de vencimento", numDiaVencimento, y);
            y += ySpacing;

            numRecorrencias.Minimum = 1;
            numRecorrencias.Maximum = 120;
            numRecorrencias.Value = 3;
            AdicionarLinhaForm(panelForm, "Quantidade de recorrências", numRecorrencias, y, " [mês]");
            y += ySpacing;

            cmbRegraTempoExcedente.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRegraTempoExcedente.Items.AddRange(new object[] { "COBRAR NA SAIDA", "TOLERANCIA DE 15 MIN", "ISENTO" });
            cmbRegraTempoExcedente.SelectedIndex = 0;
            AdicionarLinhaForm(panelForm, "Regras de tempo excedente", cmbRegraTempoExcedente, y);
            y += ySpacing;

            txtTempoExcedente.Text = "0";
            AdicionarLinhaForm(panelForm, "Tempo excedente", txtTempoExcedente, y, " [minutos]");
            y += ySpacing;

            chkControlaVaga.Text = "Limitado a 1 cartão por vaga";
            chkControlaVaga.Checked = true;
            AdicionarLinhaForm(panelForm, "Controla vaga por cartão", chkControlaVaga, y);
            y += ySpacing;

            txtValidadeLiberacao.Text = "";
            AdicionarLinhaForm(panelForm, "Validade de liberação", txtValidadeLiberacao, y, " (DD/MM/AAAA ou vazio)");
            y += ySpacing;

            chkAtivo.Text = "Mensalista ativo";
            chkAtivo.Checked = true;
            AdicionarLinhaForm(panelForm, "Status do mensalista", chkAtivo, y);
            y += ySpacing + 10;

            // Painel de botões
            var panelBotoes = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(580, 50)
            };

            btnNovo.Text = "Novo";
            btnNovo.Size = new Size(110, 36);
            btnNovo.Location = new Point(150, 5);
            btnNovo.FlatStyle = FlatStyle.Flat;
            btnNovo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNovo.Click += (_, _) => NovoRegistro();

            btnSalvar.Text = "Salvar";
            btnSalvar.Size = new Size(110, 36);
            btnSalvar.Location = new Point(270, 5);
            btnSalvar.FlatStyle = FlatStyle.Flat;
            btnSalvar.BackColor = Color.FromArgb(46, 204, 113);
            btnSalvar.ForeColor = Color.White;
            btnSalvar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSalvar.Click += BtnSalvar_Click;

            panelBotoes.Controls.AddRange(new Control[] { btnNovo, btnSalvar });
            panelForm.Controls.Add(panelBotoes);

            tabCadastro.Controls.Add(panelForm);
        }

        private void ConfigurarAbaConsulta()
        {
            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            var lblBusca = new Label
            {
                Text = "Filtrar por Nome/CPF:",
                Location = new Point(20, 20),
                Size = new Size(130, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            txtBusca.Location = new Point(160, 17);
            txtBusca.Size = new Size(250, 24);

            btnBuscar.Text = "Buscar";
            btnBuscar.Location = new Point(420, 16);
            btnBuscar.Size = new Size(80, 26);
            btnBuscar.Click += (_, _) => BuscarMensalistas();

            btnLimpar.Text = "Limpar";
            btnLimpar.Location = new Point(510, 16);
            btnLimpar.Size = new Size(80, 26);
            btnLimpar.Click += (_, _) => { txtBusca.Clear(); BuscarMensalistas(); };

            panelTop.Controls.AddRange(new Control[] { lblBusca, txtBusca, btnBuscar, btnLimpar });

            dgvMensalistas.Dock = DockStyle.Fill;
            dgvMensalistas.BackgroundColor = Color.White;
            dgvMensalistas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMensalistas.MultiSelect = false;
            dgvMensalistas.AllowUserToAddRows = false;
            dgvMensalistas.AllowUserToDeleteRows = false;
            dgvMensalistas.ReadOnly = true;
            dgvMensalistas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMensalistas.RowHeadersVisible = false;
            dgvMensalistas.DoubleClick += DgvMensalistas_DoubleClick;

            var panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };

            btnExcluir.Text = "Excluir Selecionado";
            btnExcluir.Size = new Size(160, 36);
            btnExcluir.Location = new Point(20, 12);
            btnExcluir.BackColor = Color.FromArgb(231, 76, 60);
            btnExcluir.ForeColor = Color.White;
            btnExcluir.FlatStyle = FlatStyle.Flat;
            btnExcluir.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExcluir.Click += BtnExcluir_Click;

            panelBottom.Controls.Add(btnExcluir);

            tabConsulta.Controls.AddRange(new Control[] { dgvMensalistas, panelTop, panelBottom });
            dgvMensalistas.BringToFront();
        }

        private void AdicionarLinhaForm(Panel container, string labelText, Control inputControl, int yPos, string suffixText = "")
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(20, yPos),
                Size = new Size(180, 24),
                BackColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleRight,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            inputControl.Location = new Point(210, yPos);
            inputControl.Size = new Size(350, 24);

            container.Controls.Add(lbl);
            container.Controls.Add(inputControl);

            if (!string.IsNullOrEmpty(suffixText))
            {
                var lblSuffix = new Label
                {
                    Text = suffixText,
                    Location = new Point(565, yPos),
                    Size = new Size(180, 24),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(127, 140, 141)
                };
                container.Controls.Add(lblSuffix);
            }
        }

        private void CarregarDados()
        {
            BuscarMensalistas();

            // Carregar combobox de dependente
            try
            {
                var todos = _mensalistaRepository.Listar();
                cmbDependente.Items.Clear();
                cmbDependente.Items.Add("NÃO VINCULADO");
                foreach (var item in todos)
                {
                    if (item.Id != mensalistaSelecionadoId)
                    {
                        cmbDependente.Items.Add($"{item.Nome} ({item.Matricula})");
                    }
                }
                cmbDependente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao carregar mensalistas para dependentes.", ex);
            }
        }

        private void BuscarMensalistas()
        {
            try
            {
                var lista = _mensalistaRepository.Listar();
                var busca = txtBusca.Text.Trim();

                if (!string.IsNullOrEmpty(busca))
                {
                    lista = lista.Where(m => m.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                                             m.CpfCnpj.Contains(busca)).ToList();
                }

                dgvMensalistas.DataSource = null;
                dgvMensalistas.Columns.Clear();
                dgvMensalistas.DataSource = lista.Select(m => new
                {
                    m.Id,
                    m.Nome,
                    m.CpfCnpj,
                    m.Matricula,
                    m.Chave,
                    m.Grupo,
                    m.TabelaPrecos,
                    Valor = $"R$ {m.Valor:F2}",
                    Ativo = m.Ativo ? "SIM" : "NÃO"
                }).ToList();

                dgvMensalistas.AutoGenerateColumns = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar mensalistas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NovoRegistro()
        {
            mensalistaSelecionadoId = null;
            txtNome.Clear();
            cmbDependente.SelectedIndex = 0;
            txtCpfCnpj.Clear();
            txtMatricula.Clear();
            txtChave.Clear();
            cmbGrupo.SelectedIndex = 2;
            cmbTabelaPrecos.SelectedIndex = 0;
            numVagas.Value = 1;
            txtValor.Text = "195,00";
            numDiaVencimento.Value = 7;
            numRecorrencias.Value = 3;
            cmbRegraTempoExcedente.SelectedIndex = 0;
            txtTempoExcedente.Text = "0";
            chkControlaVaga.Checked = true;
            txtValidadeLiberacao.Clear();
            chkAtivo.Checked = true;

            btnSalvar.Text = "Salvar";
        }

        private void DgvMensalistas_DoubleClick(object? sender, EventArgs e)
        {
            if (dgvMensalistas.SelectedRows.Count == 0) return;
            var id = (int)dgvMensalistas.SelectedRows[0].Cells["Id"].Value;

            try
            {
                var m = _mensalistaRepository.ObterPorId(id);
                if (m == null) return;

                mensalistaSelecionadoId = m.Id;
                txtNome.Text = m.Nome;
                txtCpfCnpj.Text = m.CpfCnpj;
                txtMatricula.Text = m.Matricula;
                txtChave.Text = m.Chave;
                
                // Set dependent
                cmbDependente.SelectedIndex = 0;
                if (!string.IsNullOrEmpty(m.DependenteDe))
                {
                    for (int i = 0; i < cmbDependente.Items.Count; i++)
                    {
                        if (cmbDependente.Items[i].ToString()!.Contains(m.DependenteDe))
                        {
                            cmbDependente.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // Set Grupo
                var idxGrupo = cmbGrupo.Items.IndexOf(m.Grupo);
                if (idxGrupo >= 0) cmbGrupo.SelectedIndex = idxGrupo;

                // Set TabelaPrecos
                var idxTab = cmbTabelaPrecos.Items.IndexOf(m.TabelaPrecos);
                if (idxTab >= 0) cmbTabelaPrecos.SelectedIndex = idxTab;

                numVagas.Value = Math.Max(1, m.Vagas);
                txtValor.Text = m.Valor.ToString("F2");
                numDiaVencimento.Value = Math.Max(1, Math.Min(31, m.DiaVencimento));
                numRecorrencias.Value = Math.Max(1, m.Recorrencias);

                // Set RegraTempoExcedente
                var idxRegra = cmbRegraTempoExcedente.Items.IndexOf(m.RegraTempoExcedente);
                if (idxRegra >= 0) cmbRegraTempoExcedente.SelectedIndex = idxRegra;

                txtTempoExcedente.Text = m.TempoExcedente.ToString();
                chkControlaVaga.Checked = m.ControlaVagaPorCartao;
                txtValidadeLiberacao.Text = m.ValidadeLiberacao;
                chkAtivo.Checked = m.Ativo;

                btnSalvar.Text = "Atualizar";
                tabControl.SelectedTab = tabCadastro;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar mensalista para edição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            var nome = txtNome.Text.Trim();
            if (string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("O Nome do mensalista é obrigatório.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtValor.Text.Replace("R$", "").Trim(), out decimal valor))
            {
                MessageBox.Show("Informe um valor mensal válido.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtTempoExcedente.Text, out int tempoExcedente))
            {
                tempoExcedente = 0;
            }

            var dependenteTexto = cmbDependente.SelectedIndex > 0 ? cmbDependente.SelectedItem.ToString() : "";
            // extrair matricula ou nome entre parenteses
            if (!string.IsNullOrEmpty(dependenteTexto) && dependenteTexto.Contains('(') && dependenteTexto.Contains(')'))
            {
                int start = dependenteTexto.IndexOf('(') + 1;
                int len = dependenteTexto.IndexOf(')') - start;
                dependenteTexto = dependenteTexto.Substring(start, len);
            }

            try
            {
                var m = new Mensalista
                {
                    Id = mensalistaSelecionadoId ?? 0,
                    Nome = nome,
                    DependenteDe = dependenteTexto ?? string.Empty,
                    CpfCnpj = txtCpfCnpj.Text.Trim(),
                    Matricula = txtMatricula.Text.Trim(),
                    Chave = txtChave.Text.Trim(),
                    Grupo = cmbGrupo.SelectedItem?.ToString() ?? "Grupo C",
                    TabelaPrecos = cmbTabelaPrecos.SelectedItem?.ToString() ?? "MENSALISTA CARRO",
                    Vagas = (int)numVagas.Value,
                    Valor = valor,
                    DiaVencimento = (int)numDiaVencimento.Value,
                    Recorrencias = (int)numRecorrencias.Value,
                    RegraTempoExcedente = cmbRegraTempoExcedente.SelectedItem?.ToString() ?? "COBRAR NA SAIDA",
                    TempoExcedente = tempoExcedente,
                    ControlaVagaPorCartao = chkControlaVaga.Checked,
                    ValidadeLiberacao = txtValidadeLiberacao.Text.Trim(),
                    Ativo = chkAtivo.Checked
                };

                if (mensalistaSelecionadoId == null)
                {
                    _mensalistaRepository.Adicionar(m);
                    _log.Info($"Mensalista cadastrado: {m.Nome}");
                    MessageBox.Show("Mensalista cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _mensalistaRepository.Atualizar(m);
                    _log.Info($"Mensalista atualizado: {m.Nome}");
                    MessageBox.Show("Mensalista atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CarregarDados();
                NovoRegistro();
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao salvar mensalista.", ex);
                MessageBox.Show($"Erro ao salvar mensalista: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            if (dgvMensalistas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um mensalista no grid para excluir.", "Excluir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var id = (int)dgvMensalistas.SelectedRows[0].Cells["Id"].Value;
            var nome = dgvMensalistas.SelectedRows[0].Cells["Nome"].Value.ToString();

            var confirm = MessageBox.Show($"Deseja realmente excluir o mensalista '{nome}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _mensalistaRepository.Excluir(id);
                _log.Info($"Mensalista excluído: {nome} (ID: {id})");
                MessageBox.Show("Mensalista excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CarregarDados();
                NovoRegistro();
            }
            catch (Exception ex)
            {
                _log.Error("Erro ao excluir mensalista.", ex);
                MessageBox.Show($"Erro ao excluir mensalista: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
