using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client configuration.
/// </summary>
public class KookWebhookConfig : KookSocketConfig
{
    /// <summary>
    ///     Gets or sets the token type used to authenticate with the KOOK API.
    /// </summary>
    public TokenType? TokenType { get; set; }

    /// <summary>
    ///     Gets or sets the token used to authenticate with the KOOK API.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    ///     Gets or sets the verification token used to verify the webhook request.
    /// </summary>
    public string? VerifyToken { get; set; }

    /// <summary>
    ///     Gets or sets the encryption key used to decrypt the webhook payload.
    /// </summary>
    public string? EncryptKey { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the token should be validated before logging in.
    /// </summary>
    public bool ValidateToken { get; set; } = true;

    /// <summary>
    ///     Gets or sets a function that is called when the webhook client is being configured.
    /// </summary>
    public Action<IServiceProvider, KookWebhookClient>? ConfigureKookClient { get; set; }
}
