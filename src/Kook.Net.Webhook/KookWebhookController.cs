using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Gateway;
using Kook.API.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Kook.Webhook;

/// <summary>
///     Represents a controller for handling Kook webhook requests.
/// </summary>
[ApiController]
public class KookWebhookController : ControllerBase
{
    private static readonly JsonElement EmptyJsonElement = JsonDocument.Parse("{}").RootElement;
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
    [Route("kook")]
    public async Task Index()
    {
        using StreamReader streamReader = new(Request.Body);
        if (Request.Query.TryGetValue("compress", out StringValues compressValues)
            && compressValues.Any(x => x?.StartsWith("0") is true))
        {
            string requestBody = await streamReader.ReadToEndAsync();
            await OnTextMessage(requestBody);
        }
        else
        {
            using MemoryStream stream = new();
            await Request.Body.CopyToAsync(stream);
            byte[] bytes = stream.ToArray();
            await OnBinaryMessage(bytes, 0, (int?)Request.ContentLength ?? bytes.Length);
        }
    }

    private async Task OnBinaryMessage(byte[] data, int index, int count)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        await using MemoryStream decompressed = new();
#else
        using MemoryStream decompressed = new();
#endif
        using MemoryStream compressed = data[0] == 0x78
            ? new MemoryStream(data, index + 2, count - 2)
            : new MemoryStream(data, index, count);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        await using DeflateStream decompressor = new(compressed, CompressionMode.Decompress);
#else
        using DeflateStream decompressor = new(compressed, CompressionMode.Decompress);
#endif
        await decompressor.CopyToAsync(decompressed);
        decompressed.Position = 0;

        GatewayEncryptedFrame? gatewayEncryptedFrame = JsonSerializer.Deserialize<GatewayEncryptedFrame>(decompressed, _serializerOptions);
        if (gatewayEncryptedFrame is null)
            return;
        string decryptedFrame = Decrypt(gatewayEncryptedFrame.Encrypted, _kookWebhookClient.EncryptKey);
        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(decryptedFrame, _serializerOptions);
        if (gatewaySocketFrame is null)
            return;
        JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;
        string? response = await _kookWebhookClient
            .ProcessMessageAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
            .ConfigureAwait(false);
        if (response is not null)
            await Response.WriteAsync(response).ConfigureAwait(false);
    }

    private async Task OnTextMessage(string message)
    {
        GatewayEncryptedFrame? gatewayEncryptedFrame = JsonSerializer.Deserialize<GatewayEncryptedFrame>(message, _serializerOptions);
        if (gatewayEncryptedFrame is null)
            return;
        string decryptedFrame = Decrypt(gatewayEncryptedFrame.Encrypted, _kookWebhookClient.EncryptKey);
        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(decryptedFrame, _serializerOptions);
        if (gatewaySocketFrame is null)
            return;
        JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;
        string? response = await _kookWebhookClient
            .ProcessMessageAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
            .ConfigureAwait(false);
        if (response is not null)
            await Response.WriteAsync(response).ConfigureAwait(false);
    }

    private static string Decrypt(string data, string encryptKey)
    {
        string padEncryptKey = encryptKey.PadRight(32, '\0');

        string originCipher = Encoding.UTF8.GetString(Convert.FromBase64String(data));
        string iv = originCipher[..16];
        string newCipher = originCipher[16..];
        byte[] newCipherByte = Convert.FromBase64String(newCipher);

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(padEncryptKey);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new(newCipherByte);
        using CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        using StreamReader reader = new(cryptoStream);
        return reader.ReadToEnd();
    }
}
