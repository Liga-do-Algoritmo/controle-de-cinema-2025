using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ControleDeCinema.Aplicacao.ModuloFilme;

public class FilmeAppService
{
    private readonly ITenantProvider tenantProvider;
    private readonly IRepositorioFilme repositorioFilme;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<FilmeAppService> logger;

    public FilmeAppService(
        ITenantProvider tenantProvider,
        IRepositorioFilme repositorioFilme,
        IUnitOfWork unitOfWork,
        ILogger<FilmeAppService> logger
    )
    {
        this.tenantProvider = tenantProvider;
        this.repositorioFilme = repositorioFilme;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public Result Cadastrar(Filme filme)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(filme.Titulo))
            erros.Add("O Título é obrigatorio");

        if (filme.Duracao <= 0)
            erros.Add("A duração deve ser um número positivo");

        if (filme.Lancamento == null)
            erros.Add("O lançamento é obrigatorio");

        if (filme.Genero is null)
            erros.Add("O Gênero é obrigatorio");

        var registros = repositorioFilme.SelecionarRegistros();
        if (registros != null && registros.Any(i => i.Titulo.Equals(filme.Titulo)))
            erros.Add("Já existe um filme registrado com este título.");

        if (erros.Any())
            return Result.Fail(erros.Select(e => new Error(e)).ToList());

        try
        {
            filme.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();
            repositorioFilme.Cadastrar(filme);
            unitOfWork.Commit();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            logger.LogError(ex, "Ocorreu um erro durante o registro de {@Registro}.", filme);
            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }


    public Result Editar(Guid id, Filme FilmeEditado)
    {
        var registros = repositorioFilme.SelecionarRegistros();

        if (registros.Any(i => !i.Id.Equals(id) && i.Titulo.Equals(FilmeEditado.Titulo)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um filme registrado com este título."));

        try
        {
            repositorioFilme.Editar(id, FilmeEditado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição do registro {@Registro}.",
                FilmeEditado
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Excluir(Guid id)
    {
        try
        {
            repositorioFilme.Excluir(id);

            unitOfWork.Commit();

            return Result.Ok();

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Filme> SelecionarPorId(Guid id)
    {
        try
        {
            var registroSelecionado = repositorioFilme.SelecionarRegistroPorId(id);

            if (registroSelecionado is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            return Result.Ok(registroSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Filme>> SelecionarTodos()
    {
        try
        {
            var registros = repositorioFilme.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de registros."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
