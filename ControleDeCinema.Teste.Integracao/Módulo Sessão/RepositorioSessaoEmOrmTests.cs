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
using System.Security.Cryptography.X509Certificates;

namespace ControleDeCinema.Teste.Integracao;

[TestClass]
[TestCategory("Teste de Integração de Sessão")]
public class RepositorioSessaoEmOrmTests : Testfixture
{

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

        dbContext.SaveChanges();

        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, registroSelecionado);
    }
    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Build();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Build();

        var sala = Builder<Sala>.CreateNew().Build();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);
        repositorioSessao.Cadastrar(sessao);
        dbContext.SaveChanges();

        var sessaoEditada = new Sessao(DateTime.UtcNow, 75, filme, sala);

        var conseguiuEditar = repositorioSessao.Editar(sessao.Id, sessaoEditada);
        dbContext.SaveChanges();

        Assert.IsTrue(conseguiuEditar);
    }
    [TestMethod]
    public void Deve_Selecionar_Sessao_Corretamente()
    {
        var genero = Builder<GeneroFilme>.CreateNew().Build();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);
        var sessao2 = new Sessao(DateTime.UtcNow, 70, filme, sala);
        var sessao3 = new Sessao(DateTime.UtcNow, 90, filme, sala);

        repositorioSessao.Cadastrar(sessao);
        repositorioSessao.Cadastrar(sessao2);
        repositorioSessao.Cadastrar(sessao3);
        dbContext.SaveChanges();

        List<Sessao> resultadoEsperado = [sessao, sessao2, sessao3];

        var registrosSelecionados = repositorioSessao.SelecionarRegistros();

        CollectionAssert.AreEquivalent(resultadoEsperado, registrosSelecionados);
    }
        [TestMethod]

         void Deve_ExcluirSessao_corretamente()
        {
            var genero = Builder<GeneroFilme>.CreateNew().Persist();

            var filme = Builder<Filme>.CreateNew()
                .With(f => f.Genero = genero)
                .Persist();

            var sala = Builder<Sala>.CreateNew().Persist();

            var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);

            repositorioSessao.Cadastrar(sessao);

            dbContext.SaveChanges();

            var conseguiuExcluir = repositorioSessao.Excluir(sessao.Id);

            Assert.IsTrue(conseguiuExcluir);
        }
    }

