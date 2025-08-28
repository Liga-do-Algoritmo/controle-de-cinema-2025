using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Teste.Integracao.Compartilhado;

namespace ControleDeCinema.Teste.Integracao.ModuloSala
{
    [TestClass]
    [TestCategory("Teste de Integração de Sala")]
    public class RepositorioSalaEmOrmTests
    {
        private ControleDeCinemaDbContext dbcontext;
        private RepositorioSalaEmOrm repositorioSala;

        [TestInitialize]
        public void ConfigurarTeste()
        {
            dbcontext = TestDbContextFactory.CriarDbContext();
            repositorioSala = new RepositorioSalaEmOrm(dbcontext);
        }

        [TestMethod]
        public void Deve_Cadastrar_Sala_Corretamente()
        {
            var sala = new Sala(1, 100);

            repositorioSala.Cadastrar(sala);
            dbcontext.SaveChanges();

            var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

            Assert.AreEqual(sala, registroSelecionado);
        }

        [TestMethod]
        public void Deve_Selecionar_Salas_Corretamente()
        {
            var sala1 = new Sala(1, 100);
            var sala2 = new Sala(2, 80);
            var sala3 = new Sala(3, 120);

            repositorioSala.Cadastrar(sala1);
            repositorioSala.Cadastrar(sala2);
            repositorioSala.Cadastrar(sala3);
            dbcontext.SaveChanges();

            var salasEsperadas = new List<Sala> { sala1, sala2, sala3 };
            var salasSelecionadas = repositorioSala.SelecionarRegistros();

            CollectionAssert.AreEquivalent(salasEsperadas, salasSelecionadas);
        }

        [TestMethod]
        public void Deve_Editar_Sala_Corretamente()
        {
            var sala = new Sala(1, 100);
            repositorioSala.Cadastrar(sala);
            dbcontext.SaveChanges();

            var salaEditada = new Sala(1, 150);

            var conseguiuEditar = repositorioSala.Editar(sala.Id, salaEditada);
            dbcontext.SaveChanges();

            Assert.IsTrue(conseguiuEditar);
        }

        [TestMethod]
        public void Deve_Excluir_Sala_Corretamente()
        {
            var sala = new Sala(1, 100);
            repositorioSala.Cadastrar(sala);
            dbcontext.SaveChanges();

            var conseguiuExcluir = repositorioSala.Excluir(sala.Id);
            dbcontext.SaveChanges();

            var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

            Assert.IsNull(registroSelecionado);
            Assert.IsTrue(conseguiuExcluir);
        }
    }
}
