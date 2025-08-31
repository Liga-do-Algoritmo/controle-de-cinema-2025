using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
[TestCategory("Teste Unidade de Gênero")]
public class GeneroAppServiceTest
{
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioGeneroFilme> repositorioFgeneroMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<GeneroFilmeAppService>> loggerMock;

    private GeneroFilmeAppService generoAppService;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioFgeneroMock = new Mock<IRepositorioGeneroFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();

        generoAppService = new GeneroFilmeAppService(
            tenantProviderMock.Object,
            repositorioFgeneroMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Genero_Com_Campos_Obrigatorios_Vazios()
    {
        // arrange
        var genero = new GeneroFilme(null);

        // act
        var resultado = generoAppService.Cadastrar(genero);

        // assert
        var mensagemErro = resultado.Errors.First().Message;

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O Gênero é obrigatorio", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_Deve_Falhar_Quando_Descricao_Duplicada()
    {
        // arrange
        var generoExistente = new GeneroFilme("Ação");
        var generoNovo = new GeneroFilme("Ação");

        repositorioFgeneroMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme> { generoExistente });

        // act
        Result resultado = generoAppService.Cadastrar(generoNovo);

        // assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro duplicado", resultado.Errors[0].Message);
    }
}
