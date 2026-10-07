# Catálogo de Jogos de Videogame — Web API .NET 8 + MongoDB (CP5)

API REST desenvolvida em **.NET 8** integrada ao **MongoDB** utilizando o driver oficial `MongoDB.Driver`. Esta aplicação implementa um catálogo completo de jogos com operações de **CRUD**, injeção de dependência via **Options Pattern**, separação de responsabilidades em camadas e dois **desafios práticos de consultas avançadas e pipeline de agregação**.

### Integrantes
Rafael de Freitas Moraes - RM563210 | 
Rafael Pascotte Mercadante - RM564928

---

## Sumário
- [Requisitos e Tecnologias](#-requisitos-e-tecnologias)
- [How-To: Como Executar o Projeto do Zero](#-how-to-como-executar-o-projeto-do-zero)
  - [Passo 1: Subir o MongoDB](#passo-1-subir-o-mongodb-via-docker)
  - [Passo 2: Restaurar e Executar a API](#passo-2-restaurar-e-executar-a-api)
  - [Passo 3: Acessar a Documentação Interativa](#passo-3-acessar-a-documenta%C3%A7%C3%A3o-interativa)
- [Guia de Uso dos Endpoints (How-To Use)](#-guia-de-uso-dos-endpoints-how-to-use)
  - [1. Cadastrar Jogo (`POST /api/jogos`)](#1-cadastrar-jogo-post-apijogos)
  - [2. Listar Todos os Jogos (`GET /api/jogos`)](#2-listar-todos-os-jogos-get-apijogos)
  - [3. Buscar Jogo por ID (`GET /api/jogos/{id}`)](#3-buscar-jogo-por-id-get-apijogosid)
  - [4. Atualizar Jogo (`PUT /api/jogos/{id}`)](#4-atualizar-jogo-put-apijogosid)
  - [5. Remover Jogo (`DELETE /api/jogos/{id}`)](#5-remover-jogo-delete-apijogosid)
  - [6. Desafio 1: Busca por Plataforma e Preço Máximo (`GET /api/jogos/busca`)](#6-desafio-1-busca-por-plataforma-e-pre%C3%A7o-m%C3%A1ximo-get-apijogosbusca)
  - [7. Desafio 2: Relatório de Estoque e Agregação (`GET /api/jogos/relatorio-estoque`)](#7-desafio-2-relat%C3%B3rio-de-estoque-e-agrega%C3%A7%C3%A3o-get-apijogosrelatorio-estoque)
- [Formas de Testar a API](#-formas-de-testar-a-api)
- [Estrutura da Arquitetura](#-estrutura-da-arquitetura)
- [Códigos de Status HTTP](#-c%C3%B3digos-de-status-http)

---

## Requisitos e Tecnologias

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** ou superior
- **[Docker](https://www.docker.com/)** (para rodar o container do MongoDB) ou uma instância local do MongoDB na porta `27017`
- **MongoDB.Driver (v3.x)** — Driver oficial do MongoDB para .NET
- **Swashbuckle (Swagger UI)** e **Scalar.AspNetCore (Scalar UI)** para exploração e documentação dos endpoints

---

## How-To: Como Executar o Projeto do Zero

### Passo 1: Subir o MongoDB via Docker

Você pode iniciar uma instância limpa do MongoDB usando o comando direto do Docker ou o Docker Compose:

#### Opção A — Comando Docker direto:
```bash
docker run -d -p 27017:27017 --name mongo-local mongo
```

#### Opção B — Docker Compose:
No diretório raiz do projeto, execute:
```bash
docker compose up -d
```

> **Verificação:** Para conferir se o container está em execução:
> ```bash
> docker ps
> ```

---

### Passo 2: Restaurar e Executar a API

Abra o terminal na pasta raiz do projeto (`JogosApi`) e execute:

```bash
# Restaurar pacotes NuGet
dotnet restore

# Compilar e rodar a aplicação
dotnet run
```

A aplicação será iniciada e estará escutando no endereço padrão:
```
http://localhost:5000
```

---

### Passo 3: Acessar a Documentação Interativa

Com a API em execução, abra seu navegador em qualquer uma das interfaces disponíveis:

- **Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **Scalar UI:** [http://localhost:5000/scalar/v1](http://localhost:5000/scalar/v1)

---

## Guia de Uso dos Endpoints (How-To Use)

### 1. Cadastrar Jogo (`POST /api/jogos`)
Cria um novo jogo no catálogo. O ID é gerado automaticamente pelo MongoDB e devolvido no cabeçalho `Location` e no corpo da resposta.

- **URL:** `POST http://localhost:5000/api/jogos`
- **Corpo da Requisição (JSON):**
```json
{
  "titulo": "God of War",
  "plataforma": "PlayStation 3",
  "genero": "Ação",
  "preco": 79.90,
  "anoLancamento": 2010,
  "estoque": 15
}
```

- **Exemplo com cURL:**
```bash
curl -X POST http://localhost:5000/api/jogos \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "God of War",
    "plataforma": "PlayStation 3",
    "genero": "Ação",
    "preco": 79.90,
    "anoLancamento": 2010,
    "estoque": 15
  }'
```

- **Retorno Esperado (`201 Created`):**
```json
{
  "id": "6ac58e4bca67d2c80ab99e83",
  "titulo": "God of War",
  "plataforma": "PlayStation 3",
  "genero": "Ação",
  "preco": 79.9,
  "anoLancamento": 2010,
  "estoque": 15
}
```
*Header retornado:* `Location: http://localhost:5000/api/jogos/6ac58e4bca67d2c80ab99e83`

---

### 2. Listar Todos os Jogos (`GET /api/jogos`)
Retorna a listagem completa dos jogos cadastrados.

- **URL:** `GET http://localhost:5000/api/jogos`
- **Exemplo com cURL:**
```bash
curl -X GET http://localhost:5000/api/jogos
```
- **Retorno Esperado (`200 OK`):**
```json
[
  {
    "id": "6ac58e4bca67d2c80ab99e83",
    "titulo": "God of War",
    "plataforma": "PlayStation 3",
    "genero": "Ação",
    "preco": 79.9,
    "anoLancamento": 2010,
    "estoque": 15
  }
]
```

---

### 3. Buscar Jogo por ID (`GET /api/jogos/{id}`)
Busca um jogo específico pelo seu identificador hexadecimal (ObjectId).

- **URL:** `GET http://localhost:5000/api/jogos/{id}`
- **Exemplo com cURL:**
```bash
curl -X GET http://localhost:5000/api/jogos/6ac58e4bca67d2c80ab99e83
```
- **Retorno (`200 OK`):** Objeto do jogo correspondente.
- **Se não existir (`404 Not Found`):** Quando o ID é válido mas não consta na base.
- **Se o ID for malformado (`400 Bad Request`):** Ex: `GET /api/jogos/id-invalido`.

---

### 4. Atualizar Jogo (`PUT /api/jogos/{id}`)
Atualiza todas as informações de um jogo existente.

- **URL:** `PUT http://localhost:5000/api/jogos/{id}`
- **Corpo da Requisição (JSON):**
```json
{
  "titulo": "God of War HD Remaster",
  "plataforma": "PlayStation 3",
  "genero": "Ação",
  "preco": 89.90,
  "anoLancamento": 2010,
  "estoque": 20
}
```

- **Exemplo com cURL:**
```bash
curl -X PUT http://localhost:5000/api/jogos/6ac58e4bca67d2c80ab99e83 \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "God of War HD Remaster",
    "plataforma": "PlayStation 3",
    "genero": "Ação",
    "preco": 89.90,
    "anoLancamento": 2010,
    "estoque": 20
  }'
```
- **Retorno Esperado (`200 OK`):** Dados atualizados do jogo.
- **Se não existir (`404 Not Found`):** ID inexistente.
- **Se ID malformado ou payload inválido (`400 Bad Request`)**.

---

### 5. Remover Jogo (`DELETE /api/jogos/{id}`)
Exclui um jogo do banco de dados pelo seu ID.

- **URL:** `DELETE http://localhost:5000/api/jogos/{id}`
- **Exemplo com cURL:**
```bash
curl -X DELETE http://localhost:5000/api/jogos/6ac58e4bca67d2c80ab99e83
```
- **Retorno Esperado (`204 No Content`):** Jogo removido com sucesso (sem corpo de resposta).
- **Se não existir (`404 Not Found`)**.
- **Se ID malformado (`400 Bad Request`)**.

---

### 6. Desafio 1: Busca por Plataforma e Preço Máximo (`GET /api/jogos/busca`)
Filtra jogos pertencentes a uma determinada plataforma cujo preço seja menor ou igual (`<=`) ao preço máximo fornecido.
- Implementado no repositório com `Builders<Jogo>.Filter.And()`.
- A comparação da plataforma é **case-insensitive** (ignora maiúsculas e minúsculas).
- Os resultados são retornados ordenados pelo preço em ordem crescente.

- **URL:** `GET http://localhost:5000/api/jogos/busca?plataforma={plataforma}&precoMaximo={preco}`
- **Exemplo com cURL:**
```bash
curl -X GET "http://localhost:5000/api/jogos/busca?plataforma=PlayStation%203&precoMaximo=100"
```
- **Retorno Esperado (`200 OK`):**
```json
[
  {
    "id": "6ac58e4bca67d2c80ab99e83",
    "titulo": "God of War HD Remaster",
    "plataforma": "PlayStation 3",
    "genero": "Ação",
    "preco": 89.9,
    "anoLancamento": 2010,
    "estoque": 20
  }
]
```

---

### 7. Desafio 2: Relatório de Estoque e Agregação (`GET /api/jogos/relatorio-estoque`)
Gera um relatório estatístico sumarizando os dados agrupados por **Plataforma**.
- Implementado utilizando a pipeline de agregação oficial do MongoDB (`$group`, `$multiply`, `$sum`, `$sort`).
- Retorna:
  1. **Plataforma**: Nome da plataforma.
  2. **QuantidadeTitulos**: Quantidade total de títulos distintos cadastrados na plataforma.
  3. **ValorTotalInventario**: Valor financeiro total em estoque, calculando a multiplicação de cada **Preço × Estoque** e somando por plataforma.

- **URL:** `GET http://localhost:5000/api/jogos/relatorio-estoque`
- **Exemplo com cURL:**
```bash
curl -X GET http://localhost:5000/api/jogos/relatorio-estoque
```
- **Retorno Esperado (`200 OK`):**
```json
[
  {
    "plataforma": "Nintendo DS",
    "quantidadeTitulos": 1,
    "valorTotalInventario": 1500.0,
    "quantidadeTotalTitulos": 1,
    "valorTotalEstoque": 1500.0
  },
  {
    "plataforma": "PlayStation 3",
    "quantidadeTitulos": 2,
    "valorTotalInventario": 1318.5,
    "quantidadeTotalTitulos": 2,
    "valorTotalEstoque": 1318.5
  },
  {
    "plataforma": "Xbox 360",
    "quantidadeTitulos": 1,
    "valorTotalInventario": 340.0,
    "quantidadeTotalTitulos": 1,
    "valorTotalEstoque": 340.0
  }
]
```

---

## Formas de Testar a API

### 1. Pelo Arquivo `requests.http`
No VS Code (com a extensão *REST Client*) ou no JetBrains Rider, você pode abrir diretamente o arquivo [`requests.http`](requests.http). Ele contém um fluxo ordenado pronto para executar cada requisição com um clique, capturando automaticamente o `id` gerado no `POST` para usar nos testes subsequentes.

### 2. Pelo Swagger UI
Acesse `http://localhost:5000/swagger` para visualizar todos os schemas, modelos e botões interativos *Try it out*.

### 3. Pelo Scalar UI
Acesse `http://localhost:5000/scalar/v1` para testar os endpoints através de uma interface moderna e intuitiva com geração de exemplos de código em múltiplas linguagens.

---

## Estrutura da Arquitetura

O projeto aplica estritamente a separação em camadas e o desacoplamento por injeção de dependência:

```
JogosApi/
├── Controllers/
│   └── JogosController.cs        # Endpoints HTTP, validações e respostas REST
├── Services/
│   ├── IJogoService.cs           # Contrato da camada de serviço
│   └── JogoService.cs            # Regras de negócio e mapeamento DTO ↔ Entidade
├── Repositories/
│   ├── IJogoRepository.cs        # Contrato de persistência
│   └── JogoRepository.cs         # Implementação direta com MongoDB.Driver
├── Models/
│   └── Jogo.cs                   # Documento MongoDB com decorators BSON
├── Dtos/
│   ├── JogoRequest.cs            # Payload de entrada para POST e PUT com DataAnnotations
│   └── RelatorioEstoqueDto.cs    # Objeto de transferência da agregação de estoque
├── Settings/
│   └── MongoDbSettings.cs        # Classe de configuração tipada (Options Pattern)
├── Properties/
│   └── launchSettings.json       # Configuração de portas e ambiente Kestrel
├── appsettings.json              # String de conexão e nome da base MongoDB
├── docker-compose.yml            # Orquestração do container do MongoDB
├── requests.http                 # Coleção de testes HTTP prontos
└── Program.cs                    # Configuração de serviços, DI, Swagger e Scalar
```

### Principais Padrões Utilizados:
1. **Options Pattern (`IOptions<MongoDbSettings>`)**:
   As configurações do MongoDB (`ConnectionString`, `DatabaseName`, `JogosCollectionName`) são mapeadas a partir de `appsettings.json` e injetadas tipadas nas classes necessárias.
2. **Ciclo de Vida de Serviços (DI)**:
   - `IMongoClient` e `IMongoDatabase`: Registrados como **Singletons** (thread-safe, reaproveitando conexões).
   - `IJogoRepository` e `IJogoService`: Registrados como **Scoped** (respeitando o ciclo de vida de cada requisição HTTP).
3. **Mapeamento BSON**:
   - `[BsonId]` e `[BsonRepresentation(BsonType.ObjectId)]` no campo `Id`.
   - `[BsonRepresentation(BsonType.Decimal128)]` no campo `Preco`.
   - `[BsonElement("...")]` em todas as propriedades para consistência de schema.

---

## Códigos de Status HTTP

| Código | Nome | Ocorrência na API |
|---|---|---|
| **`200 OK`** | Sucesso | Retorno de listagem, busca por ID, consulta com filtros, agregação e atualização (`PUT`). |
| **`201 Created`** | Criado | Retorno de criação com sucesso (`POST`), incluindo o cabeçalho `Location`. |
| **`204 No Content`** | Sem Conteúdo | Retorno de remoção com sucesso (`DELETE`). |
| **`400 Bad Request`** | Requisição Inválida | ID com formato hexadecimal inválido, parâmetros de busca ausentes ou payload em desacordo com as regras de validação. |
| **`404 Not Found`** | Não Encontrado | Recurso não localizado para o ID informado em `GET`, `PUT` ou `DELETE`. |

