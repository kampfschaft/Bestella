using Microsoft.EntityFrameworkCore;
using Bestella.Models;

namespace Bestella.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<ParsedOrder> Orders => Set<ParsedOrder>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParsedOrder>().Property(o => o.Id).ValueGeneratedNever();
        }
    }
}
#pragma warning restore format
