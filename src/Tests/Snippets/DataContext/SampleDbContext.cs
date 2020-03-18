using System;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.FluentValidation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class SampleDbContext :
    ValidatingDbContext
{
    static Func<Type, IEnumerable<IValidator>> validatorFactory;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;

    static SampleDbContext()
    {
        var validators = ValidationFinder.FindValidatorsInAssemblyContaining<SampleDbContext>();

        var typeCache = new ValidatorTypeCache(validators);
        validatorFactory = type =>
        {
            if (typeCache.TryGetValidators(type, out var enumerable))
            {
                return enumerable;
            }

            return Enumerable.Empty<IValidator>();
        };
    }

    public SampleDbContext(DbContextOptions options) :
        base(options,validatorFactory)
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