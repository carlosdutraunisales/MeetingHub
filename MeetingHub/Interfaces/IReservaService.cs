using MeetingHub.Models;
using MeetingHub.Response;
using MongoDB.Bson;

namespace MeetingHub.Interfaces;

public interface IReservaService
{
    Task<Reserva> CriarReservaAsync(Reserva reserva);
    Task<ResultadoOperacao> AtualizarReservaAsync(ObjectId id, ObjectId userId, Reserva reserva);
    Task<List<Reserva>> ObterReservasPorUsuarioAsync(ObjectId usuarioId);
    Task<Reserva> ObterReservaPorIdAsync(ObjectId usuarioId);
    Task<List<Reserva>> ObterTodasReservas();
    Task<List<Reserva>> ObterReservasPorUserId(ObjectId userId);
    Task<ResultadoOperacao> CancelarReservaAsync (ObjectId reservaId, ObjectId userId);
    
}