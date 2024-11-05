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
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterReservaPorId(ObjectId id)
    {
        var reserva = await _reservaService.ObterReservaPorIdAsync(id);
        return reserva == null ? NotFound() : Ok(reserva);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarReserva(ObjectId id, [FromBody] CriarReservaViewModel model)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(_usuarioRepository.UserAuth(User).Message);
        var reserva = await _reservaService.ObterReservaPorIdAsync(id);
        if (reserva == null)
        {
            return BadRequest(new { message = "Reserva não encontrada" });
        }
        else
        {
            var sala = await _salaRepository.ObterSalaPorCodigoAsync(model.SalaCodigo);
            if (sala != null && sala.Ativa == "A")
            {
                reserva.SalaId = sala.Id;
                reserva.DataInicio = model.DataInicio;
                reserva.DataFim = model.DataFim;
                try
                {
                    var reservaAtualizada = await _reservaService.AtualizarReservaAsync(id, usuario.Id, reserva);
                    return reservaAtualizada == null ? NotFound() : Ok(reservaAtualizada.Mensagem);
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
        
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarReserva(ObjectId id)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(_usuarioRepository.UserAuth(User).Message);
        var resultado = await _reservaService.CancelarReservaAsync(id, usuario.Id);
        return resultado.Sucesso ? NoContent() : NotFound(resultado.Mensagem);
    }
    
    [Authorize]
    [HttpGet("todas")]
    public async Task<IActionResult> ObterTodasReservas()
    {
        var reservas = await _reservaService.ObterTodasReservas();
        return Ok(reservas);
    }

    [Authorize]
    [HttpGet("proprias")]
    public async Task<IActionResult> ObterReservasUsuario()
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(_usuarioRepository.UserAuth(User).Message);
        var reservas = await _reservaService.ObterReservasPorUsuarioAsync(usuario.Id);
        return Ok(reservas);
    }

}