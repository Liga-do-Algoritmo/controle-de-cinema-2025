using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleDeCinema.Teste.Integracao.ModuloGenero
{
    [TestClass]
    [TestCategory("Teste de Integração de Genero")]
    public sealed class RepositorioGenerEmOrmTests
    {

        private ControleDeCinemaDbContext dbcontext;
        private RepositorioGeneroFilmeEmOrm repositorioGenero;

        [TestInitialize]

        public void ConfigurarTeste()
        {
            var assembly = typeof(RepositorioGenerEmOrmTests).Assembly;

            var configuracao = new ConfigurationBuilder()
                .AddUserSecrets(assembly)
                .Build();

            var connectionString = configuracao["SQL_CONNECTION_STRING"];

            var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            dbcontext = new ControleDeCinemaDbContext(options);

            repositorioGenero = new RepositorioGeneroFilmeEmOrm(dbcontext);

            dbcontext.Database.EnsureDeleted();
            dbcontext.Database.EnsureCreated();
        }

        [TestMethod]
        public void Deve_Cadastrar_Genero_Corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("terror");

            //açao
            repositorioGenero.Cadastrar(genero);
            dbcontext.SaveChanges();

            //assenção
            var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

            Assert.AreEqual(genero, registroSelecionado);
        }

        [TestMethod]

        public void Deve_Selecionar_Generro_corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("aventura");
            var genero2 = new GeneroFilme("drama");
            var genero3 = new GeneroFilme("ação");
            repositorioGenero.Cadastrar(genero);
            repositorioGenero.Cadastrar(genero2);
            repositorioGenero.Cadastrar(genero3);

            List<GeneroFilme> generosEsperados = [genero, genero2, genero3];
            dbcontext.SaveChanges();

            //açao
            var GenerosSelecionados = repositorioGenero.SelecionarRegistros();

            //assenção
            CollectionAssert.AreEquivalent(generosEsperados, GenerosSelecionados);

        }

        [TestMethod]

        public void Deve_Editar_Genero_Corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("terror");

            repositorioGenero.Cadastrar(genero);
            dbcontext.SaveChanges();

            var generoEditado = new GeneroFilme("aventura");
            dbcontext.SaveChanges();
            //açao
            var conseguiuEditar = repositorioGenero.Editar(genero.Id, generoEditado);
            dbcontext.SaveChanges();

            //assenção

            Assert.IsTrue(conseguiuEditar);

        }

        [TestMethod]

        public void Deve_Excluir_Genero_Corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("drama");

            repositorioGenero.Cadastrar(genero);
            dbcontext.SaveChanges();


            //açao
            var ConseguiuExcluir = repositorioGenero.Excluir(genero.Id);
            dbcontext.SaveChanges();

            //assenção
            var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

            Assert.IsNull(registroSelecionado);
            Assert.IsTrue(ConseguiuExcluir);

        }
    }
}
