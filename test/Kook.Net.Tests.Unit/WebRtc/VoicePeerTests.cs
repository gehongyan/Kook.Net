using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="VoicePeer"/>.
/// </summary>
[Trait("Category", "Unit")]
public class VoicePeerTests
{
    [Fact]
    public void CreateMapsTheFieldsOfThePeerInfo()
    {
        VoicePeerInfo info = new()
        {
            Id = "2810246202",
            PeerId = "2810246202",
            DisplayName = "someone",
            Device = "android",
            IsVip = true,
            PausedProducer = true,
            PausedHeadset = false,
            Accompaniment = false,
            IsServerMute = false,
            PermissionSay = true,
            PermissionFreeMic = true,
            PermissionVoiceManage = false
        };

        VoicePeer? peer = VoicePeer.Create(info);

        Assert.NotNull(peer);
        Assert.Equal(2810246202UL, peer.Id);
        Assert.Equal("someone", peer.DisplayName);
        Assert.Equal("android", peer.Device);
        Assert.True(peer.IsVip);
        Assert.True(peer.IsMicrophoneMuted);
        Assert.False(peer.IsHeadsetMuted);
        Assert.False(peer.IsPlayingAccompaniment);
        Assert.False(peer.Permissions.IsServerMuted);
        Assert.True(peer.Permissions.CanSpeak);
        Assert.True(peer.Permissions.CanFreeMic);
        Assert.False(peer.Permissions.CanManageVoice);
    }

    [Fact]
    public void CreateFallsBackToTheIdWhenThePeerIdIsMissing()
    {
        VoicePeer? peer = VoicePeer.Create(new VoicePeerInfo { Id = "821393881" });

        Assert.Equal(821393881UL, peer!.Id);
    }

    [Fact]
    public void FieldsThatTheServerDidNotSendStayNull()
    {
        VoicePeer? peer = VoicePeer.Create(new VoicePeerInfo { PeerId = "1" });

        // 服务端没有下发的字段不能被当成 false
        Assert.Null(peer!.IsVip);
        Assert.Null(peer.IsMicrophoneMuted);
        Assert.Null(peer.IsHeadsetMuted);
        Assert.Null(peer.Permissions.CanSpeak);
        Assert.Null(peer.Permissions.IsServerMuted);
        // 成员信息里没有这两个字段，只会出现在 peerPermissionChanged 通知里
        Assert.Null(peer.Permissions.IsServerHeadsetMuted);
        Assert.Null(peer.Permissions.CanLink);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a number")]
    [InlineData("-1")]
    public void CreateReturnsNullWhenTheUserIdIsMissingOrInvalid(string? peerId) =>
        Assert.Null(VoicePeer.Create(new VoicePeerInfo { PeerId = peerId, Id = peerId }));
}
