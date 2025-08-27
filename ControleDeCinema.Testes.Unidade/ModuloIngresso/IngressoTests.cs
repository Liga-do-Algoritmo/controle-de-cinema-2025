using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ControleDeCinema.Testes.Unidade.ModuloIngresso
{
    [TestClass]
    [TestCategory("Testes de Unidade - Ingresso/Sessão")]
    public sealed class IngressoTests
    {
        private Sala CriarSalaFake() => new Sala(1, 50);

        private Filme CriarFilmeFake()
        {
            var genero = new GeneroFilme("Ação");
            return new Filme("Matrix", 120, true, genero);
        }

        [TestMethod]
        public void Deve_Comprar_Ingresso_Com_Sucesso()
        {
    
            var sala = CriarSalaFake();
            var filme = CriarFilmeFake();
            var sessao = new Sessao(DateTime.Now, 10, filme, sala);

       
            var ingresso = sessao.GerarIngresso(1, false);

     
            Assert.IsNotNull(ingresso);
            Assert.AreEqual(1, ingresso.NumeroAssento);
            Assert.IsFalse(ingresso.MeiaEntrada);
            Assert.AreEqual(9, sessao.ObterQuantidadeIngressosDisponiveis());
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Compra_Se_Sessao_Lotada()
        {
       
            var sala = CriarSalaFake();
            var filme = CriarFilmeFake();
            var sessao = new Sessao(DateTime.Now, 2, filme, sala);

            sessao.GerarIngresso(1, false);
            sessao.GerarIngresso(2, false);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                sessao.GerarIngresso(3, true);
            });

      
            Assert.AreEqual(0, sessao.ObterQuantidadeIngressosDisponiveis());
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Comprar_Assento_Repetido()
        {
   
            var sala = CriarSalaFake();
            var filme = CriarFilmeFake();
            var sessao = new Sessao(DateTime.Now, 5, filme, sala);

            sessao.GerarIngresso(1, false);

      
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                sessao.GerarIngresso(1, true);
            });
        }
    }
}

