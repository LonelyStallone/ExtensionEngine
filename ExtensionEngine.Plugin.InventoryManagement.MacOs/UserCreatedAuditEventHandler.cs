using ExtensionEngine.Plugin.InventoryManagement.MacOs.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs;

public class UserCreatedAuditEventHandler : INotificationHandler<UpdateInventoryEvent>
{
    private readonly ILogger<UserCreatedAuditEventHandler> _logger;

    public UserCreatedAuditEventHandler(ILogger<UserCreatedAuditEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UpdateInventoryEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{@notification}", notification);

        return Task.CompletedTask;
    }
}
