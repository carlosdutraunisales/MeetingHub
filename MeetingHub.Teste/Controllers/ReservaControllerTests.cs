using MeetingHub.Actions;
using MeetingHub.Controllers;
using MeetingHub.Interfaces;
using MeetingHub.Models;
using MeetingHub.Response;
using MeetingHub.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using System.Security.Claims;
namespace MeetingHub.Teste.Controllers
{
    public class ReservaControllerTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<ISalaRepository> _salaRepositoryMock;
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ReservasController _controller;

        public ReservaControllerTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _salaRepositoryMock = new Mock<ISalaRepository>();
            _reservaServiceMock = new Mock<IReservaService>();

            // Inicializa o controller com os mocks
            _controller = new ReservasController(
                _reservaServiceMock.Object,
                _usuarioRepositoryMock.Object,
                _salaRepositoryMock.Object
            );

            // Simula um usuário autenticado
            var claims = new[] { new Claim(ClaimTypes.Name, "usuario@exemplo.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Atribui o usuário simulado ao controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task CriarReserva_DeveRetornarCreatedAtAction_QuandoSalaAtiva()
        {
            // Arrange
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var sala = new Sala { Id = ObjectId.GenerateNewId(), Codigo = 1, Ativa = "A" };
            var reserva = new Reserva { SalaId = sala.Id, UsuarioId = usuario.Id };

            // Configuração dos mocks
            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>()))
                                  .Returns(new AuthResponse { Message = usuario.Email });
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(It.IsAny<int>())).ReturnsAsync(sala);
            _reservaServiceMock.Setup(s => s.CriarReservaAsync(It.IsAny<Reserva>())).ReturnsAsync(reserva);

            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            // Act
            var result = await _controller.CriarReserva(model);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(reserva, createdResult.Value);
        }

        [Fact]
        public async Task CriarReserva_DeveRetornarBadRequest_QuandoSalaInativa()
        {
            // Arrange
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var salaInativa = new Sala { Id = ObjectId.GenerateNewId(), Codigo = 1, Ativa = "I" }; // Sala inativa

            // Configuração dos mocks
            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>()))
                                  .Returns(new AuthResponse { Message = usuario.Email });
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(It.IsAny<int>())).ReturnsAsync(salaInativa);

            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            // Act
            var result = await _controller.CriarReserva(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CriarReserva_DeveRetornarNotFound_QuandoSalaNaoEncontrada()
        {
            // Arrange
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };

            // Configuração dos mocks
            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>()))
                                  .Returns(new AuthResponse { Message = usuario.Email });
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(It.IsAny<int>())).ReturnsAsync((Sala)null); // Sala não encontrada

            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            // Act
            var result = await _controller.CriarReserva(model);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task ObterReservaPorId_DeveRetornarNotFound_QuandoReservaNaoEncontrada()
        {
            // Arrange
            var reservaId = new ObjectId();

            // Configura o mock 
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync((Reserva)null);

            // Act
            var result = await _controller.ObterReservaPorId(reservaId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ObterReservaPorId_DeveRetornarOkComReserva_QuandoReservaEncontrada()
        {
            // Arrange
            var reservaId = new ObjectId();
            var reserva = new Reserva { Id = reservaId, SalaId = ObjectId.GenerateNewId(), UsuarioId = ObjectId.GenerateNewId() };

            // Configura o mock para retornar a reserva simulada
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync(reserva);

            // Act
            var result = await _controller.ObterReservaPorId(reservaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reserva, okResult.Value);
        }

        [Fact]
        public async Task AtualizarReserva_DeveRetornarBadRequest_QuandoReservaNaoEncontrada()
        {
            // Arrange
            var reservaId = new ObjectId();
            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(new Usuario());
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = "usuario@exemplo.com" });
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync((Reserva)null);

            // Act
            var result = await _controller.AtualizarReserva(reservaId, model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarReserva_DeveRetornarNotFound_QuandoSalaNaoEncontrada()
        {
            // Arrange
            var reservaId = new ObjectId();
            var reserva = new Reserva { Id = reservaId };
            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(new Usuario());
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = "usuario@exemplo.com" });
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync(reserva);
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(model.SalaCodigo)).ReturnsAsync((Sala)null);

            // Act
            var result = await _controller.AtualizarReserva(reservaId, model);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarReserva_DeveRetornarBadRequest_QuandoSalaInativa()
        {
            // Arrange
            var reservaId = new ObjectId();
            var reserva = new Reserva { Id = reservaId };
            var salaInativa = new Sala { Id = ObjectId.GenerateNewId(), Codigo = 1, Ativa = "I" }; // Sala inativa
            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(new Usuario());
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = "usuario@exemplo.com" });
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync(reserva);
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(model.SalaCodigo)).ReturnsAsync(salaInativa);

            // Act
            var result = await _controller.AtualizarReserva(reservaId, model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarReserva_DeveRetornarOk_QuandoAtualizacaoBemSucedida()
        {
            // Arrange
            var reservaId = new ObjectId();
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var reserva = new Reserva { Id = reservaId };
            var sala = new Sala { Id = ObjectId.GenerateNewId(), Codigo = 1, Ativa = "A" };
            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync(reserva);
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(model.SalaCodigo)).ReturnsAsync(sala);
            _reservaServiceMock.Setup(s => s.AtualizarReservaAsync(reservaId, usuario.Id, reserva)).ReturnsAsync(new ResultadoOperacao(true, "Reserva atualizada com sucesso"));

            // Act
            var result = await _controller.AtualizarReserva(reservaId, model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Reserva atualizada com sucesso", okResult.Value);
        }

        [Fact]
        public async Task AtualizarReserva_DeveRetornarBadRequest_QuandoExcecaoLancada()
        {
            // Arrange
            var reservaId = new ObjectId();
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var reserva = new Reserva { Id = reservaId };
            var sala = new Sala { Id = ObjectId.GenerateNewId(), Codigo = 1, Ativa = "A" };
            var model = new CriarReservaViewModel { SalaCodigo = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.ObterReservaPorIdAsync(reservaId)).ReturnsAsync(reserva);
            _salaRepositoryMock.Setup(r => r.ObterSalaPorCodigoAsync(model.SalaCodigo)).ReturnsAsync(sala);
            _reservaServiceMock.Setup(s => s.AtualizarReservaAsync(reservaId, usuario.Id, reserva)).ThrowsAsync(new Exception("Erro ao atualizar reserva"));

            // Act
            var result = await _controller.AtualizarReserva(reservaId, model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CancelarReserva_DeveRetornarNoContent_QuandoCancelamentoBemSucedido()
        {
            // Arrange
            var reservaId = new ObjectId();
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var resultadoCancelamento = new ResultadoOperacao(true);

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.CancelarReservaAsync(reservaId, usuario.Id)).ReturnsAsync(resultadoCancelamento);

            // Act
            var result = await _controller.CancelarReserva(reservaId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CancelarReserva_DeveRetornarNotFound_QuandoReservaNaoEncontrada()
        {
            // Arrange
            var reservaId = new ObjectId();
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var resultadoCancelamento = new ResultadoOperacao(false, "Reserva não encontrada");

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.CancelarReservaAsync(reservaId, usuario.Id)).ReturnsAsync(resultadoCancelamento);

            // Act
            var result = await _controller.CancelarReserva(reservaId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Reserva não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task ObterTodasReservas_DeveRetornarOkComReservas_QuandoExistiremReservas()
        {
            // Arrange
            var reservas = new List<Reserva>
    {
        new Reserva { Id = new ObjectId(), SalaId = new ObjectId(), DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1) },
        new Reserva { Id = new ObjectId(), SalaId = new ObjectId(), DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(2) }
    };

            _reservaServiceMock.Setup(s => s.ObterTodasReservas()).ReturnsAsync(reservas);

            // Act
            var result = await _controller.ObterTodasReservas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reservas, okResult.Value);
        }

        [Fact]
        public async Task ObterTodasReservas_DeveRetornarOkComListaVazia_QuandoNaoExistiremReservas()
        {
            // Arrange
            var reservas = new List<Reserva>();

            _reservaServiceMock.Setup(s => s.ObterTodasReservas()).ReturnsAsync(reservas);

            // Act
            var result = await _controller.ObterTodasReservas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Empty((IEnumerable<Reserva>)okResult.Value);
        }

        [Fact]
        public async Task ObterReservasUsuario_DeveRetornarOkComReservas_QuandoUsuarioTemReservas()
        {
            // Arrange
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var reservas = new List<Reserva>
    {
        new Reserva { Id = new ObjectId(), SalaId = new ObjectId(), DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(1), UsuarioId = usuario.Id },
        new Reserva { Id = new ObjectId(), SalaId = new ObjectId(), DataInicio = DateTime.Now, DataFim = DateTime.Now.AddHours(2), UsuarioId = usuario.Id }
    };

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.ObterReservasPorUsuarioAsync(usuario.Id)).ReturnsAsync(reservas);

            // Act
            var result = await _controller.ObterReservasUsuario();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reservas, okResult.Value);
        }

        [Fact]
        public async Task ObterReservasUsuario_DeveRetornarOkComListaVazia_QuandoUsuarioNaoTemReservas()
        {
            // Arrange
            var usuario = new Usuario { Id = ObjectId.GenerateNewId(), Email = "usuario@exemplo.com" };
            var reservas = new List<Reserva>();

            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = usuario.Email });
            _reservaServiceMock.Setup(s => s.ObterReservasPorUsuarioAsync(usuario.Id)).ReturnsAsync(reservas);

            // Act
            var result = await _controller.ObterReservasUsuario();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Empty((IEnumerable<Reserva>)okResult.Value);
        }

        [Fact]
        public async Task ObterReservasUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExiste()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario)null);
            _usuarioRepositoryMock.Setup(r => r.UserAuth(It.IsAny<ClaimsPrincipal>())).Returns(new AuthResponse { Message = "usuario@exemplo.com" });

            // Act
            var result = await _controller.ObterReservasUsuario();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}