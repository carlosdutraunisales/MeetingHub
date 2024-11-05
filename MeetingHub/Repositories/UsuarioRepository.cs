using MeetingHub.Actions;
using MeetingHub.Data;
using MeetingHub.Interfaces;
using MeetingHub.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace MeetingHub.Repositories;

public class UsuarioRepository: IUsuarioRepository
{
    private readonly IMongoCollection<Usuario>? _usuarios;

    public UsuarioRepository(MongoDbService mongoDbService )
    {
        _usuarios = mongoDbService.Database?.GetCollection<Usuario>("Usuarios");
    }

    public async Task<Usuario> CriarUsuarioAsync(Usuario usuario)
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
        
        usuario.Id = ObjectId.GenerateNewId();
        usuario.SenhaHash = senhaHash; 
        await _usuarios.InsertOneAsync(usuario);
        return usuario;
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(ObjectId id)
    {
        return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
    {
        return await _usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task AtualizarUsuarioAsync(Usuario usuario)
    {
        await _usuarios.ReplaceOneAsync(u => u.Id == usuario.Id, usuario);
    }

    public async Task<bool> RemoverUsuarioAsync(ObjectId id)
    {
        var result = await _usuarios.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Usuario>> ObterTodosUsuariosAsync()
    {
        return await _usuarios.Find(_ => true).ToListAsync();
    }

    public AuthResponse UserAuth(ClaimsPrincipal user)
    {
        var userEmail = user.FindFirst(ClaimTypes.Email)?.Value;

        return userEmail == null ?
            new AuthResponse { Message = "E-mail não encontrado no token.", Success = false } :
            new AuthResponse { Message = userEmail, Success = true };
    }
}