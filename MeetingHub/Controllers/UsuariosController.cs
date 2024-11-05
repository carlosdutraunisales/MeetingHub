using MeetingHub.Interfaces;
using MeetingHub.Models;
using MeetingHub.Repositories;
using MeetingHub.Services;
using MeetingHub.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MeetingHub.Controllers;

[Route("api/usuarios")]
[ApiController]
public class UsuariosController: ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly TokenService _tokenService;

    public UsuariosController(IUsuarioService usuarioService, TokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodosUsuarios()
    {
        var usuarios = await _usuarioService.ObterTodosUsuariosAsync();
        return Ok(usuarios);
    }
    
    [HttpPost]
    public async Task<IActionResult> CriarUsuario([FromBody] Usuario usuario)
    {
        try
        {
            var novoUsuario = await _usuarioService.CriarUsuarioAsync(usuario);
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = novoUsuario.Id }, novoUsuario);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterUsuarioPorId(ObjectId id)
    {
        var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> ObterUsuarioPorEmail(string email)
    {
        var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(email);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarUsuario(ObjectId id, [FromBody] Usuario usuario)
    {
        usuario.Id = id;
        await _usuarioService.AtualizarUsuarioAsync(usuario);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverUsuario(ObjectId id)
    {
        var sucesso = await _usuarioService.RemoverUsuarioAsync(id);
        return sucesso ? NoContent() : NotFound();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(model.email);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.senha, usuario.SenhaHash))
        {
            return Unauthorized("Nome de usuário ou senha inválidos.");
        }

        var token = _tokenService.GerarToken(usuario);
        return Ok(new { Token = token });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUserEmail()
    {
        var loggedUser = await _usuarioService.UsuariosAutenticados(User);

        return loggedUser.Success ? Ok(new { Email = loggedUser.Message }) : Unauthorized(loggedUser.Message);

    }

    /*[HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        // Hash da senha antes de salvar (exemplo simplificado)
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(model.senha);

        var usuario = new Usuario
        {
            Nome = model.Nome,
            Email = model.email,
            SenhaHash = senhaHash,
            NivelAcesso = model.nivelAcesso
        };

        await _usuarioRepository.CriarUsuarioAsync(usuario);
        return Ok("Usuário Registrado com sucesso.");
    }*/

}