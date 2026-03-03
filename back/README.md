# INSPAND Developers Exam — Backend (resumo)

Este README contém o entendimento sobre a arquitetura do backend e observações finais sobre o trabalho entregue.

Resumo das entregas
- Endpoints CRUD para `Book` implementados em `src/WebAPI/Controllers/BooksController.cs` (padrão REST, usando DTOs).
- Entidade `Book` criada em `src/Domain/Entities/Book.cs` com validações de domínio (FluentValidation).
- Persistência com EF Core 10 em `src/Infrastructure/Data/SqlDbContext.cs` e migration inicial gerada/aplicada (`src/Infrastructure/Migrations`).
- Swagger (Swashbuckle) configurado para documentação OpenAPI.
- Disparo de evento de domínio `BookCreatedEvent` e `BookCreatedEventHandler` que usa `IEmailService` (simulado) — apenas disparo, sem envio real.

Entendimento sobre a arquitetura (Clean Architecture)

1) Qual o papel da camada `Domain`?
- Contém as entidades de negócio, regras de validação e contratos (interfaces) que modelam o problema. 
- É independente de infra e frameworks; concentra a lógica de domínio (ex.: `Book`, validações, eventos de domínio).

2) Qual o papel da camada `Infrastructure`?
- Implementa detalhes de infraestrutura: acesso a dados (EF Core), repositórios, implementação de serviços externos (ex.: `IEmailService`), handlers de eventos e configuração de dependências. 
- Fornece implementações usadas pela camada de aplicação/entrada.

3) Qual o papel da camada `WebAPI`?
- Ponto de entrada HTTP. Responsável por adaptar requests/responses (controllers, DTOs), validar entrada, orquestrar chamadas aos serviços de domínio e expor a API. 
- Também configura middlewares (Swagger, CORS, etc.).

Ponto de melhoria (prioritário)
- Implementar `Result Pattern` (tipo resultado explícito para serviços) para padronizar retornos sucessos/erros e evitar usar exceções como fluxo de controle.
- Adicionar tratamento global de erros (middleware) que converta exceções/erros de validação em respostas HTTP consistentes e logue corretamente.
- Melhorar model binding / DTOs e separar contratos de API do domínio (atualmente básico). Incluir validação de request model e exemplos no Swagger.

Observações finais
- Remover a connection string hardcoded usada para debugging antes de commitar ao repositório remoto e usar secrets/variáveis de ambiente.
- Recomenda-se adicionar testes unitários para `BookService` e handlers de evento, e considerar outbox pattern se for necessário garantir entrega de mensagens de forma resiliente.

