using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Teste.Integracao.ModuloGenero;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDeCinema.Teste.Integracao.Compartilhado
{
    public static class TestDbContextFactory
    {
        public static ControleDeCinemaDbContext CriarDbContext()
        {
            var configuracao = CriarConfiguracao();

            var connectionString = configuracao["SQL_CONNECTION_STRING"];

            var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
                .UseNpgsql(connectionString)
                .Options;      

            var dbcontext  = new ControleDeCinemaDbContext(options);

            dbcontext.Database.EnsureDeleted();
            dbcontext.Database.EnsureCreated();

            return dbcontext;
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
