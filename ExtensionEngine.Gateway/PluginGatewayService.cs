using ExtensionEngine.Abstractions.Utils;
using Grpc.Core;
using MediatR;
using PluginGateway.Proto;

namespace ExtensionEngine.Gateway;

public class PluginGatewayService : PluginGatewayFacade.PluginGatewayFacadeBase
{
    private readonly ILogger<PluginGatewayService> _logger;

    private readonly IMediator _mediator;

    public PluginGatewayService(
        IMediator mediator,
        ILogger<PluginGatewayService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<PublishResponse> Publish(PublishRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received ProduceInventoryData request with {Count} inventory items", request.Messages.Count);


        foreach (var message in request.Messages)
        {
            var payload = EnvelopePayloadSerializer.Deserialize(message.Payload.PayloadType, message.Payload.Data);
            await _mediator.Publish(request, context.CancellationToken);
        }

        return new PublishResponse();
    }
}