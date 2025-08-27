using ControleDeCinema.Dominio.ModuloSala;

namespace ControleDeCinema.Testes.Unidade.ModuloSala
{
    [TestClass]
    [TestCategory("Testes de Unidade - Sala")]
    public sealed class SalaTests
    {
        [TestMethod]
        public void Deve_Criar_Sala_Com_Dados_Validos()
        {
    
            var numeroEsperado = 1;
            var capacidadeEsperada = 100;


            var sala = new Sala(numeroEsperado, capacidadeEsperada);

       
            Assert.AreEqual(numeroEsperado, sala.Numero);
            Assert.AreEqual(capacidadeEsperada, sala.Capacidade);
            Assert.AreNotEqual(Guid.Empty, sala.Id);
        }

        [TestMethod]
        public void Deve_Atualizar_Sala_Corretamente()
        {
       
            var sala = new Sala(1, 100);
            var salaEditada = new Sala(2, 150);

         
            sala.AtualizarRegistro(salaEditada);

         
            Assert.AreEqual(2, sala.Numero);
            Assert.AreEqual(150, sala.Capacidade);
        }
    }
}

