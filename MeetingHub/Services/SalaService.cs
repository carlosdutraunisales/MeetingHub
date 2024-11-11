using MeetingHub.Interfaces;
using MeetingHub.Models;
using MongoDB.Bson;

namespace MeetingHub.Services;

public class SalaService
{
    private readonly ISalaRepository _salaRepository;
    private readonly IReservaRepository _reservaRepository;

    public SalaService(ISalaRepository salaRepository, IReservaRepository reservaRepository)
    {
        _salaRepository = salaRepository;
        _reservaRepository = reservaRepository;
    }

    public async Task<List<Sala>> ObterTodasSalasAsync() => await _salaRepository.ObterTodasSalasAsync();

    public async Task<Sala> ObterSalaPorIdAsync(ObjectId salaId)
    {
        var sala = await _salaRepository.ObterSalaPorIdAsync(salaId);
        if (sala == null)
            throw new KeyNotFoundException("Sala não encontrada.");
        return sala;
    }

    public async Task CriarSalaAsync(Sala sala) => await _salaRepository.CriarSalaAsync(sala);

    public async Task AtualizarSalaAsync(ObjectId salaId, Sala sala)
    {
        var salaExistente = await ObterSalaPorIdAsync(salaId);
        if (salaExistente == null)
            throw new KeyNotFoundException("Sala não encontrada.");

        await _salaRepository.AtualizarSalaAsync(salaId, sala);
    }

    public async Task ExcluirSalaAsync(ObjectId salaId) => await _salaRepository.ExcluirSalaAsync(salaId);


    public virtual async Task<List<Sala>> BuscarSalasDisponiveis(DateTime data, int? capacidade, List<string> recursos)
    {
        var salas = await _salaRepository.BuscarSalasAtivas(capacidade, recursos);

        var salasDisponiveis = new List<Sala>();
        if (salasDisponiveis.Any())
        {
            foreach (var sala in salas)
            {
                var reservaExistente = await _reservaRepository.BuscarReservaPorSalaEData(sala.Id, data);
                if (reservaExistente == null)
                {
                    salasDisponiveis.Add(sala);
                }
            }
        }
        return salasDisponiveis;
    }
}

