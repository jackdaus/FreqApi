using Microsoft.EntityFrameworkCore;

namespace FreqApi.Models
{
    public class FreqContext : DbContext
    {
        public FreqContext(DbContextOptions<FreqContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
    }
}
