using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
[TestCategory("Teste Unidade De Sessão")]
public class SessaoTests
{
    private Sessao sessao;

    [TestInitialize]

    public void setup()
    {
        var genero = new GeneroFilme("ação");
        var filme = new Filme("homem aranha", 90, true, genero);

        var sala = new Sala();
        sessao = new Sessao(DateTime.Now,3,filme,sala);      
    }

    [TestMethod]
    public void Deve_Gerrar_Ingressos_Corretamente()
    {
        //arrenge 
        var assentoSelecionado = 1;
        //act
       var ingresso= sessao.GerarIngresso(assentoSelecionado, true);
        //assert
        Assert.AreEqual(1, sessao.Ingressos.Count);
        Assert.AreEqual(assentoSelecionado, ingresso.NumeroAssento);
        Assert.IsTrue(sessao.Ingressos.Contains(ingresso));
    }
    [TestMethod]
    public void Deve_Obter_Assentos_Disponiveis()
    {    
        //act
        var assentosSelecionado = sessao.ObterAssentosDisponiveis();
        //assert
        Assert.AreEqual(3, assentosSelecionado.Count());
    }
    [TestMethod]
    public void Obter_Quantidade_De_Ingressos_Disponiveis()
    {
        //arrange
       var Ingresso = new Ingresso(2,false,sessao);
       var Ingresso2 = new Ingresso(1,true,sessao);

       sessao.Ingressos.Add(Ingresso);
       sessao.Ingressos.Add(Ingresso2);
        //act
        var IngressosDisponiveis = sessao.ObterQuantidadeIngressosDisponiveis();
        //assert
        Assert.AreEqual(1, IngressosDisponiveis);
    }
    [TestMethod]
    public void Deve_Encerrar_Sessao_Corretamente()
    {
        //act
        sessao.Encerrar();

        //assert
        Assert.IsTrue(sessao.Encerrada);
        
    }
}
