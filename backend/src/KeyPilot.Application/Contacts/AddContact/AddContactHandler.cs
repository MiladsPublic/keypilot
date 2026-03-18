using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Contacts.AddContact;

public sealed class AddContactHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<AddContactCommand, ContactDto?>
{
    public async Task<ContactDto?> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.PropertyId, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var contact = property.AddContact(
            request.Role,
            request.Name,
            request.Email,
            request.Phone,
            dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ContactDto.FromContact(contact);
    }
}
