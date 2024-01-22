using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Gateway;

using Microsoft.AspNetCore.Mvc;

namespace Kook.Webhook;

/// <summary>
///     Represents a controller for handling Kook webhook requests.
/// </summary>
[ApiController]
public class KookWebhookController : ControllerBase
{
    private readonly KookWebhookClient _kookWebhookClient;
    private readonly JsonSerializerOptions _serializerOptions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookWebhookController"/> controller class.
    /// </summary>
    /// <param name="kookWebhookClient"> The Kook webhook client. </param>
    public KookWebhookController(KookWebhookClient kookWebhookClient)
    {
        _kookWebhookClient = kookWebhookClient;
        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

    /// <summary>
    ///     Handles a Kook webhook request.
    /// </summary>
    [HttpPost]
    // TODO: Move to config
    [Route("[controller]/[action]")]
    public async Task Index()
    {
        using StreamReader streamReader = new(Request.Body);
        string requestBody = await streamReader.ReadToEndAsync();
        // TODO: Decrypt & chanllenge
        string message = requestBody;
        GatewaySocketFrame gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(message, _serializerOptions);
        await _kookWebhookClient.ProcessMessageAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, gatewaySocketFrame.Payload)
            .ConfigureAwait(false);
    }
}
