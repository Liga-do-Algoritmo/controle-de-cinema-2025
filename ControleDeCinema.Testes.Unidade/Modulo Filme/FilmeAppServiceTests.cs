using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
[TestCategory("Teste Unidade de Filmes")]
public class FilmeAppServiceTests
{     
        private Mock<ITenantProvider> tenantProviderMock;
        private Mock<IRepositorioFilme> repositorioFilmeMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<ILogger<FilmeAppService>> loggerMock;

        private FilmeAppService filmeAppService;

        [TestInitialize]
        public void Setup()
        {
            tenantProviderMock = new Mock<ITenantProvider>();
            repositorioFilmeMock = new Mock<IRepositorioFilme>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogger<FilmeAppService>>();

        filmeAppService = new FilmeAppService(
            tenantProviderMock.Object,
            repositorioFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
        }
    [TestMethod]

    public void Nao_Deve_Cadastrar_Filme_Com_Campos_Obrigatorios_Vazios()
    {
        // arrange
        var filme = new Filme(null, 0, true, null); 

        // act
        var resultado = filmeAppService.Cadastrar(filme);

        // assert
        Assert.IsTrue(resultado.IsFailed);

        var mensagensEsperadas = new List<string>
    {
        "O Título é obrigatorio",
        "A duração deve ser um número positivo",
        "O Gênero é obrigatorio"
    };

        var mensagensObtidas = resultado.Errors.Select(e => e.Message).ToList();

        CollectionAssert.AreEquivalent(mensagensEsperadas, mensagensObtidas);
    }
    [TestMethod]
    
    public void Filme_Deve_Conte_Duracao_Positiva()
    {
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("teste",-1, true, genero);

        var resultado = filmeAppService.Cadastrar(filme);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_Deve_Falhar_Quando_Filme_For_Duplicado()
    {
        // arrange
        var genero = new GeneroFilme("Ação");

        var filmeExistente = new Filme("testando",10,true, genero);

        var filmeNovo = new Filme ("testando", 10, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> {filmeExistente});

        // act
        Result resultado = filmeAppService.Cadastrar(filmeNovo);

        // assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um filme registrado com este título.", resultado.Errors[0].Message);
    }
}

