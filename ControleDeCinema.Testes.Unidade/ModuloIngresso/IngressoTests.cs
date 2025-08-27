using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloGeneroFilme;


namespace ControleDeCinema.Testes.Unidade.ModuloIngresso
{
    [TestClass]
    public class IngressoTest
    {
        private Filme filme;
        private Sala sala;
        private Sessao sessao;

        [TestInitialize]
        public void Setup()
        {
            GeneroFilme genero = new GeneroFilme("Ação");
            filme = new Filme("Matrix", 120, true, genero);
            sala = new Sala(1, 10); // Sala nº1 com capacidade 10
            sessao = new Sessao(DateTime.Now.AddDays(1), 10, filme, sala);
        }

        [TestMethod]
        public void Deve_Comprar_Ingresso_Com_Sucesso()
        {
       
            var ingresso = new Ingresso(1, false, sessao);

    
            sessao.Ingressos.Add(ingresso);


            Assert.AreEqual(1, sessao.Ingressos.Count);
            Assert.AreEqual(1, sessao.Ingressos[0].NumeroAssento);
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Compra_Quando_Sessao_Lotada()
        {
 
            for (int i = 1; i <= sessao.NumeroMaximoIngressos; i++)
                sessao.Ingressos.Add(new Ingresso(i, false, sessao));


            var ingressoExtra = new Ingresso(11, false, sessao);
            bool podeComprar = sessao.Ingressos.Count < sessao.NumeroMaximoIngressos;

  
            Assert.IsFalse(podeComprar);
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Compra_De_Assento_Ja_Ocupado()
        {
   
            sessao.Ingressos.Add(new Ingresso(5, false, sessao));

  
            bool assentoDisponivel = sessao.Ingressos.TrueForAll(i => i.NumeroAssento != 5);


            Assert.IsFalse(assentoDisponivel);
        }

        [TestMethod]
        public void Deve_Aceitar_Compra_Com_MeiaEntrada()
        {

            var ingresso = new Ingresso(2, true, sessao);


            sessao.Ingressos.Add(ingresso);


            Assert.AreEqual(1, sessao.Ingressos.Count);
            Assert.IsTrue(sessao.Ingressos[0].MeiaEntrada);
        }
    }
}



