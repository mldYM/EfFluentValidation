using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

public class Company
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string? Content { get; set; }
    public List<Employee> Employees { get; set; } = null!;
    public class Validator :
        AbstractValidator<Company>
    {
        public Validator()
        {
            RuleFor(_ => _.Content)
                .NotEmpty();
        }
    }
}