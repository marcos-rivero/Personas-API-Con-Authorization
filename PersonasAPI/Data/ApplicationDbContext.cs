using Microsoft.EntityFrameworkCore;
using PersonasAPI.Modelo;

namespace PersonasAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Personas> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
