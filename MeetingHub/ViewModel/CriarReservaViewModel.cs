using MongoDB.Bson;

namespace MeetingHub.ViewModel;

public class CriarReservaViewModel
{
    public int  SalaCodigo { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}