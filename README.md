# 🚗 ParkSystem — Sistema de Estacionamento

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/)
[![Platform Windows](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://learn.microsoft.com/dotnet/desktop/winforms/)
[![CI Build Status](https://img.shields.io/github/actions/workflow/status/DanielBarbieri21/Estacionamento/ci.yml?branch=main&style=for-the-badge&logo=github-actions&label=CI)](https://github.com/DanielBarbieri21/Estacionamento/actions)

Uma aplicação desktop robusta, moderna e de alta performance desenvolvida em **Windows Forms** e **.NET 8** para controle operacional de entrada e saída de veículos em estacionamentos. Conta com persistência de dados em **SQLite**, criptografia avançada de credenciais, relatórios gerenciais e pipeline de integração contínua (CI).

---

## 🌟 Principais Funcionalidades & Diferenciais

O projeto passou por melhorias profundas de segurança, arquitetura e usabilidade. Veja os pilares de evolução do sistema:

### 🛡️ 1. Segurança de Ponta
* **Criptografia de Credenciais:** Substituição de senhas em texto plano por hashes robustos baseados no algoritmo **PBKDF2 com Salt exclusivo** por usuário (`SenhaHash` e `SenhaSalt`).
* **Migração Automática (Zero Friction):** Usuários legados têm suas senhas migradas silenciosamente para o novo padrão seguro no primeiro login bem-sucedido.
* **Seed de Administrador Controlado:** O usuário admin padrão é inicializado de forma segura por meio de uma classe dedicada (`AuthenticationService`), reduzindo o risco de hardcoding inseguro.

### 🏛️ 2. Arquitetura Limpa e Injeção de Dependências
* **Desacoplamento e Testabilidade:** Introdução de contratos e abstrações fundamentais (ex: `IClock`, `ILogService`, `IVeiculoRepository`, `IUsuarioRepository`).
* **Composição Centralizada (Composition Root):** O ciclo de vida e a injeção manual das dependências ocorrem centralizadamente no `Program.cs`, facilitando testes unitários e de integração.
* **Serviços Especializados:** Divisão clara de responsabilidades com o `AuthenticationService` (segurança) e o `RelatorioService` (geração de dados analíticos).

### 🗄️ 3. Banco de Dados SQLite Resiliente
* **Auto-Migrations:** Execução automática do schema e de atualizações estruturais assim que o aplicativo é inicializado.
* **Auditoria de Registros:** Novas colunas temporais (`CreatedAtUtc`, `UpdatedAtUtc`) armazenadas em formato UTC (ISO 8601) para precisão histórica absoluta.
* **Integridade das Regras de Negócio:** Índice único parcial que impede que um veículo com uma mesma placa possua mais de uma entrada ativa ao mesmo tempo.
* **Índices Otimizados:** Alta performance em consultas frequentes por placa, data de entrada e de saída.

### 🧪 4. Garantia de Qualidade (QA)
* **Testes Unitários Automatizados:** Projeto de testes dedicado (`Estacionamento.Tests`) cobrindo cenários críticos de regras de cobrança, fluxo de entrada, permanência e saída.
* **EditorConfig:** Padronização global das regras de codificação e estilo do C# no projeto.

### 📊 5. Usabilidade e Operação Avançada
* **Fluxo de Saída Assistido:** Janela de confirmação com resumo dos valores antes da persistência definitiva.
* **Validação de Placas Inteligente:** Aceita o formato tradicional brasileiro (`ABC1234`) e o novo padrão Mercosul (`ABC1D23`).
* **Exportação Multiformato:** Geração de relatórios gerenciais em PDF e CSV com facilidade.
* **Logging Estruturado:** Rastreamento local de eventos operacionais e falhas em arquivos rotativos (`logs/app-YYYYMMDD.log`).

### 📦 6. Entrega Contínua (CI)
* **GitHub Actions:** Pipeline automatizado (`ci.yml`) que restaura, compila e valida toda a suíte de testes a cada push ou pull request.

---

## 📂 Estrutura do Projeto

Abaixo está representada a árvore principal do repositório com o mapeamento de cada componente:

```text
├── Estacionamento/                 # Código-fonte principal (WinForms)
│   ├── Abstractions/               # Interfaces e contratos (ILogService, IClock, etc.)
│   ├── Infrastructure/             # Implementações de infraestrutura (FileLogService, etc.)
│   ├── Models/                     # Modelos de dados e entidades do domínio
│   ├── Repositories/               # Acesso ao banco de dados SQLite (IVeiculoRepository, etc.)
│   ├── Security/                   # Lógica de criptografia e hashing de senhas
│   ├── Services/                   # Lógica de negócios (EstacionamentoService, AuthenticationService)
│   ├── UI/                         # Telas secundárias e formulários adicionais
│   ├── Form1.cs                    # Interface operacional principal (Dashboard)
│   ├── LoginForm.cs                # Tela de Login e autenticação
│   ├── UserManagementForm.cs       # Administração de usuários do sistema
│   └── Program.cs                  # Ponto de entrada e Composição Root do App
│
├── Estacionamento.Package/         # Projeto de empacotamento (Windows Application Packaging)
│   ├── Package.appxmanifest        # Declaração de capacidades e assets do app
│   └── Images/                     # Assets visuais de empacotamento
│
├── Estacionamento.Tests/           # Projeto de testes de unidade (xUnit / NUnit)
│
├── docs/                           # Documentação auxiliar do projeto
│   ├── STORE_PUBLISHING.md         # Guia e referências de publicação
│   └── privacy-policy.html         # Política de privacidade oficial
│
├── scripts/                        # Scripts utilitários de automação e build
│   └── build-msix.ps1              # Script PowerShell para geração manual do MSIX
│
└── tools/                          # Ferramentas auxiliares de desenvolvimento
    └── GenerateAppIcons/           # Utilitário C# para geração dos assets de imagens a partir da logo
```

---

## 🚀 Como Executar Localmente

### Pré-requisitos
* **.NET SDK 8.0**
* Sistema Operacional **Windows** (requerido para executar aplicações Windows Forms)

### Passo a Passo

1. **Clonar e Acessar o Repositório:**
   ```powershell
   git clone https://github.com/DanielBarbieri21/Estacionamento.git
   cd Estacionamento
   ```

2. **Restaurar Dependências:**
   ```powershell
   dotnet restore Estacionamento.sln
   ```

3. **Compilar a Solução:**
   ```powershell
   dotnet build Estacionamento.sln
   ```

4. **Executar a Aplicação:**
   ```powershell
   dotnet run --project Estacionamento/Estacionamento.csproj
   ```

---

## 🔑 Credenciais Iniciais de Acesso

Ao iniciar a aplicação pela primeira vez, utilize as credenciais padrão de administrador:

* **Usuário:** `admin`
* **Senha:** `246895`

> [!WARNING]
> Por motivos de segurança, é **fortemente recomendado** alterar a senha padrão no primeiro acesso através da funcionalidade de gerenciamento de usuários no menu administrativo.
---

## 🛠️ Solução de Problemas comuns

### Falha ao restaurar pacotes NuGet (`NU1101`)
Caso seu ambiente não consiga localizar as dependências, verifique se a fonte pública do NuGet está ativa e configurada. Execute:
```powershell
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
```
Ou valide seu arquivo `NuGet.config` na raiz do projeto.
