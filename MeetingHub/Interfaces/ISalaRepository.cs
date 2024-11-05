using MeetingHub.Models;
using MongoDB.Bson;

namespace MeetingHub.Interfaces;

public interface ISalaRepository
{
    Task<List<Sala>> ObterTodasSalasAsync();
    Task<Sala> ObterSalaPorIdAsync(ObjectId salaId);
    Task CriarSalaAsync(Sala sala);
    Task AtualizarSalaAsync(ObjectId salaId, Sala sala);
    Task ExcluirSalaAsync(ObjectId salaId);
    
    Task<Sala> ObterSalaPorCodigoAsync(int codigo);
}