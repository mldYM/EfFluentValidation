using System;
using System.Collections.Generic;
using EfFluentValidation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class SampleDbContext :
    ValidatingDbContext
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;

    public SampleDbContext(
        DbContextOptions options,
        Func<Type, IEnumerable<IValidator>> validatorFactory) :
        base(options, validatorFactory)
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
}