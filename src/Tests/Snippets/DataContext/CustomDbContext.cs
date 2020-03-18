using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Custom
{
    #region CustomDbContext

    public class SampleDbContext :
        DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        private static Func<Type, IEnumerable<IValidator>> validatorFactory;

        static SampleDbContext()
        {
            validatorFactory = DefaultValidatorFactory<SampleDbContext>.Factory;
        }

        public SampleDbContext(DbContextOptions options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Employees)
                .WithOne(e => e.Company)
                .IsRequired();
            modelBuilder.Entity<Employee>();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbContextValidator.Validate(this, validatorFactory).GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DbContextValidator.Validate(this, validatorFactory);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

    #endregion
}