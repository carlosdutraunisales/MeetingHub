using MeetingHub.Data;
using MeetingHub.Interfaces;
using MeetingHub.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeetingHub.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly IMongoCollection<Reserva>? _reservas;

    public ReservaRepository(MongoDbService mongoDbService)
    {
        _reservas = mongoDbService.Database?.GetCollection<Reserva>("Reservas");
    }

    public async Task<Reserva> CriarReservaAsync(Reserva reserva)
    {
        await _reservas.InsertOneAsync(reserva);
        return reserva;
    }

    public async Task AtualizarReservaAsync(ObjectId id, Reserva reserva)
    {
        await _reservas.ReplaceOneAsync(r => r.Id == reserva.Id, reserva);
    }

    public async Task<Reserva?> ObterReservaPorIdAsync(ObjectId id)
    {
        return await _reservas.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CancelarReservaAsync(ObjectId id)
    {
        var result = await _reservas.DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Reserva>> ObterReservasPorSalaAsync(ObjectId salaId, DateTime dataInicio, DateTime dataFim)
    {
        return await _reservas.Find(r =>
            r.SalaId == salaId &&
            ((dataInicio >= r.DataInicio && dataInicio < r.DataFim) ||
             (dataFim > r.DataInicio && dataFim <= r.DataFim))
        ).ToListAsync();
    }

    public async Task<List<Reserva>> ObterReservasPorUsuarioAsync(ObjectId usuarioId)
    {
        return await _reservas.Find(r => r.UsuarioId == usuarioId).ToListAsync();
    }

    public async Task<List<Reserva>> ObterTodasResevasAsync()
    {
        return await _reservas.Find(reserva => true).ToListAsync() ;

    }

    public async Task<Reserva> BuscarReservaPorSalaEData(ObjectId salaId, DateTime data)
    {
        var inicioDoDia = data.Date;
        var fimDoDia = data.Date.AddDays(1).AddTicks(-1); 

        return await _reservas.Find(r => r.SalaId == salaId && r.DataInicio >= inicioDoDia && r.DataInicio <= fimDoDia).FirstOrDefaultAsync();
    }
}
