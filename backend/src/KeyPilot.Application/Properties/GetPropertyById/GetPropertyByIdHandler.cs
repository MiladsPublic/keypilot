using KeyPilot.Application.Abstractions.Persistence;
using MediatR;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        return property is null ? null : PropertyDto.FromProperty(property);
    }
}
