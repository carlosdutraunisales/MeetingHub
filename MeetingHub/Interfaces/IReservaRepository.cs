using MeetingHub.Models;
using MongoDB.Bson;

namespace MeetingHub.Interfaces;

public interface IReservaRepository
{
    Task<Reserva> CriarReservaAsync(Reserva reserva);
    Task<Reserva?> ObterReservaPorIdAsync(ObjectId id);
    Task AtualizarReservaAsync(ObjectId id, Reserva reserva);
    Task<bool> CancelarReservaAsync(ObjectId id);
    Task<List<Reserva>> ObterReservasPorUsuarioAsync(ObjectId usuarioId);
    Task<List<Reserva>> ObterReservasPorSalaAsync(ObjectId salaId, DateTime dataInicio, DateTime dataFim);
    Task<List<Reserva>> ObterTodasResevasAsync();
    
    
}