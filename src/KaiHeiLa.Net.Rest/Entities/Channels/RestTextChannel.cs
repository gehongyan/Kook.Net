using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based channel in a guild that can send and receive messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
internal class RestTextChannel : RestGuildChannel, IRestMessageChannel, ITextChannel
{
    #region RestTextChannel
    
    /// <inheritdoc />
    public string Topic { get; private set; }
    /// <inheritdoc />
    public virtual int SlowModeInterval { get; private set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <inheritdoc />
    public bool IsPermissionSynced { get; set; }
    
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    
    internal RestTextChannel(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id)
        : base(kaiHeiLa, guild, id, ChannelType.Text)
    {
    }
    
    internal new static RestTextChannel Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        var entity = new RestTextChannel(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode;
        IsPermissionSynced = model.PermissionSync == 1;
    }
    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null)
        => ChannelHelper.GetMessageAsync(this, KaiHeiLa, id, options);
    
    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Text)";

    #region IMessageChannel
    
    /// <inheritdoc />
    async Task<IMessage> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetMessageAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    
    #endregion
}