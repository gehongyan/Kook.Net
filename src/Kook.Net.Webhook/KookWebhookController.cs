// using Kook.Net.Webhooks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Kook.Webhook;
//
// /// <summary>
// ///     Represents a controller for handling Kook webhook requests.
// /// </summary>
// [ApiController]
// public class KookWebhookController : ControllerBase
// {
//     private readonly IAspNetWebhookClient _aspNetWebhookClient;
//
//     /// <summary>
//     ///     Initializes a new instance of the <see cref="KookWebhookController"/> controller class.
//     /// </summary>
//     /// <param name="kookWebhookClient"> The Kook webhook client. </param>
//     public KookWebhookController(KookWebhookClient kookWebhookClient)
//     {
//         if (kookWebhookClient.ApiClient.WebhookClient is not IAspNetWebhookClient aspNetWebhookClient)
//             throw new InvalidOperationException("The Kook webhook client is not an AspNetWebhookClient.");
//         _aspNetWebhookClient = aspNetWebhookClient;
//     }
//
//     /// <summary>
//     ///     Handles a Kook webhook request.
//     /// </summary>
//     [HttpPost]
//     [Route("kook")]
//     public async Task Index()
//     {
//         string? response = await _aspNetWebhookClient.HandleKookHttpRequestAsync(Request);
//         if (response is not null)
//             await Response.WriteAsync(response);
//     }
// }
