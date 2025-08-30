using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
[TestCategory("Teste Unidade de Sessão")]
public class SessaoAppServiceTests
{
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioSessao> repositorioSessaoMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SessaoAppService>> loggerMock;

    private SessaoAppService sessaoAppService;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSessaoMock = new Mock<IRepositorioSessao>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SessaoAppService>>();

        sessaoAppService = new SessaoAppService(
            tenantProviderMock.Object,
            repositorioSessaoMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object);

        var genero = new GeneroFilme("ação");
        var filme = new Filme("homem aranha", 90, true, genero);

        var sala = new Sala();
        var sessao = new Sessao(DateTime.Now, 3, filme, sala);
    }


    [TestMethod]
    public void Nao_Deve_Cadastrar_Sessao_Com_Campos_Obrigatorios_Vazios()
    {
        //arrange
        var sessaoInvalida = new Sessao(DateTime.MinValue, 0, null, null);
        //act
        var resultado = sessaoAppService.Cadastrar(sessaoInvalida);

        //assert
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Erro ao cadastrar sessão")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Deve conter uma sala")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Deve conter uma um filme para cadastrar uma sessão")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("O Número de ingressos Não pode ser 0")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("O inicio da sessão deve ser informado")));
    }
    [TestMethod]
    public void Nao_Deve_Cadastrar_Sessao_Com_Conflito_De_Horario()
    {
        // arrange
        var genero = new GeneroFilme("ficção");
        var filme = new Filme("Matrix", 90, true, genero);
        var sala = new Sala();

        var sessaoExistente = new Sessao(DateTime.Today.AddHours(20), 50, filme, sala);
        var novaSessao = new Sessao(DateTime.Today.AddHours(21), 50, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao> { sessaoExistente });

        // act
        Result resultado = sessaoAppService.Cadastrar(novaSessao);

        // assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro duplicado", resultado.Errors[0].Message);
    }


}

