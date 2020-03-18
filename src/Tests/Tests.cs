using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EntityFramework.FluentValidation;
using Microsoft.EntityFrameworkCore;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
{
    #region InValid

    [Fact]
    public async Task InValid()
    {
        var options = DbContextOptions();

        await using var context = new SampleDbContext(options);
        context.Add(new Employee {Content = ""});
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => context.SaveChangesAsync());
        await Verify(exception);
    }

    #endregion

    [Fact]
    public async Task InValid_Multiple()
    {
        var options = DbContextOptions();

        await using var context = new SampleDbContext(options);
        context.Add(new Employee {Content = ""});
        context.Add(new Company {Content = ""});
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => context.SaveChangesAsync());
        await Verify(exception);
    }
    [Fact]
    public async Task Valid()
    {
        var options = DbContextOptions();

        await using var context = new SampleDbContext(options);
        context.Add(new Employee {Content = "a"});
        await context.SaveChangesAsync();
    }

    static DbContextOptions<SampleDbContext> DbContextOptions(
        [CallerMemberName] string databaseName = "")
    {
        return new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}