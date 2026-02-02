# 📦 ECommerce Order Processing API

API REST desenvolvida em **.NET 8**, seguindo os princípios da **Clean Architecture**, com persistência em **SQL Server** para dados transacionais e **MongoDB** para logs e auditoria.

---

## 🧱 Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- MongoDB
- Docker e Docker Compose
- xUnit + Moq (testes unitários)

---

## 📂 Arquitetura

O projeto segue os princípios da **Clean Architecture**, com separação clara de responsabilidades:

- **API** → Controllers, configuração e exposição dos endpoints
- **Application** → Serviços, DTOs e regras de negócio
- **Domain** → Entidades e enums
- **Infrastructure** → Acesso a dados (EF Core, Repositórios)
- **Application.Tests** → Testes unitários (xUnit)

---

## 🚀 Como rodar o projeto

### ✅ Pré-requisitos

- Docker
- Docker Compose

---

## 🔐 Configuração de ambiente (.env)

Na **raiz do projeto**, crie um arquivo chamado `.env`:

```env
SA_PASSWORD=YourStrong@Password
```

⚠️ A senha do SQL Server **deve conter**:
- Pelo menos 8 caracteres
- Letra maiúscula
- Letra minúscula
- Número
- Caractere especial

Exemplo válido:

```env
SA_PASSWORD=Sql@123456
```

---

## ▶️ Subindo a aplicação

Na raiz do projeto, execute:

```bash
docker compose up -d --build
```

---

## 🌐 Acessos

- **API (Swagger)**  
  👉 http://localhost:8080/swagger

- **SQL Server**
  - Host: `localhost`
  - Porta: `1433`
  - Usuário: `sa`
  - Senha: definida no `.env`

- **MongoDB**
  - Host: `localhost`
  - Porta: `27017`
  - Database: `OrderProcessingLogs`

  **RabbitMQ (Management UI)**  
  👉 http://localhost:15672  
  - Usuário: `guest`  
  - Senha: `guest`

---

## 🧪 Testes unitários

Os testes unitários foram desenvolvidos utilizando **xUnit** e **Moq**, com foco na validação de regras de negócio da camada de Application.

Para rodar os testes localmente (fora do Docker):

```bash
dotnet test
```

---

## 🗃️ Banco de dados

- O banco **OrderProcessingDb** é criado automaticamente na inicialização da aplicação
- As migrations são aplicadas automaticamente via Entity Framework Core
- MongoDB é utilizado exclusivamente para logs e auditoria (before/after)

---

## 🔄 Mensageria e processamento assíncrono

A aplicação utiliza **RabbitMQ** para desacoplar a criação do pedido do seu processamento:

- Ao criar um pedido, a API publica uma mensagem em uma fila (`order-created`)
- Um **consumer** consome essa mensagem de forma assíncrona
- O pedido tem seu status atualizado de **Recebido** para **Processado**

---

## ⏱️ Processamento em background (Hangfire)

Durante o consumo da mensagem, é disparado um job em background utilizando **Hangfire**, responsável por simular o envio de um e-mail de confirmação do pedido.

- O envio de e-mail é apenas simulado (fake email)
- A execução do job pode ser acompanhada através dos **logs dos containers Docker**

---

## 🧠 Observações técnicas

- SQL Server é utilizado apenas para dados transacionais
- MongoDB é utilizado para **logs** e **auditoria**
- A aplicação está preparada para rodar em ambiente containerizado com mínimo esforço
- Foi implementado **filtro por status e paginação** na listagem de pedidos, permitindo consultas como `?status=Processado`. Caso o status não seja informado, são retornados apenas pedidos ativos
- Foi adotada a estratégia de **Soft Delete** para a entidade **Order**, utilizando a flag `IsActive`, permitindo a desativação lógica de pedidos sem perda de histórico, o que facilita auditoria e rastreabilidade dos dados
- O processamento de pedidos ocorre de forma **assíncrona**, utilizando **RabbitMQ**, evitando bloqueios na requisição principal
- Jobs em background são executados com **Hangfire**, simulando o envio de e-mails e permitindo acompanhamento via logs

---