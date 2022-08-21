using KaiHeiLa.Rest;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace KaiHeiLa;

[Trait("Category", "Unit")]
public class GuildHelperTests
{
    [Theory]
    [InlineData(BoostLevel.None, 96)]
    [InlineData(BoostLevel.Level1, 128)]
    [InlineData(BoostLevel.Level2, 192)]
    [InlineData(BoostLevel.Level3, 256)]
    [InlineData(BoostLevel.Level4, 256)]
    [InlineData(BoostLevel.Level5, 320)]
    [InlineData(BoostLevel.Level6, 320)]
    public void GetMaxBitrate(BoostLevel level, int factor)
    {
        var guild = Mock.Of<IGuild>(g => g.BoostLevel == level);
        var expected = factor * 1000;

        var actual = GuildHelper.GetMaxBitrate(guild);

        actual.Should().Be(expected);
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
        var guild = Mock.Of<IGuild>(g => g.BoostLevel == level);
        var expected = factor * (ulong)Math.Pow(2, 20);

        var actual = GuildHelper.GetUploadLimit(guild);

        actual.Should().Be(expected);
    }
}
