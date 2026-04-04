# Sistema de Estacionamento (WinForms + .NET 8)

Aplicacao desktop para controle de entrada e saida de veiculos, com dashboard operacional, autenticacao, relatorios e persistencia em SQLite.

## Upgrade aplicado

### 1) Seguranca
- Senhas migradas para hash PBKDF2 com salt (`SenhaHash` e `SenhaSalt`).
- Suporte a migracao automatica de usuarios legados com senha em texto plano no primeiro login.
- Seed de admin padrao controlado por `AuthenticationService`.

### 2) Arquitetura
- Introduzidas abstracoes (`IClock`, `ILogService`).
- Repositorios com interfaces (`IVeiculoRepository`, `IUsuarioRepository`).
- Composicao centralizada no `Program.cs` (injeção manual de dependencias).
- Servico de autenticacao separado (`AuthenticationService`).
- Servico de relatorios separado (`RelatorioService`).

### 3) Banco de dados e migracoes
- Migracao automatica de schema em inicializacao.
- Novas colunas de auditoria em `Veiculos`: `CreatedAtUtc`, `UpdatedAtUtc`.
- Indice unico parcial para impedir mais de um registro ativo por placa.
- Indices para consultas por placa, entrada e saida.
- Persistencia de datas em UTC (ISO 8601).

### 4) Qualidade
- Projeto de testes unitarios criado em `Estacionamento.Tests`.
- Cobertura inicial de cenarios criticos de entrada e saida.
- `.editorconfig` adicionado para padronizacao.

### 5) Upgrade funcional
- Correcao do fluxo de saida: agora confirma antes de persistir.
- Validacao de placa atualizada para formatos:
  - `ABC1234`
  - `ABC1D23` (Mercosul)
- Relatorios com exportacao em PDF e CSV.
- Logging de eventos e falhas em arquivo (`logs/app-YYYYMMDD.log`).

### 6) Operacao e entrega
- Pipeline GitHub Actions (`.github/workflows/ci.yml`) com restore, build e test.

## Estrutura principal

- `Estacionamento/Program.cs`: composicao da aplicacao.
- `Estacionamento/Form1.cs`: UI principal.
- `Estacionamento/LoginForm.cs`: tela de autenticacao.
- `Estacionamento/Services/EstacionamentoService.cs`: regras de negocio.
- `Estacionamento/Services/AuthenticationService.cs`: autenticacao e migracao de credenciais.
- `Estacionamento/Services/RelatorioService.cs`: exportacao PDF/CSV.
- `Estacionamento/Repositories/`: acesso a SQLite e contratos.
- `Estacionamento/Security/PasswordHasher.cs`: hash e verificacao de senha.
- `Estacionamento/Infrastructure/FileLogService.cs`: log local.
- `Estacionamento.Tests/`: testes unitarios.

## Como executar

1. Restaurar pacotes:
   - `dotnet restore Estacionamento.sln`
2. Compilar:
   - `dotnet build Estacionamento.sln`
3. Executar app (via Visual Studio ou `dotnet run` no projeto WinForms).

## Credencial inicial

- Usuario: `admin`
- Senha inicial: `246895`

Recomendacao: trocar senha inicial no primeiro acesso com uma funcionalidade de administracao de usuarios.

## Requisitos

- .NET SDK 8.0
- Windows (WinForms)

## Observacao de ambiente

Se o restore falhar com `NU1101`, configure o NuGet para incluir a fonte oficial `https://api.nuget.org/v3/index.json`.
