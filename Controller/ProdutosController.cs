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