using Kook.Rest;

namespace Kook.Gateway;

/// <summary>
///     Represents a base class for Kook.Net gateway client.
/// </summary>
public abstract partial class BaseGatewayClient : BaseKookClient, IKookClient
{
    #region BaseKookClient

    /// <inheritdoc />
    internal BaseGatewayClient(KookGatewayConfig config, API.KookRestApiClient client)
        : base(config, client)
    {
    }

    #endregion

    #region IKookClient



    #endregion
}
