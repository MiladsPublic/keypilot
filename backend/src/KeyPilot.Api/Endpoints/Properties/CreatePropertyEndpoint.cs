using FluentValidation;
using KeyPilot.Application.Properties.CreateProperty;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Properties;

public static class CreatePropertyEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateProperty")
            .WithSummary("Create a property workspace entry.")
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static async Task<Results<Created<CreatePropertyResponse>, ValidationProblem>> HandleAsync(
        CreatePropertyCommand command,
        IValidator<CreatePropertyCommand> validator,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/properties/{response.Id}", response);
    }
}
