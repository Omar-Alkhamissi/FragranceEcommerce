using FragEcom.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FragEcom.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<Brands>? Brands { get; set; }
        public virtual DbSet<Products>? Products { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderLineItem>? OrderLineItems { get; set; }
        public virtual DbSet<Branch>? Branches { get; set; }
    }
}