using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Teste.Integracao.Compartilhado;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ControleDeCinema.Teste.Integracao;

[TestClass]
[TestCategory("Teste Integração de Filmes")]
public class RepositorioFilmeEmOrmTests :Testfixture
{
    [TestMethod]
    public void Deve_Cadastrar_Filmes_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("Homem Aranha", 120, true, genero);
        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();

        var FilmeSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.AreEqual(filme, FilmeSelecionado);
    }
    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("As Aventuras de um tester", 90, true, genero);
        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();

        var FilmeEditado = new Filme("o tester sofre", 90, false, genero);

        var ConseguiuEditar = repositorioFilme.Editar(filme.Id, FilmeEditado);

        dbContext.SaveChanges();

        Assert.IsTrue(ConseguiuEditar);
    }
    [TestMethod]
    public void Deve_Selecionar_Filmes_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("A casa Monstro", 120, true, genero);

        var filme2 = new Filme("Cidade Alta", 120, true, genero);

        var filme3 = new Filme("Cidade Baixa", 120, true, genero);
        repositorioFilme.Cadastrar(filme);
        repositorioFilme.Cadastrar(filme2);
        repositorioFilme.Cadastrar(filme3);

        List<Filme> filmesEsperados = [filme, filme2, filme3];
        dbContext.SaveChanges();

        var filmesSelecionados = repositorioFilme.SelecionarRegistros();

        CollectionAssert.AreEquivalent(filmesEsperados, filmesSelecionados);
    }
    [TestMethod]
    public void Deve_Excluir_Filme_Corretemante()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Build();

        var filme = new Filme("Papel House",55, false, genero);

        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();
        var conseguiuExcluir = repositorioFilme.Excluir(filme.Id);
        dbContext.SaveChanges();

        var registroSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }
}
