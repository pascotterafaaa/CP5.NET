using System.Text.RegularExpressions;
using JogosApi.Dtos;
using JogosApi.Models;
using JogosApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JogosApi.Repositories;

public class JogoRepository : IJogoRepository
{
    private readonly IMongoCollection<Jogo> _jogos;

    public JogoRepository(IMongoDatabase database, IOptions<MongoDbSettings> options)
    {
        _jogos = database.GetCollection<Jogo>(options.Value.JogosCollectionName);
    }

    // ---------- CRUD ----------

    public async Task<List<Jogo>> ListarAsync() =>
        await _jogos.Find(FilterDefinition<Jogo>.Empty).ToListAsync();

    public async Task<Jogo?> ObterPorIdAsync(string id) =>
        await _jogos.Find(j => j.Id == id).FirstOrDefaultAsync();

    public async Task CriarAsync(Jogo jogo) =>
        await _jogos.InsertOneAsync(jogo); // o driver preenche jogo.Id após o insert

    public async Task<bool> AtualizarAsync(string id, Jogo jogo)
    {
        jogo.Id = id;
        var resultado = await _jogos.ReplaceOneAsync(j => j.Id == id, jogo);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var resultado = await _jogos.DeleteOneAsync(j => j.Id == id);
        return resultado.DeletedCount > 0;
    }

    // ---------- Desafio 1: filtro por plataforma + preço máximo ----------

    public async Task<List<Jogo>> BuscarPorPlataformaEPrecoAsync(string plataforma, decimal precoMaximo)
    {
        var builder = Builders<Jogo>.Filter;

        // Plataforma: comparação exata, ignorando maiúsculas/minúsculas
        var filtroPlataforma = builder.Regex(
            j => j.Plataforma,
            new BsonRegularExpression($"^{Regex.Escape(plataforma.Trim())}$", "i"));

        // Preço <= precoMaximo
        var filtroPreco = builder.Lte(j => j.Preco, precoMaximo);

        var filtro = builder.And(filtroPlataforma, filtroPreco);

        return await _jogos.Find(filtro)
                           .SortBy(j => j.Preco)
                           .ToListAsync();
    }

    // ---------- Desafio 2: agregação (relatório de estoque) ----------

    public async Task<List<RelatorioEstoqueDto>> RelatorioEstoquePorPlataformaAsync()
    {
        // db.jogos.aggregate([
        //   { $group: { _id: "$plataforma",
        //               quantidadeTitulos: { $sum: 1 },
        //               valorTotalInventario: { $sum: { $multiply: ["$preco", "$estoque"] } } } },
        //   { $sort: { _id: 1 } }
        // ])
        var estagios = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$plataforma" },
                { "quantidadeTitulos", new BsonDocument("$sum", 1) },
                { "valorTotalInventario", new BsonDocument("$sum",
                    new BsonDocument("$multiply", new BsonArray { "$preco", "$estoque" })) }
            }),
            new BsonDocument("$sort", new BsonDocument("_id", 1))
        };

        var pipeline = PipelineDefinition<Jogo, BsonDocument>.Create(estagios);
        var docs = await _jogos.Aggregate(pipeline).ToListAsync();

        return docs.Select(d => new RelatorioEstoqueDto
        {
            Plataforma = d.Contains("_id") && !d["_id"].IsBsonNull ? d["_id"].AsString : string.Empty,
            QuantidadeTitulos = d.Contains("quantidadeTitulos") ? d["quantidadeTitulos"].ToInt32() : 0,
            ValorTotalInventario = d.Contains("valorTotalInventario") && !d["valorTotalInventario"].IsBsonNull
                ? d["valorTotalInventario"].ToDecimal()
                : 0m
        }).ToList();
    }
}
