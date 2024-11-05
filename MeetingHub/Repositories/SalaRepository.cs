using MeetingHub.Data;
using MeetingHub.Interfaces;
using MeetingHub.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeetingHub.Repositories;

public class SalaRepository : ISalaRepository
{
    private readonly IMongoCollection<Sala>? _salasCollection;
    

    public SalaRepository(MongoDbService mongoDbService )
    {
        _salasCollection = mongoDbService.Database?.GetCollection<Sala>("salas");
    }

    public async Task<List<Sala>> ObterTodasSalasAsync() => await _salasCollection.Find(s => true).ToListAsync();

    public async Task<Sala> ObterSalaPorIdAsync(ObjectId salaId) =>
        await _salasCollection.Find(s => s.Id == salaId).FirstOrDefaultAsync();

    public async Task CriarSalaAsync(Sala sala)
    {
        var salaRet = await ObterSalaPorCodigoAsync(sala.Codigo);
        if (salaRet == null)
        {
            await _salasCollection.InsertOneAsync(sala);    
        }
    } 

    public async Task AtualizarSalaAsync(ObjectId salaId, Sala sala) =>
        await _salasCollection.ReplaceOneAsync(s => s.Id == salaId, sala);

    public async Task ExcluirSalaAsync(ObjectId salaId) =>
        await _salasCollection.DeleteOneAsync(s => s.Id == salaId);

    public async Task<Sala> ObterSalaPorCodigoAsync(int codigo)
    {
        return await _salasCollection.Find(s => s.Codigo == codigo).FirstOrDefaultAsync();
    }

    public async Task<List<Sala>> BuscarSalasAtivas(int? capacidade, List<string> recursos)
    {
        var filtro = Builders<Sala>.Filter.Eq(s => s.Ativa, "A");

        if (capacidade.HasValue)
        {
            filtro &= Builders<Sala>.Filter.Gte(s => s.Capacidade, capacidade.Value);
        }

        if (recursos != null && recursos.Any())
        {
            foreach (var recurso in recursos)
            {
                filtro &= Builders<Sala>.Filter.AnyEq(s => s.Recursos, recurso);
            }
        }

        return await _salasCollection.Find(filtro).ToListAsync();
    }

   
}