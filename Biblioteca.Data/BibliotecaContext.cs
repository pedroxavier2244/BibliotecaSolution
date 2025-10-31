using Biblioteca.Domain; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Biblioteca.Data
{
    public class BibliotecaContext : DbContext 
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        private static readonly StreamWriter _logStream = new StreamWriter("efcore_log.txt", append: true);
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=BibliotecaDbMiniprojeto; Integrated Security=True;Pooling=False")
                    .LogTo(_logStream.WriteLine,
                        new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                        .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, FirstName = "William", LastName = "Shakespeare" },
                new Author { AuthorId = 2, FirstName = "Jane", LastName = "Austen" }
            );
        }
    }
}