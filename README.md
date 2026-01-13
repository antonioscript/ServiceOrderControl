# Instruções

- Caso o tempo não seja suficiente, priorize a **qualidade, o padrão e a estrutura do código**, definindo claramente quais funcionalidades não serão implementadas.
- Caso alguma funcionalidade não seja implementada, isso **deve ser documentado neste README**, explicando o motivo.
- O código fornecido contém **problemas que devem ser identificados e corrigidos**.
- Fique a vontade para criar, renomear e remover pastas,bibliotecas e até a solução não utilizadas.
- O sistema deve **compilar corretamente e executar todas as ações previstas**.
- O código final **não deve apresentar erros nem warnings** durante a compilação.
- Deve ser enviado via e-mail para consultoria com o link do projeto no Github. A consultaoria terá até terça-feira dia 13 as as 13 horas para encaminhar o e-mail.

---

## 1. Introdução

Sistema para um prestador de serviços (ou pequena equipe) registrar clientes, abrir ordens de serviço, acompanhar status, registrar valores e anexar fotos de antes/depois do serviço.

---

## 2. Funcionalidades Detalhadas

### 2.1 Cadastro de Cliente

#### Objetivo
Permitir registrar e consultar dados do cliente para vinculação em Ordens de Serviço (OS).

#### Campos (mínimo)
- Nome (obrigatório, 2–150 caracteres)
- Id (gerado pelo sistema)
- Telefone (opcional, até 30 caracteres)
- E-mail (opcional, até 120 caracteres, formato válido)
- Documento (CPF/CNPJ) (opcional, até 30 caracteres, sem validação pesada)
- Data de criação (gerado pelo sistema)

#### Regras de Negócio
1. Nome é obrigatório e não pode conter apenas whitespace.
2. Telefone e e-mail podem ser nulos; se informados, devem ser trimados.
3. Opcionalmente, bloquear ou alertar duplicidade por:
   - Documento (CPF/CNPJ), quando informado
   - Telefone, quando informado

#### Operações
- Criar cliente
- Consultar cliente por Id
- Buscar cliente por telefone ou documento

#### Casos de Teste
- Criar cliente com nome válido retorna 201 Created + id
- Criar cliente sem nome retorna 400 Validation Error
- Criar cliente com e-mail inválido retorna 400 Validation Error
- Criar cliente com telefone e buscar retorna dados consistentes
- Criar cliente com documento duplicado (se regra ativa) retorna 409 Conflict ou 400

---

### 2.2 Abertura de Ordem de Serviço

#### Objetivo
Criar uma OS vinculada a um cliente, com descrição e dados iniciais.

#### Campos (mínimo)
- ClienteId (obrigatório)
- Descrição do serviço (obrigatório, 1–500 caracteres)
- Número da OS (gerado automaticamente, sequencial/identity)
- Status (inicial = Aberta)
- Data de abertura (gerado pelo sistema)
- Valor do serviço (decimal(18,2)) (opcional no momento da abertura)
- Moeda (BRL)
- Data de atualização valor (opcional)

#### Regras de Negócio
1. Só é possível abrir OS para cliente existente.
2. Descrição é obrigatória.
3. Status inicial deve ser sempre Aberta.
4. Número da OS deve ser único e sequencial.
5. Regra de negócio item 2.4 

#### Operações
- Abrir OS
- Consultar OS por Id
- Listar OS por cliente, status ou período

#### Casos de Teste
- Abrir OS para cliente existente retorna 201 Created
- Abrir OS para cliente inexistente retorna 404 Not Found
- Abrir OS com descrição vazia retorna 400 Bad Request
- Consultar OS recém-criada retorna status Aberta

---

### 2.3 Status da Ordem de Serviço

#### Objetivo
Permitir acompanhar o ciclo do serviço.

#### Estados
- Aberta
- Em Execução
- Finalizada

#### Regras de Transição
- Aberta -> Em Execução (permitido)
- Em Execução -> Finalizada (permitido)
- Aberta -> Finalizada (bloqueado)
- Finalizada -> qualquer outro (bloqueado)

#### Operações
- Alterar status
- Registrar datas opcionais:
  - StartedAt ao entrar em Em Execução
  - FinishedAt ao entrar em Finalizada

#### Casos de Teste
- Alterar Aberta para Em Execução retorna 200 OK
- Alterar Em Execução para Finalizada retorna 200 OK
- Alterar Finalizada para outro status retorna 409 Conflict

---

### 2.4 Valor do Serviço

#### Objetivo
Permitir definir ou ajustar o valor do serviço.

#### Campos
- Valor (decimal(18,2))
- Moeda (BRL)
- Data de atualização (opcional)

#### Regras de Negócio
1. Valor pode ser nulo enquanto Aberta ou Em Execução.
2. Valor pode ser obrigatório para finalizar a OS.
3. Valor não pode ser negativo.
4. Após Finalizada, não permitir alteração.

#### Operações
- Definir ou alterar valor
- Validar valor ao finalizar OS

---

### 2.5 Fotos Antes / Depois (Opcional)

#### Objetivo
Permitir anexar evidências do serviço.

#### Campos do Anexo
- Id
- ServiceOrderId
- Type (Before | After)
- FileName
- ContentType (image/jpeg, image/png)
- SizeBytes
- StoragePath
- UploadedAt

#### Regras de Negócio
1. Aceitar apenas JPG e PNG.
2. Tamanho máximo sugerido: 5MB.
3. Permitir múltiplos anexos.
4. Upload local em /data/uploads (container ou volume).

---

## 3. API Sugerida

### Clientes
- POST /v1/customers
- GET /v1/customers/{id}

### Ordens de Serviço
- POST /v1/service-orders
- GET /v1/service-orders/{id}
- PATCH /v1/service-orders/{id}/status
- PUT /v1/service-orders/{id}/price
- POST /v1/service-orders/{id}/attachments/before
- POST /v1/service-orders/{id}/attachments/after
- GET /v1/service-orders/{id}/attachments

---

## 4. Requisitos Não Funcionais (Opcional)

### Performance
- Upload deve ser feito via streaming, evitando carregar todo o arquivo em memória.

### Segurança
- Validar content-type e extensão real do arquivo.
- Sanitizar nome do arquivo.

### Observabilidade
- Registrar logs para criação de cliente, abertura de OS e mudança de status.

-----------------------


<img width="1291" height="912" alt="image" src="https://github.com/user-attachments/assets/87d2bc18-da99-4fad-aa70-253546656022" />


<img width="964" height="547" alt="image" src="https://github.com/user-attachments/assets/fb87deea-315b-45cb-98d0-fca3e92152d2" />




-------------------


# ServiceOrderControl

API para cadastro de clientes e controle de Ordens de Serviço (OS), construída em .NET 8 com foco em **arquitetura em camadas, DDD**, boas práticas de **design orientado a domínio** e um código fácil de evoluir.

---

## Sumário

1. [Visão geral](#visão-geral)  
2. [Stack e principais decisões](#stack-e-principais-decisões)  
3. [Arquitetura da solução](#arquitetura-da-solução)  
   1. [Camada Domain](#camada-domain)  
   2. [Camada Application](#camada-application)  
   3. [Camada Infrastructure](#camada-infrastructure)  
   4. [Camada ApiService](#camada-apiservice)  
   5. [Módulos e composição (IModule)](#módulos-e-composição-imodule)  
4. [Modelagem de domínio](#modelagem-de-domínio)  
   1. [Cliente](#cliente)  
   2. [Ordem de Serviço](#ordem-de-serviço)  
5. [Fluxos principais da API](#fluxos-principais-da-api)  
   1. [Clientes](#clientes)  
   2. [Ordens de Serviço](#ordens-de-serviço)  
6. [Tratamento de erros e contrato de resposta](#tratamento-de-erros-e-contrato-de-resposta)  
7. [Observabilidade e qualidade](#observabilidade-e-qualidade)  
8. [Como executar o projeto](#como-executar-o-projeto)  
9. [Decisões de design e trade-offs](#decisões-de-design-e-trade-offs)  
10. [Próximos passos / melhorias possíveis](#próximos-passos--melhorias-possíveis)

---

## Visão geral

O **ServiceOrderControl** é uma API responsável por:

- Cadastrar **clientes**;
- Abrir **ordens de serviço** vinculadas a clientes;
- Atualizar o **status** de uma OS (ex.: Aberta, Em execução, Finalizada);
- Atualizar o **valor** da OS;
- Anexar imagens de **antes/depois** do serviço na OS;
- Consultar OS por **id**, **cliente**, **status** e/ou **período de abertura**.

O projeto foi construído com foco em:

- **Separação clara de responsabilidades** (DDD + camadas);
- **Facilidade de manutenção** (handlers por caso de uso, Result Pattern);
- **Observabilidade e tratativa de erros** coerente para quem consome a API;
- Código pronto para crescer (mais módulos, mensageria, background jobs, etc).

---

## Stack e principais decisões

- **Linguagem / Runtime**
  - .NET 8
  - C# com **construtor primário** em controllers e handlers para DI mais enxuta.
- **Persistência**
  - SQL Server
  - **Entity Framework Core** (migração do Dapper para EF Core)
- **Arquitetura**
  - DDD com camadas: **Domain**, **Application**, **Infrastructure**, **ApiService**
  - CQRS por Feature: **Command/Query + Handler + Response**
  - Padrão de composição via **`IModule`** (AddModules)
- **Infra cross-cutting**
  - MediatR para orquestrar UseCases a partir da API
  - AutoMapper para mapear entidades de domínio para DTOs de resposta
  - Result Pattern (`Result<T>`, `Error`) para modelar sucesso/erro
  - SonarQube/SonarLint para análise estática
  - Global Exception Handler + ProblemDetails (RFC 7807)
  - Swagger/Scalar com documentação em PT-BR

---

## Arquitetura da solução

Hoje a solução é composta, no backend, por:

- `OsService.Domain`
- `OsService.Application`
- `OsService.Infrastructure`
- `OsService.ApiService`
- `OsService.ServiceDefaults` (biblioteca de apoio do template Aspire)

> Os projetos de visualização (Blazor/Aspire AppHost) que vinham no template foram **removidos** para focar somente no backend do desafio.

### Camada Domain

> “The Domain layer is completely persistence-agnostic — it only contains business concepts.”

A camada **Domain** contém:

- Entidades de domínio (`CustomerEntity`, `ServiceOrderEntity`, etc.)
- Enums (`ServiceOrderStatus`)
- Tipos genéricos de suporte, como `BaseEntity` e o **Result Pattern** (`Result`, `Result<T>`, `Error`).

Ela **não conhece** nada sobre EF Core, repositórios, conexões de banco ou infraestrutura.

### Camada Application

> “The Application layer defines repository and unit-of-work interfaces as outbound ports, and the Infrastructure layer implements these ports using EF Core.”

Responsável por **casos de uso** e **regras de aplicação**, aqui vivem:

- Interfaces de repositório e Unit of Work  
  (`ICustomerRepository`, `IServiceOrderRepository`, `IUnitOfWork`, etc.)
- Casos de uso organizados por **Features/UseCases**  
  Ex.: `Customers.CreateCustomer`, `ServiceOrders.OpenServiceOrder`, etc.
- Cada UseCase segue CQRS:
  - Um `Command` ou `Query`
  - Um `Handler`
  - E, quando faz sentido, um `Response` específico

> “Na camada de Application eu organizei por Features/UseCases. Cada caso de uso segue CQRS: um Command ou Query, com seu Handler, e, quando faz sentido, um Response específico. A entidade de domínio (CustomerEntity) não é exposta diretamente — eu mapeio via AutoMapper para um modelo de saída (GetCustomerByIdResponse) que fica dentro da própria Feature. Infraestrutura implementa os repositórios e Unit of Work, e a API só conhece os UseCases via MediatR.”

**Importante:**  
A camada de **Application não referencia mais a camada de Infrastructure**.  
As interfaces vivem em Application e são implementadas em Infrastructure, mantendo o sentido de dependência correto.

### Camada Infrastructure

A camada **Infrastructure** implementa as portas definidas em Application:

- `OsServiceDbContext` com EF Core
- `EfRepository<TEntity>` (repositório genérico com métodos **virtuais**)
- Implementações concretas:
  - `CustomerRepository : EfRepository<CustomerEntity>, ICustomerRepository`
  - `ServiceOrderRepository : EfRepository<ServiceOrderEntity>, IServiceOrderRepository`
- Implementação de `IUnitOfWork`

> Sobre o `EfRepository<TEntity>`: os métodos são **virtuais** justamente para permitir que repositórios específicos sobrescrevam comportamento (ex.: includes, filtros padrão) sem quebrar o contrato base. Isso está documentado no README porque é parte do design do repositório genérico.

### Camada ApiService

É o projeto ASP.NET Core minimal hosting:

- `Program.cs` como Composition Root
- Controllers
- Registradores de serviços comuns (Swagger, ProblemDetails, ExceptionHandler)
- Conversão de `Result<T>` em `IActionResult`

A API **conhece apenas**:

- As interfaces da camada Application (via DI)
- Os `Command`/`Query` e manda tudo via MediatR

### Módulos e composição (`IModule`)

Para manter o `Program.cs` limpo e escalável, foi introduzido um padrão simples de módulos:

> “To keep the composition root clean and scalable, I introduced a simple IModule pattern. Each layer (Application, Infrastructure) exposes a module that implements IModule.ConfigureServices. In Program.cs I just call AddModules, which scans the assemblies for IModule implementations and executes their registrations. This way, when the system grows (more layers, messaging, background jobs, etc.), we only add new modules — the startup code remains small and easy to reason about.”

- `ApplicationModule` registra MediatR e AutoMapper
- `InfrastructureModule` registra DbContext, repositórios e Unit of Work
- `ApiServiceModule` (conceito) registra controllers, ProblemDetails, ExceptionHandler, OpenAPI

O `Program.cs` apenas chama:

```csharp
builder.Services.AddModules(
    builder.Configuration,
    typeof(ApplicationModule).Assembly,
    typeof(InfrastructureModule).Assembly,
    typeof(ApiServiceModule).Assembly);


