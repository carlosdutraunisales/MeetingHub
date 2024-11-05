using MeetingHub.Actions;
using MeetingHub.Interfaces;
using MeetingHub.Models;
using MongoDB.Bson;
using System.Security.Claims;

namespace MeetingHub.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario> CriarUsuarioAsync(Usuario usuario)
    {
        return await _usuarioRepository.CriarUsuarioAsync(usuario);
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(ObjectId id)
    {
        return await _usuarioRepository.ObterUsuarioPorIdAsync(id);
    }

    public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
    {
        return await _usuarioRepository.ObterUsuarioPorEmailAsync(email);
    }

    public async Task AtualizarUsuarioAsync(Usuario usuario)
    {
        await _usuarioRepository.AtualizarUsuarioAsync(usuario);
    }

    public async Task<bool> RemoverUsuarioAsync(ObjectId id)
    {
        return await _usuarioRepository.RemoverUsuarioAsync(id);
    }

    public async Task<List<Usuario>> ObterTodosUsuariosAsync()
    {
        return await _usuarioRepository.ObterTodosUsuariosAsync();
    }

    public async Task<AuthResponse> UsuariosAutenticados(ClaimsPrincipal user) 
    {
        return _usuarioRepository.UserAuth(user);
    }

}