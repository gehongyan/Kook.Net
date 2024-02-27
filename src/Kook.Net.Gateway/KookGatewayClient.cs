namespace Kook.Gateway;

public partial class KookGatewayClient : BaseGatewayClient, IKookClient
{
    #region KookGatewayClient



    #endregion

    // From KookGatewayConfig

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client.
    /// </summary>
    public KookGatewayClient() : this(new KookGatewayConfig())
    {
    }

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="config">The configuration to be used with the client.</param>
    public KookGatewayClient(KookGatewayConfig config) : this(config, CreateApiClient(config))
    {
    }

    private KookGatewayClient(KookGatewayConfig config, KookGatewayApiClient client)
        : base(config, client)
    {

    }
}
