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
[TestCategory("Teste Unidade de Sess�o")]
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

        var genero = new GeneroFilme("a��o");
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
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Erro ao cadastrar sess�o")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Deve conter uma sala")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Deve conter uma um filme para cadastrar uma sess�o")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("O N�mero de ingressos N�o pode ser 0")));
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("O inicio da sess�o deve ser informado")));
    }
}
