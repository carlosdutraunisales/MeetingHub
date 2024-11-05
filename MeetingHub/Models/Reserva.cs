using MongoDB.Bson;

namespace MeetingHub.Models;

public class Reserva
{
    public ObjectId Id { get; set; } 
    public ObjectId UsuarioId { get; set; }
    public ObjectId SalaId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}