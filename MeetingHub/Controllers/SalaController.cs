using MeetingHub.Models;
using MeetingHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MeetingHub.Controllers;

[Route("api/salas")]
[ApiController]
public class SalaController: ControllerBase
{
    private readonly SalaService _salaService;

    public SalaController(SalaService salaService)
    {
        _salaService = salaService;
    }
    
    [Authorize(Roles = "Administrador,Usuario")]
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _salaService.ObterTodasSalasAsync());
    
    [Authorize(Roles = "Administrador,Usuario")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            return Ok(await _salaService.ObterSalaPorIdAsync(ObjectId.Parse(id)));
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Sala não encontrada.");
        }
    }
    
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create(Sala sala)
    {
        await _salaService.CriarSalaAsync(sala);
        return CreatedAtAction(nameof(GetById), new { id = sala.Id.ToString() }, sala);
    }
    
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Sala sala)
    {
        try
        {
            await _salaService.AtualizarSalaAsync(ObjectId.Parse(id), sala);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Sala não encontrada.");
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _salaService.ExcluirSalaAsync(ObjectId.Parse(id));
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Sala não encontrada.");
        }
    }

    [Authorize(Roles = "Administrador,Usuario")]
    [HttpGet("disponiveis")]
    public async Task<IActionResult> GetSalasDisponiveis(DateTime data, int? capacidade = null, List<string> recursos = null)
    {
        try
        {
            var salasDisponiveis = await _salaService.BuscarSalasDisponiveis(data, capacidade, recursos);
            return Ok(salasDisponiveis);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }


}