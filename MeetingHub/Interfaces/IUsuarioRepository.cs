using MeetingHub.Actions;
using MeetingHub.Models;
using MongoDB.Bson;
using System.Security.Claims;

namespace MeetingHub.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> CriarUsuarioAsync(Usuario usuario);
    Task<Usuario?> ObterUsuarioPorIdAsync(ObjectId id);
    Task<Usuario?> ObterUsuarioPorEmailAsync(string email);
    Task AtualizarUsuarioAsync(Usuario usuario);
    Task<bool> RemoverUsuarioAsync(ObjectId id);
    Task<List<Usuario>> ObterTodosUsuariosAsync();
    AuthResponse UserAuth(ClaimsPrincipal user);


}