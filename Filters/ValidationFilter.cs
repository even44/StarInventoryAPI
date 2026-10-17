using System.ComponentModel.DataAnnotations;

namespace StarInventoryAPI.Filters;

// Validates the request body argument of type T using DataAnnotations (and IValidatableObject)
// and short-circuits with a 400 ValidationProblem when it is invalid.
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument == null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { typeof(T).Name, ["A request body is required."] }
            });
        }

        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(argument, new ValidationContext(argument), results, validateAllProperties: true))
        {
            var errors = results
                .SelectMany(r => r.MemberNames.DefaultIfEmpty(string.Empty), (r, member) => (member, message: r.ErrorMessage ?? "Invalid value."))
                .GroupBy(e => e.member)
                .ToDictionary(g => g.Key, g => g.Select(e => e.message).ToArray());
            return TypedResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}

public static class ValidationFilterExtentions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder
            .AddEndpointFilter<ValidationFilter<T>>()
            .ProducesValidationProblem();
    }
}
