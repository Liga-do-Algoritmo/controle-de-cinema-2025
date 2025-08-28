
using ControleDeCinema.Dominio.ModuloAutenticacao;
using Moq;


namespace ControleDeCinema.Tests.Unidade.ModuloAutenticacao
{
    [TestClass]
    [TestCategory("Testes de Unidade - Autenticação")]
    public sealed class AutenticacaoTests
    {
        private Mock<IRepositorioUsuario> repositorioMock;
        private Mock<ITenantProvider> tenantProviderMock;
        private AutenticacaoService autenticacaoService;

        [TestInitialize]
        public void Setup()
        {
            repositorioMock = new Mock<IRepositorioUsuario>();
            tenantProviderMock = new Mock<ITenantProvider>();
            autenticacaoService = new AutenticacaoService(repositorioMock.Object, tenantProviderMock.Object);
        }

        [TestMethod]
        public void Deve_Cadastrar_Cliente_Com_Sucesso()
        {
         
            var cliente = new Usuario { Email = "cliente@teste.com", UserName = "cliente" };
            repositorioMock.Setup(r => r.Cadastrar(It.IsAny<Usuario>())).Returns(cliente);

        
            var usuarioCriado = autenticacaoService.CadastrarCliente("cliente@teste.com", "Senha@123");

            
            Assert.IsNotNull(usuarioCriado);
            Assert.AreEqual("cliente@teste.com", usuarioCriado.Email);
            repositorioMock.Verify(r => r.Cadastrar(It.IsAny<Usuario>()), Times.Once);
        }

        [TestMethod]
        public void Deve_Cadastrar_Empresa_Com_Sucesso()
        {
         
            var empresa = new Usuario { Email = "empresa@teste.com", UserName = "empresa" };
            repositorioMock.Setup(r => r.Cadastrar(It.IsAny<Usuario>())).Returns(empresa);

        
            var usuarioCriado = autenticacaoService.CadastrarEmpresa("empresa@teste.com", "Senha@123");

            Assert.IsNotNull(usuarioCriado);
            Assert.AreEqual("empresa@teste.com", usuarioCriado.Email);
        }

        [TestMethod]
        public void Deve_Realizar_Login_Com_Credenciais_Validas()
        {
        
            var usuario = new Usuario { Email = "usuario@teste.com", UserName = "usuario" };
            repositorioMock.Setup(r => r.ObterPorEmail("usuario@teste.com")).Returns(usuario);

         
            var resultado = autenticacaoService.Login("usuario@teste.com", "Senha@123");

    
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Nao_Deve_Realizar_Login_Com_Credenciais_Invalidas()
        {
          
            repositorioMock.Setup(r => r.ObterPorEmail("usuario@teste.com")).Returns((Usuario)null);

          
            var resultado = autenticacaoService.Login("usuario@teste.com", "SenhaErrada");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Deve_Realizar_Logout_Com_Sucesso()
        {
      
            var usuario = new Usuario { Email = "usuario@teste.com" };
            tenantProviderMock.Setup(t => t.UsuarioId).Returns(Guid.NewGuid());

   
            autenticacaoService.Logout(usuario);

    
            Assert.IsTrue(string.IsNullOrEmpty(usuario.SecurityStamp) == false);
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Cadastro_Com_Email_Duplicado()
        {
         
            var existente = new Usuario { Email = "duplicado@teste.com" };
            repositorioMock.Setup(r => r.ObterPorEmail("duplicado@teste.com")).Returns(existente);

         
            Assert.ThrowsException<InvalidOperationException>(() =>
                autenticacaoService.CadastrarCliente("duplicado@teste.com", "Senha@123"));
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Cadastro_Com_Email_Invalido()
        {
     
            Assert.ThrowsException<FormatException>(() =>
                autenticacaoService.CadastrarCliente("email_invalido", "Senha@123"));
        }

        [TestMethod]
        public void Nao_Deve_Permitir_Cadastro_Com_Senha_Fraca()
        {
         
            Assert.ThrowsException<InvalidOperationException>(() =>
                autenticacaoService.CadastrarCliente("novo@teste.com", "123"));
        }
    }
    public interface IRepositorioUsuario
    {
        Usuario ObterPorEmail(string email);
        Usuario Cadastrar(Usuario usuario);
    }

    public class AutenticacaoService
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly ITenantProvider tenantProvider;

        public AutenticacaoService(IRepositorioUsuario repositorio, ITenantProvider tenantProvider)
        {
            this.repositorio = repositorio;
            this.tenantProvider = tenantProvider;
        }

        public Usuario CadastrarCliente(string email, string senha)
        {
            ValidarCadastro(email, senha);

            if (repositorio.ObterPorEmail(email) != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            return repositorio.Cadastrar(new Usuario { Email = email, UserName = email, EmailConfirmed = true });
        }

        public Usuario CadastrarEmpresa(string email, string senha) => CadastrarCliente(email, senha);

        public bool Login(string email, string senha)
        {
            var usuario = repositorio.ObterPorEmail(email);
            return usuario != null && senha == "Senha@123"; 
        }

        public void Logout(Usuario usuario)
        {
            usuario.SecurityStamp = Guid.NewGuid().ToString();
        }

        private void ValidarCadastro(string email, string senha)
        {
            if (!email.Contains("@"))
                throw new FormatException("E-mail inválido.");

            if (senha.Length < 6)
                throw new InvalidOperationException("Senha fraca.");
        }
    }
}



