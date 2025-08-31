using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

public class Teste : EntidadeBase<Teste>
{
    public string Titulo { get; set; }
    public  GeneroFilme Genero { get; set; }
    public Filme Filme { get; set; }
    public Sessao Sessao { get; set; }


    public Teste() { }

    public Teste(
        string titulo,
        GeneroFilme generoFilme,
        Filme filme,
        Sessao sessao
    
    )
    {
        Titulo = titulo;
        Genero = generoFilme;
        Filme = filme;
        Sessao = sessao;
      
    }

    public override void AtualizarRegistro(Teste registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Genero = registroEditado.Genero;
        Filme = registroEditado.Filme;
        Sessao = registroEditado.Sessao;
        
    }
}