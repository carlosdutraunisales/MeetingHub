using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MeetingHub.Models;

public class Usuario
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("Nome")]
    public string Nome { get; set; }
    
    [BsonElement("Email")]
    public string Email { get; set; }
    
    [BsonElement("SenhaHash")]
    public string SenhaHash { get; set; }
    
    [BsonElement("NivelAcesso")]
    public string NivelAcesso { get; set; }
}