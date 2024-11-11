namespace MeetingHub.Response
{
    public class ResultadoOperacao
    {
        public bool Sucesso { get;  }
        public string Mensagem { get;  }

        public ResultadoOperacao(bool sucesso, string mensagem = "")
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }
    }
}
