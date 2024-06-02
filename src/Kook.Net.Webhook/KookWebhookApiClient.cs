﻿using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Gateway;
using Kook.API.Webhook;
using Kook.Net.Rest;
using Kook.Net.Webhooks;
using Kook.Net.WebSockets;

namespace Kook.API;

internal class KookWebhookApiClient : KookSocketApiClient
{
    public event Func<string, Task> WebhookChallenge
    {
        add => _webhookChallenge.Add(value);
        remove => _webhookChallenge.Remove(value);
    }

    private readonly AsyncEvent<Func<string, Task>> _webhookChallenge = new();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly string _encryptKey;
    private readonly string _verifyToken;
    internal IWebhookClient WebhookClient { get; }

    public KookWebhookApiClient(RestClientProvider restClientProvider,
        WebSocketProvider webSocketProvider,
        WebhookProvider webhookProvider,
        string encryptKey, string verifyToken, string userAgent, string acceptLanguage,
        RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions? serializerOptions = null,
        Func<IRateLimitInfo, Task>? defaultRatelimitCallback = null)
        : base(restClientProvider, webSocketProvider, userAgent,
            acceptLanguage, null, defaultRetryMode, serializerOptions, defaultRatelimitCallback)
    {
        _encryptKey = encryptKey;
        _verifyToken = verifyToken;
        WebhookClient = webhookProvider();
        WebhookClient.BinaryMessage += OnBinaryMessage;
        WebhookClient.TextMessage += OnTextMessage;
    }

    private async Task<string?> OnBinaryMessage(byte[] data, int index, int count)
    {
        await using MemoryStream decompressed = new();
        using MemoryStream compressed = data[0] == 0x78
            ? new MemoryStream(data, index + 2, count - 2)
            : new MemoryStream(data, index, count);
        await using DeflateStream decompressor = new(compressed, CompressionMode.Decompress);
        await decompressor.CopyToAsync(decompressed);
        decompressed.Position = 0;

        GatewayEncryptedFrame? gatewayEncryptedFrame = JsonSerializer.Deserialize<GatewayEncryptedFrame>(decompressed, _serializerOptions);
        if (gatewayEncryptedFrame is null)
            return null;
        string decryptedFrame = Decrypt(gatewayEncryptedFrame.Encrypted, _encryptKey);
        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(decryptedFrame, _serializerOptions);
        if (gatewaySocketFrame is null)
            return null;
        JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;

        if (TryParseWebhookChallenge(gatewaySocketFrame.Type, payloadElement, out string? challenge))
        {
            await _webhookChallenge.InvokeAsync(challenge);
            return JsonSerializer.Serialize(new GatewayChallengeFrame { Challenge = challenge }, SerializerOptions);
        }

        await _receivedGatewayEvent
            .InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
            .ConfigureAwait(false);
        return null;
    }

    private async Task<string?> OnTextMessage(string message)
    {
        GatewayEncryptedFrame? gatewayEncryptedFrame = JsonSerializer.Deserialize<GatewayEncryptedFrame>(message, _serializerOptions);
        if (gatewayEncryptedFrame is null)
            return null;
        string decryptedFrame = Decrypt(gatewayEncryptedFrame.Encrypted, _encryptKey);
        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(decryptedFrame, _serializerOptions);
        if (gatewaySocketFrame is null)
            return null;
        JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;

        if (TryParseWebhookChallenge(gatewaySocketFrame.Type, payloadElement, out string? challenge))
        {
            await _webhookChallenge.InvokeAsync(challenge);
            return JsonSerializer.Serialize(new GatewayChallengeFrame { Challenge = challenge }, SerializerOptions);
        }

        await _receivedGatewayEvent
            .InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
            .ConfigureAwait(false);
        return null;
    }

    private bool TryParseWebhookChallenge(GatewaySocketFrameType gatewaySocketFrameType, JsonElement payload,
        [NotNullWhen(true)] out string? challenge)
    {
        if (gatewaySocketFrameType != GatewaySocketFrameType.Event
            || !payload.TryGetProperty("type", out JsonElement typeProperty)
            || !typeProperty.TryGetInt32(out int typeValue)
            || typeValue != 255
            || !payload.TryGetProperty("channel_type", out JsonElement channelTypeProperty)
            || channelTypeProperty.GetString() != "WEBHOOK_CHALLENGE")
        {
            challenge = null;
            return false;
        }

        if (!payload.TryGetProperty("verify_token", out JsonElement verifyTokenProperty)
            || verifyTokenProperty.GetString() is not { Length: > 0 } verifyTokenValue)
            throw new InvalidOperationException("Webhook challenge is missing the verify token.");
        if (verifyTokenValue != _verifyToken)
            throw new InvalidOperationException("Webhook challenge verify token does not match.");

        if (!payload.TryGetProperty("challenge", out JsonElement challengeProperty)
            || challengeProperty.GetString() is not { Length: > 0 } challengeValue)
            throw new InvalidOperationException("Webhook challenge is missing the challenge.");

        challenge = challengeValue;
        return true;
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
