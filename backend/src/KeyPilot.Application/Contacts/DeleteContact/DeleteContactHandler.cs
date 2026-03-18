using KeyPilot.Application.Abstractions.Persistence;
using MediatR;

namespace KeyPilot.Application.Contacts.DeleteContact;

public sealed class DeleteContactHandler(
    IApplicationDbContext dbContext) : IRequestHandler<DeleteContactCommand, bool>
{
    public async Task<bool> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await dbContext.GetContactByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (contact is null)
        {
            return false;
        }

        dbContext.RemoveContact(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
