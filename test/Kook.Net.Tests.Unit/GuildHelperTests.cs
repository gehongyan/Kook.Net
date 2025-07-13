using Kook.Rest;
using System;
using NSubstitute;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class GuildHelperTests
{
    [Theory]
    [InlineData(BoostLevel.None, 64)]
    [InlineData(BoostLevel.Level1, 128)]
    [InlineData(BoostLevel.Level2, 192)]
    [InlineData(BoostLevel.Level3, 256)]
    [InlineData(BoostLevel.Level4, 256)]
    [InlineData(BoostLevel.Level5, 320)]
    [InlineData(BoostLevel.Level6, 320)]
    public void GetMaxBitrate(BoostLevel level, int factor)
    {
        var guild = Substitute.For<IGuild>();
        guild.BoostLevel.Returns(level);
        int expected = factor * 1000;

        int actual = GuildHelper.GetMaxBitrate(guild);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(BoostLevel.None, 5)]
    [InlineData(BoostLevel.Level1, 20)]
    [InlineData(BoostLevel.Level2, 50)]
    [InlineData(BoostLevel.Level3, 100)]
    [InlineData(BoostLevel.Level4, 150)]
    [InlineData(BoostLevel.Level5, 200)]
    [InlineData(BoostLevel.Level6, 300)]
    public void GetUploadLimit(BoostLevel level, ulong factor)
    {
        var guild = Substitute.For<IGuild>();
        guild.BoostLevel.Returns(level);
        ulong expected = factor * (ulong)Math.Pow(2, 20);

        ulong actual = GuildHelper.GetUploadLimit(guild);

        Assert.Equal(expected, actual);
    }
}
