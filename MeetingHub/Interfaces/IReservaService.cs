using MeetingHub.Models;
using MeetingHub.Response;
using MongoDB.Bson;

namespace MeetingHub.Interfaces;

public interface IReservaService
{
    Task<Reserva> CriarReservaAsync(Reserva reserva);
    Task<Reserva?> AtualizarReservaAsync(ObjectId id, Reserva reserva);
    Task<bool> CancelarReservaAsync(ObjectId id);
    Task<List<Reserva>> ObterReservasPorUsuarioAsync(ObjectId usuarioId);
    Task<Reserva> ObterReservaPorIdAsync(ObjectId usuarioId);
    Task<List<Reserva>> BuscarSalasDisponiveisAsync(DateTime dataInicio, DateTime dataFim, int capacidade,List<string> recursos);
    Task<List<Reserva>> ObterTodasReservas();
    Task<List<Reserva>> ObterReservasPorUserId(ObjectId userId);
    Task<ResultadoOperacao> CancelarReserva(ObjectId reservaId, ObjectId userId);

    // ResultadoOperacao UpdateReserva(string reservaId, ReservaRequest reservaRequest, string userId);
}