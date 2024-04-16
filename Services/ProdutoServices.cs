using APIMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace APIMongoDB.Services;

#region Construtordos serviços
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
#endregion


#region Metodos

// Create One (C-r-u-d)
    public async Task CreateAsync(ProdutosModel produto) =>
    await _produtoCollection.InsertOneAsync(produto);

// Read ALL (c-R-u-d)
    public async Task<List<ProdutosModel>> GetAsync() =>
   await _produtoCollection.Find(x => true).ToListAsync();

// Read One (c-R-u-d)
    public async Task<ProdutosModel> GetAsync(string id) =>
        await _produtoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


// Update (c-r-U-d)
    public async Task UpdateAsync(string id, ProdutosModel produto) =>
        await _produtoCollection.ReplaceOneAsync(x => x.Id == id, produto);

// Delete (c-r-u-D)
    public async Task RemoveAsync(string id) =>
    await _produtoCollection.DeleteOneAsync(x => x.Id == id);

#endregion
}