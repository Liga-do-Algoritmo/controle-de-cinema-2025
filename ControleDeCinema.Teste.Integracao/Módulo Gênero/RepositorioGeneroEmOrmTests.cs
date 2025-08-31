using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Teste.Integracao.Compartilhado;
using Testcontainers.PostgreSql;
namespace ControleDeCinema.Teste.Integracao.ModuloGenero
{
    [TestClass]
    [TestCategory("Teste de Integração de Genero")]
    public sealed class RepositorioGeneroEmOrmTests : Testfixture
    {  
        [TestMethod]
        public void Deve_Cadastrar_Genero_Corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("terror");

            //açao
            repositorioGenero.Cadastrar(genero);
            dbContext.SaveChanges();

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
            dbContext.SaveChanges();

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
            dbContext.SaveChanges();

            var generoEditado = new GeneroFilme("aventura");
            dbContext.SaveChanges();
            //açao
            var conseguiuEditar = repositorioGenero.Editar(genero.Id, generoEditado);
            dbContext.SaveChanges();

            //assenção

            Assert.IsTrue(conseguiuEditar);

        }

        [TestMethod]
        public void Deve_Excluir_Genero_Corretamente()
        {
            //arranjo
            var genero = new GeneroFilme("drama");

            repositorioGenero.Cadastrar(genero);
            dbContext.SaveChanges();

            //açao
            var ConseguiuExcluir = repositorioGenero.Excluir(genero.Id);
            dbContext.SaveChanges();

            //assenção
            var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

            Assert.IsNull(registroSelecionado);
            Assert.IsTrue(ConseguiuExcluir);
        }
    }
}
