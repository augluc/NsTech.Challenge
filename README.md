# nstech Challenge - Order Management Service

> Serviço de Gerenciamento de Pedidos de alta performance, resiliência e concorrência.

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791)
![Docker](https://img.shields.io/badge/Docker-Enabled-0db7ed)
![Tests](https://img.shields.io/badge/Tests-8%2F8%20Passing-10b981)

---

## 🏛️ Arquitetura do Projeto

A solução foi organizada em camadas independentes segundo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

```text
nstech-challenge/
 ├── src/
 │    ├── NsTech.Challenge.Domain/         # Entidades, Enums, Exceções e Regras de Negócio
 │    ├── NsTech.Challenge.Application/    # Use Cases, DTOs, Validadores (FluentValidation)
 │    ├── NsTech.Challenge.Infrastructure/ # EF Core 9, PostgreSQL (xmin), Repositórios, Migrations
 │    └── NsTech.Challenge.Api/            # Controllers, JWT, Middlewares (RFC 7807), Serilog
 └── tests/
      ├── NsTech.Challenge.UnitTests/      # Testes unitários do Domínio e Regras de Negócio
      └── NsTech.Challenge.IntegrationTests/ # Testes E2E HTTP via WebApplicationFactory
```

## 🚀 Principais Decisões Técnicas

- **PostgreSQL & Concorrência Otimista (`xmin`):** Mapeamento da coluna nativa `xmin` como `rowVersion` no EF Core para prevenção de *Race Conditions* em atualizações concorrentes de estoque e pedidos sem locks pessimistas.
- **Global Exception Handling (RFC 7807):** Middleware customizado retornando respostas padronizadas `ProblemDetails` para erros de estoque, estado do pedido e exceções não tratadas.
- **Autenticação e Segurança:** Proteção dos endpoints da API via JWT Bearer Token (`/auth/token`).
- **Idempotência no Domínio:** Operações de transição de estado (`Confirm()` e `Cancel()`) implementadas de forma idempotente com devolução automática de estoque.
- **Automação de Migrations & Seeding:** Aplicação automática de migrações e carga inicial de produtos durante a inicialização da API (`DbInitializer`).

## 🛠️ Como Executar a Aplicação

A forma recomendada para rodar a solução inteira (API + Banco de Dados PostgreSQL) é utilizando o **Docker Compose**:

### 1. Execução Completa via Docker Compose (API + Banco)

Na raiz do projeto, execute:

```bash
docker-compose up --build -d
```

A API subirá automaticamente aplicará as migrations e o seeding no startup, ficando acessível em: `http://localhost:5000/swagger`

## 🧪 Como Executar os Testes

Para rodar a suíte completa com os testes unitários e testes funcionais E2E:

```bash
dotnet test
```

> **✅ Resultado da Suíte de Testes (100% Passing)**
>
> Todos os 8 testes (5 unitários + 3 de integração E2E) executados com sucesso em ~6,2s, cobrindo:
> - Autenticação JWT (`POST /auth/token`)
> - Criação de Pedidos e reserva de estoque (`POST /orders`)
> - Confirmação e cancelamento idempotentes (`POST /orders/{id}/confirm` e `/cancel`)
> - Proteção de rotas não autenticadas (`401 Unauthorized`)

## 🐳 Comandos Úteis do Docker

```bash
# Encerrar e remover containers
docker-compose down

# Encerrar containers e limpar volume de dados do PostgreSQL
docker-compose down -v
```

## 🔑 Autenticação na API

Para interagir com as rotas protegidas de pedidos, obtenha um token JWT.

### Exemplo via cURL

```bash
curl -X POST http://localhost:5000/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "username": "nstech",
    "password": "admin123"
  }'
```

### Payload JSON (Postman / Insomnia)

**Endpoint:** `POST /auth/token`
**Header:** `Content-Type: application/json`

```json
{
  "username": "nstech",
  "password": "admin123"
}
```

Adicione o token retornado ao cabeçalho das requisições: `Authorization: Bearer <SEU_TOKEN>`

---

*nstech Challenge - Order Management Service • .NET 9 • Clean Architecture & DDD*
