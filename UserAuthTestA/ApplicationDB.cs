using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserAuthTestA
{
    public class ApplicationDB : IdentityDbContext
    {
        public ApplicationDB(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Model.Customer>().HasKey(customer => new { customer.ID, customer.Name, customer.BusinessName });
        }
        public DbSet<Model.Unit> Units { get; set; }
        public DbSet<Model.Customer> Customers { get; set; }
        public DbSet<Model.Driver> Drivers { get; set; }
    }
}
