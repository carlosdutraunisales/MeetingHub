using MeetingHub.Interfaces;
using MeetingHub.Models;
using MeetingHub.Response;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeetingHub.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly SalaService _salaService;

    public ReservaService(IReservaRepository reservaRepository, SalaService salaService)
    {
        _reservaRepository = reservaRepository;
        _salaService = salaService;
    }

    public async Task<Reserva> CriarReservaAsync(Reserva reserva)
    {
        var sala = await _salaService.ObterSalaPorIdAsync(reserva.SalaId);
        if (sala == null || sala.Ativa == "I")
        {
            throw new Exception("A sala está desativada ou não existe.");
        }

        var conflitos = await _reservaRepository.ObterReservasPorSalaAsync(reserva.SalaId, reserva.DataInicio, reserva.DataFim);
        if (conflitos.Any())
        {
            throw new Exception("Conflito de horário: já existe uma reserva para este período.");
        }

        return await _reservaRepository.CriarReservaAsync(reserva);
    }

    public async Task<Reserva?> AtualizarReservaAsync(ObjectId id, Reserva reserva)
    {
        var reservaExistente = await _reservaRepository.ObterReservaPorIdAsync(id);
        if (reservaExistente == null) return null;

        var conflitos = await _reservaRepository.ObterReservasPorSalaAsync(reserva.SalaId, reserva.DataInicio, reserva.DataFim);
        if (conflitos.Any(r => r.Id != id))
        {
            throw new Exception("Conflito de horário: já existe uma reserva para este período.");
        }

        reserva.Id = id;
        await _reservaRepository.AtualizarReservaAsync(id, reserva);
        return reserva;
    }

    public async Task<bool> CancelarReservaAsync(ObjectId id)
    {
        return await _reservaRepository.CancelarReservaAsync(id);
    }

    public async Task<List<Reserva>> ObterReservasPorUsuarioAsync(ObjectId usuarioId)
    {
        return await _reservaRepository.ObterReservasPorUsuarioAsync(usuarioId);
    }

    public async Task<Reserva> ObterReservaPorIdAsync(ObjectId usuarioId)
    {
        return await _reservaRepository.ObterReservaPorIdAsync(usuarioId);
    }

    public async Task<List<Reserva>> BuscarSalasDisponiveisAsync(DateTime dataInicio, DateTime dataFim, int capacidade, List<string> recursos)
    {
        // Implementação de filtro com base no `_salaService` para buscar salas que atendam aos requisitos.
        // ...
        throw new NotImplementedException();
    }

    public async Task<List<Reserva>> ObterTodasReservas()
    {
        var x = await _reservaRepository.ObterTodasResevasAsync();
        
        return x;
    }

   

    public async  Task<List<Reserva>> ObterReservasPorUserId(ObjectId userId)
    {
        return await _reservaRepository.ObterReservasPorUsuarioAsync(userId);
    }

 
    public async Task<ResultadoOperacao> CancelarReserva(ObjectId reservaId, ObjectId userId)
    {
        var reserva = await _reservaRepository.ObterReservaPorIdAsync(reservaId);
        if (reserva == null) return new ResultadoOperacao(false, "Reserva não encontrada.");

        if (reserva.UsuarioId != userId)
        {
            return new ResultadoOperacao(false, "Você não tem permissão para cancelar esta reserva.");
        }

        await _reservaRepository.CancelarReservaAsync(reservaId);
        return new ResultadoOperacao(true);
    }
}
