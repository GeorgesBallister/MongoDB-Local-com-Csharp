<aside>
💡 Desculpe se algo estiver faltando é que eu estou em semana de prova na outra faculdade!

</aside>

# Criando o Projeto

Passo 1: instalar o mongodb community server

Passo 2: Instalar o Mongosh

Passo 3: Adicionar o diretorio do Mongod, Mongos e Mongosh as variáveis do sistema

Passo 4: inicializar o projeto em c#:

```powershell
dotnet new webapi
```

Passo 5: Adicionar drives do mongo:

```powershell
dotnet add package MongoDB.Driver
```

Projeto Criado!

# Construindo o projeto

## Passo 1: criando a pasta Models para comportar os modelos de dados que o mongo vai receber:

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/69aeae5d-8a11-42f9-a99a-d2dc9550e748/727dd915-7556-4d9f-860e-34d4e769f52a/Untitled.png)

## Passo 2: Dentro do Arquivo iremos montar nosso modelo!

```csharp
namespace APIMongoDB.Models; // Criamos um namespace para manter o codigo limpo

public class ProdutosModel{

    public string? Id { get; set; } // Colocamos o ? depois do tipo do dado para dizeer que aquele campo aceita dados nulos
    public string? Name { get; set;} = null; // Lembrando que temos que iniciar o campo como null
}
```

Logo após acionaremos o `[BsonElement(””)]`, ele vai nos permitir vincular aquele atributo a um campo dentro do banco de dados, como por exemplo neste projeto o campo esta como `Name` mas la no banco de dados pode esta `_NomePrincipal`, o objetivo é que tudo esteja igual porem caso seja necessário existe existe essa forma do qual o programador poderá escolher qual o nome do atributo dentro do código para vincula-lo a um determinado campo dentro do MongoDB.

```csharp
using MongoDB.Bson.Serialization.Attributes; // assim que adcionarmos [BsonElement("")] na maioria dos casos esse namespace sera adcionado imediatamente, mas caso isso não aconteça, certifiquese de adcionalo!

namespace APIMongoDB.Models;

public class ProdutosModel{

    public string? Id { get; set; }

    [BsonElement("Name")] // Devemos colocar aqui o campo no banco
    public string? Name { get; set;}
}
```

## Passo3: Definindo nossa Chave Primaria e Configurações

Agora vamos adcionar 2 configurações a nosso modelo:

- `[BsonId]` = Define que esse campo sera a "Chave Primaria"
- `[BsonRepresentation(BsonType.ObjectId)]` = Esta configuração definirá que o campo se comportará como um `ObjectID` Definindo assim Que o banco gerará um ID com uma chave única.

<aside>
💡 Confira melhor os detalhes na documentação original do mongo:

[ObjectId()](https://www.mongodb.com/docs/manual/reference/method/ObjectId/#objectid--)

</aside>

```csharp
using MongoDB.Bson; // Esse namespace tambem sera utilizado assim que voce adcionar as propriedades da chave
using MongoDB.Bson.Serialization.Attributes;

namespace APIMongoDB.Models;

public class ProdutosModel{

[BsonId] // Define que esse campo sera a "Chave Primaria"
[BsonRepresentation(BsonType.ObjectId)] // Este campo sera definido como a representação do ObjectID
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string? Name { get; set;} = null;
}
```

Logo após devemos criar um novo arquivo chamado `ProdutoDatasetSettings`, do qual vai conter as configurações do Banco dentro do Programa, por questão de organização criarei-o dentro da pasta models:

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/69aeae5d-8a11-42f9-a99a-d2dc9550e748/ee107633-45c2-49b9-9b18-69f11ac65d6b/Untitled.png)

Adicionando o código dentro

```csharp
namespace APIMongoDB.Models;
public class ProdutoDatasetSettings
{

    public string ConnectionString { get; set; } = null;
    public string DatabaseName { get; set; } = null;
    public string ProdutoCollectionName { get; set; } = null;

}
```

- `ConnectionString` = URI do Mongo
- `DatabaseName` = Vai o nome do banco do Mongo
- `ProdutoCollectionName` = Vai o nome da collection dentro do banco

## Passo 4: Criar os Services.

Bem nessa etapa criaremos uma pasta dentro do projeto chamada de services, onde todos os serviços do programa esta desenvolvido.

dentro da pasta criaremos o arquivo `ProdutoServices.cs`.

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/69aeae5d-8a11-42f9-a99a-d2dc9550e748/df0f5a68-3974-4b7e-b9c1-059e43e5da6c/Untitled.png)

Dentro desse arquivo desenvolveremos o seguinte código:

```csharp
using APIMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace APIMongoDB.Services; // Lembrar sempre de adcionar as classes a seus respectivos namespaces para traser sempre uma melhor legibilidade e organização ao codigopublic class ProdutoServices
public class ProdutoServices
{
    private readonly IMongoCollection<ProdutosModel> _produtoCollection; 
    /* Criaremos uma Collection no nosso banco com base no modelo do nosso 
    produto (Utilizase readonly para que o valor se mantenha imutavel depois
    que o construtor encerrar)*/
    
    public ProdutoServices(IOptions<ProdutoDatasetSettings> produtoServices)
    {
        
    }

}
```

Dentro desse código criaremos nossa Collection com a interface que o mongo nos disponibiliza e o sobrecarregaremos com a base no model que fizemos para o produto:

- `IMongoCollection<>`

[Palavra-chave readonly – Referência de C# - C#](https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/keywords/readonly)

[IMongoCollection(TDocument) Interface](https://mongodb.github.io/mongo-csharp-driver/2.8/apidocs/html/T_MongoDB_Driver_IMongoCollection_1.htm)

Criaremos um construtor do qual sera assinado com as configurações presentes no `ProdutoDatasetSettings` que vimos anteriormente através da interface `IOptions<>` do qual oferece uma maneira flexível de injetar opções de configuração em serviços do ASP.NET Core, permitindo que você configure o comportamento do aplicativo em tempo de execução sem a necessidade de recompilação, armazenando isso na variável `produtoServices`

- `IOptions<>`

[Padrão de opções no ASP.NET Core](https://learn.microsoft.com/pt-br/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0)

Dado isso dentro do construtor vamos defini-lo com os seguintes campos:

```csharp
using APIMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace APIMongoDB.Services;
public class ProdutoServices
{
    private readonly IMongoCollection<ProdutosModel> _produtoCollection; 
    /* Criaremos uma Collection no nosso banco com base no modelo do nosso 
    produto (Utilizase readonly para que o valor se mantenha imutavel depois
    que o construtor encerrar)*/
    
    public ProdutoServices(IOptions<ProdutoDatasetSettings> produtoServices)
    {
        var mongoClient = new MongoClient(produtoServices.Value.ConnectionString); // Sera o responsavel por capturar o a Conection String (URI) do objeto ProdutoDatasetSettings
        var mongoDatabase = mongoClient.GetDatabase(produtoServices.Value.DatabaseName); // Sera responsavel por capturar qual o nome da Database que vamos utilizar
        _produtoCollection = mongoDatabase.GetCollection<ProdutosModel>
            (produtoServices.Value.ProdutoCollectionName); // Sera responsavel por capturar qual o nome da Collection que vamos utilziar
    }

}
```

Vamos entender os campos definidos dentro do construtor

1. `var mongoClient = new MongoClient(produtoServices.Value.ConnectionString);` = Primeiramente criaremos uma variável chamada de `mongoClient` para ser o receptáculo da objeto `MongoClient().`
    - `var mongoClient = new MongoClient`
    
    Do qual vai receber como sobrecarga o valor da `ConnectionString` presente dentro da variável `produtoServices`.
    
    - `(produtoServices.Value.ConnectionString);`
2. `var mongoDatabase = mongoClient.GetDatabase(produtoServices.Value.DatabaseName);` = Logo após uma nova variável chamada de `mongoDatabase` servira de receptáculo para o resultado do método `GetDatabase`
    - `var mongoDatabase = mongoClient.GetDatabase`
    
    Que recebera o valor presente no `DatabaseName` armazenado no `produtoServies`
    
    - `(produtoServices.Value.DatabaseName);`
3. `_produtoCollection = mongoDatabase.GetCollection<ProdutosModel>`
`(produtoServices.Value.ProdutoCollectionName);` = Por fim o `_produtoCollection` recebera o nome da coleção do banco de dado através da do método `GetCollection<ProdutosModel>` que é utilizado para obter uma referencia a uma coleção no caso o `ProdutosModel.`
    - `_produtoCollection = mongoDatabase.GetCollection<ProdutosModel>`
    
    Do qual vai receber o valor através do `ProdutoCollectionName`
    
    - `(produtoServices.Value.ProdutoCollectionName);`

## Passo 5: Adicionando configurações ao appsettings.json

Nessa etapa devemos procurar o arquivo  `appsettings.json` dentro nosso projeto para poder acionar algumas linhas de configuração:

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/69aeae5d-8a11-42f9-a99a-d2dc9550e748/879feaaa-6edd-46a6-a7d8-ee3259d7a42c/Untitled.png)

Dai então acionamos as seguintes linhas ( acima de tudo antes do primeiro campo depois da primeira chave `{` ):

```json
"DevNetStoreDatabase": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName" : "DatabaseTeste",
    "ProdutoCollectionName" : "CollectionTeste"
  
  },
```

Deixando o arquivo com algo parecido com isso:

```json
{
  "DevNetStoreDatabase": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName" : "DatabaseTeste",
    "ProdutoCollectionName" : "CollectionTeste"
  
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```

## Vamos entender esse código:

Primeiramente temos que ter consciência de que isso é um arquivo json, por se tratar de um arquivo json ele servira par algum momento lermos, interpretarmos os campos e obtermos os valores presentes nos campos. Isso nos leva a entender que nesse objeto json tem:

1. O Objeto a ser tratado:  `"DevNetStoreDatabase": {...}`
2. O campo 1:  `"ConnectionString":`
    1. O valor do campo 1: `"mongodb://localhost:27017"`
3. O campo 2: `"DatabaseName"`
    1. O Valor do campo 2:  `"DatabaseTeste"`
4. O campo 3: `"ProdutoCollectionName"` 
    1. O Valor do campo 3: `"CollectionTeste"`

## Vamos interpretar esse código pelos campos e seus valores:

## Vamos ver onde esses valores vão:

Agora sim todas as configurações estão criadas, é um processo meio chatinho de pegar no inicio, mas depois que tudo esta configurado agora nos basta apenas produzir os métodos async (assíncronos) que vão compor o nosso CRUD.

# Criando Métodos

Na criação de métodos sera onde a nossa API vai se desenvolver e manipular o banco e os dados nele contidos:

Vamos começar com um método que puxara todos os dados do banco, lembrando que ainda estamos dentro do arquivo `ProdutoServices` do lado de fora e embaixo do `metodo` construtor da classe!

```csharp
// Bibliotecas e namespaces para ser usados
using APIMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace APIMongoDB.Services;
public class ProdutoServices
{
    private readonly IMongoCollection<ProdutosModel> _produtoCollection; 
    /* Criaremos uma Collection no nosso banco com base no modelo do nosso 
    produto (Utilizase readonly para que o valor se mantenha imutavel depois
    que o construtor encerrar)*/
    
    // Construtor
    public ProdutoServices(IOptions<ProdutoDatasetSettings> produtoServices)
    {
        var mongoClient = new MongoClient(produtoServices.Value.ConnectionString); // Sera o responsavel por capturar o a Conection String (URI) do objeto ProdutoDatasetSettings
        var mongoDatabase = mongoClient.GetDatabase(produtoServices.Value.DatabaseName); // Sera responsavel por capturar qual o nome da Database que vamos utilizar
        _produtoCollection = mongoDatabase.GetCollection<ProdutosModel>
            (produtoServices.Value.ProdutoCollectionName); // Sera responsavel por capturar qual o nome da Collection que vamos utilziar
    }

/*
NOSSOS METODOS VAO FICAR AQUI!!!!
*/

}
```

## Create

```csharp
// Create One (C-r-u-d)
    public async Task CreateAsync(ProdutosModel produto) =>
    await _produtoCollection.InsertOneAsync(produto);
```

## Read all e Read One

```csharp
// Read ALL (c-R-u-d)
    public async Task<List<ProdutosModel>> GetAsync() =>
   await _produtoCollection.Find(x => true).ToListAsync();

// Read One (c-R-u-d)
    public async Task<ProdutosModel> GetAsync(string id) =>
        await _produtoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

```

## Update

```csharp
// Update (c-r-U-d)

    public async Task UpdateAsync(string id, ProdutosModel produto) =>
        await _produtoCollection.ReplaceOneAsync(x => x.Id == id, produto);
```

## Delete

```csharp
// Delete (c-r-u-D)
    public async Task RemoveAsync(string id) =>
    await _produtoCollection.DeleteOneAsync(x => x.Id == id);
```

# Criando a API

Passo 1: Configurando o Program.cs

Antes de tudo devemos excluir tudo relacionado a o WeatherForecast que vem como padão!

```csharp
using APIMongoDB.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```

Depois adcionaremos as configurações do builder:

```csharp
using APIMongoDB.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ProdutoDatasetSettings>
    (builder.Configuration.GetSection("DevNetStoreDatabase"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```

Passo 2: Criar o controller e a pasta:

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/69aeae5d-8a11-42f9-a99a-d2dc9550e748/3cfce279-deda-4a11-8689-4871e69f9012/Untitled.png)

e Adicionar o seguinte código dentro do arquivo:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace APIMongoDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{

}
```

Agora utilizaremos o `ProdutoServices` que criamos, adcionando a seguinte linha e criando o método contstutor:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APIMongoDB.Services;

namespace APIMongoDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoServices _produtoServices; // Adcionamos os Services

    public ProdutosController(ProdutoServices produtoServices) // Criamos o construtor
    {
        _produtoServices = produtoServices; // Adcionamos os valores do service dentro do contrutor
    }
}
```

Passo 3: adcionar a api

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APIMongoDB.Services;
using APIMongoDB.Models;

namespace APIMongoDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoServices _produtoServices;

    public ProdutosController(ProdutoServices produtoServices)
    {
        _produtoServices = produtoServices;
    }

    // Listar tudo 

    [HttpGet]
    public async Task<List<ProdutosModel>> GetProdutos()
       => await _produtoServices.GetAsync();

    // Listar Tudo

    [HttpGet("GetById")]
    public async Task<ProdutosModel> GetProdutoById(string id)
    => await _produtoServices.GetAsync(id);

    // Criar
    [HttpPost]
    public async Task<ProdutosModel> PostProdutos(ProdutosModel produto)
    {
        await _produtoServices.CreateAsync(produto);
        return produto;
    }

    // Atualizar
    [HttpPut]
    public async Task<ProdutosModel> Update(string id, ProdutosModel produto)
    {
        await _produtoServices.UpdateAsync(id, produto);
        return produto;
    }

    //Deletar
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id){
        var produto = await _produtoServices.GetAsync(id);
        if (produto == null){
            return NotFound();
        }
        await _produtoServices.RemoveAsync(id);
        return Ok(produto);
    }

}
```

Adcionar o Singleton no Progrm.cs

```csharp
// Add services to the container.
builder.Services.Configure<ProdutoDatasetSettings>
    (builder.Configuration.GetSection("DevNetStoreDatabase"));

builder.Services.AddSingleton<ProdutoServices>();

```
