using FluentValidation;

public interface IProvideId
{
    int Id { get; set; }
    public class Validator : AbstractValidator<IProvideId>
    {
        public Validator()
        {
            RuleFor(_ => _.Id)
                .NotEqual(-1);
        }
    }
}