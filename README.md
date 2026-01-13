
# 🚗 Sistema de Estacionamento - WinForms (C#)

Este projeto é um sistema simples de gerenciamento de estacionamento, desenvolvido com **Windows Forms em C#**, que permite registrar a entrada e saída de veículos, calcular o tempo de permanência e o valor a pagar com base no valor da hora definido pelo usuário.

---

## 📸 Interface


---

## ⚙️ Funcionalidades

- Registrar entrada de veículos com placa, tipo e valor por hora.
- Registrar saída de veículos com cálculo automático do tempo e valor a pagar.
- Listagem dos veículos com entrada, saída, tempo de permanência e valor cobrado.
- Interface simples e intuitiva com DataGridView.
- Tipos de veículos disponíveis: **Carro**, **Moto**, **Caminhão**.

---

## 🧾 Tecnologias utilizadas

- 🖥️ **.NET Framework (Windows Forms)**
- 💻 **C#**
- 📋 **WinForms Designer** para UI

---

## 🚀 Como executar

1. Abra o projeto no **Visual Studio**.
2. Compile e execute com `F5` ou clique em **Start**.
3. Preencha:
   - **Placa**
   - **Tipo**
   - **Valor Hora**
4. Clique em **Registrar Entrada**.
5. Para registrar a saída, informe a placa novamente e clique em **Registrar Saída**.

---

## 🧠 Lógica de cálculo

O sistema calcula o valor total da permanência com base no **valor por hora informado na entrada**.

```csharp
var horas = Math.Ceiling(TempoPermanencia.TotalHours);
return horas * ValorHora;
```

---

## 📂 Estrutura de arquivos

```
├── Form1.cs               // Lógica principal da interface
├── Form1.Designer.cs      // Componentes e layout do formulário
├── Veiculo.cs             // Classe que representa o veículo
├── Program.cs             // Classe inicial do aplicativo
└── README.md              // Este arquivo
```

---

---

🛠️ **Software desenvolvido por Daniel Barbieri**  
Engenheiro de Software | Full Stack Developer  

Código construído com foco em eficiência, organização, escalabilidade e boas práticas de desenvolvimento.

🌐 GitHub: https://github.com/DanielBarbieri21  
💼 LinkedIn: https://www.linkedin.com/in/daniel-barbieri-4990462a/

---

---

## 📄 Licença

Este projeto é open-source e você pode utilizá-lo livremente para fins de estudo ou melhorias.
