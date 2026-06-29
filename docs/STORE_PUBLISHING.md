# Publicação na Microsoft Store

Guia para empacotar e publicar o **ParkSystem - Sistema de Estacionamento** na Microsoft Store.

## Pré-requisitos

1. **Visual Studio 2022+** com a carga de trabalho:
   - Desenvolvimento para desktop com C++
   - **Ferramentas de empacotamento MSIX** (MSIX Packaging Tools)
2. **Conta de desenvolvedor** no [Microsoft Partner Center](https://partner.microsoft.com/dashboard) (taxa única).
3. **.NET SDK 8.0** instalado.

## Estrutura de empacotamento

```
Estacionamento/              → Aplicativo WinForms (.NET 8)
Estacionamento.Package/      → Projeto MSIX (wapproj)
  Package.appxmanifest       → Metadados da Store
  Images/                    → Ícones e splash screen
Estacionamento/Resources/    → Ícone do executável e logo in-app
docs/privacy-policy.html     → Política de privacidade (hospedar online)
```

## 1. Gerar ícones e assets

Execute o gerador de ícones (caso precise recriar os assets):

```powershell
dotnet run --project tools/GenerateAppIcons/GenerateAppIcons.csproj
```

Isso gera:
- `Estacionamento.Package/Images/*` — tiles e splash para MSIX
- `Estacionamento/Resources/app.ico` — ícone do executável
- `Estacionamento/Resources/logo_parksystem.png` — logo na tela de login

## 2. Compilar o pacote MSIX (Visual Studio)

1. Abra `Estacionamento.sln` no Visual Studio.
2. Selecione a configuração **Release | x64**.
3. Defina **Estacionamento.Package** como projeto de inicialização.
4. Menu **Projeto → Publicar → Criar pacotes de aplicativos...**
5. Escolha **Microsoft Store usando uma nova reserva de nome** (ou sideload para testes).
6. Selecione arquitetura **x64**.
7. Gere o pacote `.msixupload` para envio à Store.

### Script alternativo (linha de comando)

Requer Visual Studio 2022+ com:
- Carga **Desenvolvimento para desktop com C++**
- Componente **Ferramentas de empacotamento MSIX**
- **Windows 10 SDK (10.0.19041.0)** ou superior

Se aparecer o erro `APPX3217` (SDK UAP não encontrado), instale o Windows SDK pelo Instalador do Visual Studio.

```powershell
.\scripts\build-msix.ps1 -Configuration Release
```

O pacote será gerado em `Estacionamento.Package\AppPackages\`.

## 3. Configurar identidade do pacote

Antes de publicar, atualize o `Publisher` em `Estacionamento.Package/Package.appxmanifest`:

```xml
<Identity
  Name="ParkSystem.Estacionamento"
  Publisher="CN=SEU_PUBLISHER_DA_STORE"
  Version="1.0.0.0" />
```

O valor `Publisher` **deve coincidir exatamente** com o certificado exibido no Partner Center em:
**Conta → Gerenciamento de programas de desenvolvedor → Ver perfil**.

## 4. Política de privacidade

A Microsoft Store exige URL pública de política de privacidade.

1. Hospede `docs/privacy-policy.html` em:
   - GitHub Pages do repositório, ou
   - Site próprio.
2. Informe a URL no Partner Center ao criar a listagem.

Exemplo: `https://seu-usuario.github.io/Estacionamento/privacy-policy.html`

## 5. Metadados sugeridos para a Store

| Campo | Valor sugerido |
|-------|----------------|
| Nome | ParkSystem - Estacionamento |
| Descrição curta | Controle de vagas, entradas, saídas e faturamento. |
| Categoria | Negócios / Produtividade |
| Preço | Gratuito ou pago (sua escolha) |
| Capturas de tela | 1366×768 ou 1920×1080 (mín. 1) |
| Ícone da Store | `Images/Logo300.png` (300×300) |
| Palavras-chave | estacionamento, vagas, parking, faturamento, veículos |

## 6. Checklist antes do envio

- [ ] Trocar senha padrão `admin` / `246895` em produção
- [ ] Testar instalação do `.msix` em máquina limpa
- [ ] Verificar ícone na barra de tarefas e menu Iniciar
- [ ] Confirmar exportação PDF/CSV
- [ ] URL de privacidade publicada e acessível
- [ ] Publisher no manifest igual ao Partner Center
- [ ] Versão incrementada a cada envio (`1.0.0.0` → `1.0.1.0`)

## 7. Testar localmente (sideload)

```powershell
Add-AppxPackage -Path "Estacionamento.Package\AppPackages\...\Estacionamento.Package_1.0.0.0_x64.msix"
```

Para desinstalar:

```powershell
Get-AppxPackage *ParkSystem* | Remove-AppxPackage
```

## 8. Envio ao Partner Center

1. Crie uma nova aplicativo → reserva de nome.
2. Faça upload do `.msixupload` em **Envios**.
3. Preencha descrição, capturas, classificação etária e política de privacidade.
4. Envie para certificação (geralmente 1–3 dias úteis).

## Observações

- O app usa `runFullTrust` (desktop WinForms completo) — normal para apps de negócios na Store.
- Dados ficam **100% locais** (SQLite + logs); informe isso na listagem.
- Para atualizações, incremente `Version` no manifest e gere novo pacote.
