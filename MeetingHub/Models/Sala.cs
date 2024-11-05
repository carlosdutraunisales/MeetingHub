using MongoDB.Bson;

namespace MeetingHub.Models;

public class Sala
{
    public ObjectId Id { get; set; }
    public int Codigo { get; set; }
    public string Nome { get; set; }
    public int Capacidade { get; set; }
    public List<string> Recursos { get; set; } // ex: "Projetor", "TV"
    public string Ativa { get; set; } // A para ativa, I para inativa
}