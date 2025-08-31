using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Teste.Integracao.ModuloGenero;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;


namespace ControleDeCinema.Teste.Integracao.Compartilhado
{
    public  class TestDbContextFactory
    {
        private readonly PostgreSqlContainer container;

        public TestDbContextFactory()
        {
            container = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithName("Controle-de-Cinema-testdb")
                .WithUsername("postgres")
                .WithPassword("12345")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InicializarAsync()
        {
            await container.StartAsync();
        }

        public async Task EncerrarAsync()
        {
            await container.StopAsync();
            await container.DisposeAsync();
        }

        public  ControleDeCinemaDbContext CriarDbContext()
        {
            var connectionString = container.GetConnectionString();

            var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
                .UseNpgsql(connectionString)
                .Options;      

            var dbcontext  = new ControleDeCinemaDbContext(options);

           ConfigurarDbContext(dbcontext);

            return dbcontext;
        }
        private static void ConfigurarDbContext(ControleDeCinemaDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            dbContext.Teste.RemoveRange(dbContext.Testes);
            dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
            dbContext.Filmes.RemoveRange(dbContext.Filmes);
            dbContext.Sessoes.RemoveRange(dbContext.Sessoes);

            dbContext.SaveChanges();
         }
        public static IConfiguration CriarConfiguracao()
        {
            var assembly = typeof(TestDbContextFactory).Assembly;

            return  new ConfigurationBuilder()
                .AddUserSecrets(assembly)
                .Build();
        }
    }
}
