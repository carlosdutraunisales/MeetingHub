using MeetingHub.Interfaces;
using MeetingHub.Models;
using MeetingHub.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MeetingHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISalaRepository _salaRepository;
    

    public ReservasController(IReservaService reservaService, IUsuarioRepository usuarioRepository, 
         ISalaRepository salaRepository)
    {
        _reservaService = reservaService;
        _usuarioRepository = usuarioRepository;
        _salaRepository = salaRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CriarReserva([FromBody] CriarReservaViewModel model)
    {

        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(_usuarioRepository.UserAuth(User).Message);
        var sala = await _salaRepository.ObterSalaPorCodigoAsync(model.SalaCodigo);
        
        if (sala != null && sala.Ativa == "A")
        {
            var reserva = new Reserva
            {
                SalaId = sala.Id,
                DataInicio = model.DataInicio,
                DataFim = model.DataFim,
                UsuarioId = usuario.Id
            };

            try
            {
                var novaReserva = await _reservaService.CriarReservaAsync(reserva);
                return CreatedAtAction(nameof(ObterReservaPorId), new { id = novaReserva.Id }, novaReserva);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        else
        {
            return sala != null && sala.Ativa == "I" ? BadRequest(new { message = "Sala indisponível para reservas" }) : NotFound(new { message = "Sala não encontrada" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterReservaPorId(ObjectId id)
    {
        var reserva = await _reservaService.ObterReservaPorIdAsync(id);
        return reserva == null ? NotFound() : Ok(reserva);
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> AtualizarReserva(ObjectId id, [FromBody] Reserva reserva)
    //{
    //    var reservaAtualizada = await _reservaService.AtualizarReservaAsync(id, reserva);
    //    return reservaAtualizada == null ? NotFound() : Ok(reservaAtualizada);
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarReserva(ObjectId id)
    {
        var sucesso = await _reservaService.CancelarReservaAsync(id);
        return sucesso ? NoContent() : NotFound();
    }

    [HttpGet("todas")]
    public async Task<IActionResult> GetAllReservas()
    {
        var reservas = await _reservaService.ObterTodasReservas();
        return Ok(reservas);
    }

}