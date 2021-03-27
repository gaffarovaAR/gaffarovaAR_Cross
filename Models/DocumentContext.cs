using Microsoft.EntityFrameworkCore;

namespace GaffarovaAlbina.Models
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options)
            : base(options)
        {
            Database.Migrate();
            foreach (var ass in Assignments)
                Entry(ass).Collection(_ => _.Executors).Load();
            foreach (var ass in Assignments)
                Entry(ass).Collection(_ => _.History).Load();
        }

        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Executor> Executors { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
    }
}
