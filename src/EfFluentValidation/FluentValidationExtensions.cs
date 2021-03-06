using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace EfFluentValidation
{
    /// <summary>
    /// Extensions to FluentValidation.
    /// </summary>
    public static class FluentValidationExtensions
    {
        public static T DbContext<T>(this CustomContext context)
            where T : DbContext
        {
            Guard.AgainstNull(context, nameof(context));
            return context.ParentContext.DbContext<T>();
        }

        public static T DbContext<T>(this IValidationContext context)
            where T : DbContext
        {
            Guard.AgainstNull(context, nameof(context));
            return (T) context.EfContext().DbContext;
        }

        public static EfContext EfContext(this CustomContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            return context.ParentContext.EfContext();
        }

        public static EfContext EfContext(this IValidationContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            return (EfContext) context.RootContextData["EfContext"];
        }
    }
}