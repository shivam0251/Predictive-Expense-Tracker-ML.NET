// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ExpSem4App.Models;
using Microsoft.ML;
using System.Collections.Generic;

namespace ExpSem4App.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}