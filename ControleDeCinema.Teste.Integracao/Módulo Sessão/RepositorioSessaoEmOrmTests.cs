using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using ControleDeCinema.Teste.Integracao.Compartilhado;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Teste.Integracao;

[TestClass]
[TestCategory("Teste de Integração de Sessão")]
public class RepositorioSessaoEmOrmTests
{

    private RepositorioSessaoEmOrm repositorioSessao;
    private RepositorioFilmeEmOrm repositorioFilme;
    private RepositorioSalaEmOrm repositoriosala;
    private ControleDeCinemaDbContext dbcontext;
    private RepositorioGeneroFilmeEmOrm repositorioGenero;

    [TestInitialize]
    public void ConfigurarTeste()
    {
        dbcontext = TestDbContextFactory.CriarDbContext();
        repositorioFilme = new RepositorioFilmeEmOrm(dbcontext);
        repositorioSessao = new RepositorioSessaoEmOrm(dbcontext);
        repositoriosala = new RepositorioSalaEmOrm(dbcontext);
        repositorioGenero = new RepositorioGeneroFilmeEmOrm(dbcontext);


        BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Sala>(repositoriosala.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGenero.Cadastrar);
    }
    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = Builder<Filme>.CreateNew()
             .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);

        repositorioSessao.Cadastrar(sessao);

        dbcontext.SaveChanges();

        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, registroSelecionado);
    }
    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);
        repositorioSessao.Cadastrar(sessao);
        dbcontext.SaveChanges();

        var sessaoEditada = new Sessao(DateTime.UtcNow, 75, filme, sala);

        var conseguiuEditar = repositorioSessao.Editar(sessao.Id, sessaoEditada);
        dbcontext.SaveChanges();

        Assert.IsTrue(conseguiuEditar);
    }
    [TestMethod]
    public void Deve_Selecionar_Sessao_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);
        var sessao2 = new Sessao(DateTime.UtcNow, 70, filme, sala);
        var sessao3= new Sessao(DateTime.UtcNow, 90, filme, sala);

        repositorioSessao.Cadastrar(sessao);
        repositorioSessao.Cadastrar(sessao2);
        repositorioSessao.Cadastrar(sessao3);
        dbcontext.SaveChanges();

        List<Sessao> resultadoEsperado = [sessao,sessao2, sessao3];

        var registrosSelecionados = repositorioSessao.SelecionarRegistros();

        CollectionAssert.AreEquivalent(resultadoEsperado, registrosSelecionados);
    }
    [TestMethod]
    public void Deve_ExcluirSessao_corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);

        repositorioSessao.Cadastrar(sessao);

        dbcontext.SaveChanges();

        var conseguiuExcluir = repositorioSessao.Excluir(sessao.Id);

        Assert.IsTrue(conseguiuExcluir);
    }
}
