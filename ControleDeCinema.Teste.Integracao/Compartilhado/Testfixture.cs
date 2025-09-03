using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using ControleDeCinema.Teste.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Teste.Integracao;

[TestClass]
public abstract class Testfixture
{
    protected static TestDbContextFactory Factory;
    protected RepositorioGeneroFilmeEmOrm repositorioGenero;

    protected RepositorioFilmeEmOrm repositorioFilme;
    protected ControleDeCinemaDbContext dbContext;

    protected RepositorioSessaoEmOrm repositorioSessao;
    protected RepositorioSalaEmOrm repositorioSala;

    [AssemblyInitialize]
    public static async Task Setup(TestContext context)
    {
        Factory = new TestDbContextFactory();

        await Factory.InicializarAsync();
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        if(Factory is not null)
        await Factory.EncerrarAsync();
    }

    [TestInitialize]
    public void ConfigurarTeste()
    {
        dbContext = Factory.CriarDbContext();

        if (dbContext is null)
            throw new  ArgumentNullException("dbContextFactory não inicializada");

        ConfigurarTabelas(dbContext);
        repositorioSala = new RepositorioSalaEmOrm(dbContext);
        repositorioGenero = new RepositorioGeneroFilmeEmOrm(dbContext);
        repositorioFilme = new RepositorioFilmeEmOrm(dbContext);
        repositorioSessao = new RepositorioSessaoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Sala>(repositorioSala.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGenero.Cadastrar);
       
        
    }
    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbcontext)
    {

        dbcontext.Teste.RemoveRange(dbcontext.Teste);
        dbcontext.GenerosFilme.RemoveRange(dbcontext.GenerosFilme);
        dbcontext.Filmes.RemoveRange(dbcontext.Filmes);
        dbcontext.Sessoes.RemoveRange(dbcontext.Sessoes);
        dbcontext.Salas.RemoveRange(dbcontext.Salas);

        dbcontext.SaveChanges();
    }
}

