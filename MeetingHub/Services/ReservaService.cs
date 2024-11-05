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

    public async Task<ResultadoOperacao> AtualizarReservaAsync(ObjectId id, ObjectId userId, Reserva reserva)
    {
        var reservaExistente = await _reservaRepository.ObterReservaPorIdAsync(id);
        if (reservaExistente == null) return new ResultadoOperacao(false, "Reserva não encontrada.");

        if (reservaExistente.UsuarioId != userId)
        {
            return new ResultadoOperacao(false, "Você não tem permissão para cancelar esta reserva.");
        }

        var conflitos = await _reservaRepository.ObterReservasPorSalaAsync(reserva.SalaId, reserva.DataInicio, reserva.DataFim);
        if (conflitos.Any(r => r.Id != id))
        {
            throw new Exception("Conflito de horário: já existe uma reserva para este período.");
        }

        reserva.Id = id;
        await _reservaRepository.AtualizarReservaAsync(id, reserva);
        return new ResultadoOperacao(true, "Reserva atualizada com sucesso!");
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
        return await _reservaRepository.ObterTodasResevasAsync();
    }

    public async  Task<List<Reserva>> ObterReservasPorUserId(ObjectId userId)
    {
        return await _reservaRepository.ObterReservasPorUsuarioAsync(userId);
    }
 
    public async Task<ResultadoOperacao> CancelarReservaAsync(ObjectId reservaId, ObjectId userId)
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
