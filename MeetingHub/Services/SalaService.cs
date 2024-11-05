using MeetingHub.Models;
using MeetingHub.Interfaces;
using MongoDB.Bson;

namespace MeetingHub.Services;

public class SalaService
{
    private readonly ISalaRepository _salaRepository;

    public SalaService(ISalaRepository salaRepository)
    {
        _salaRepository = salaRepository;
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
}
