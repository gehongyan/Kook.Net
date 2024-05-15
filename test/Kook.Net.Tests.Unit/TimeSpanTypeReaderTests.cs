using Kook.Commands;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class TimeSpanTypeReaderTests
{
    [Theory]
    [InlineData("4d3h2m1s", false)] // tests format "%d'd'%h'h'%m'm'%s's'"
    [InlineData("4d3h2m", false)]   // tests format "%d'd'%h'h'%m'm'"
    [InlineData("4d3h1s", false)]   // tests format "%d'd'%h'h'%s's'"
    [InlineData("4d3h", false)]     // tests format "%d'd'%h'h'"
    [InlineData("4d2m1s", false)]   // tests format "%d'd'%m'm'%s's'"
    [InlineData("4d2m", false)]     // tests format "%d'd'%m'm'"
    [InlineData("4d1s", false)]     // tests format "%d'd'%s's'"
    [InlineData("4d", false)]       // tests format "%d'd'"
    [InlineData("3h2m1s", false)]   // tests format "%h'h'%m'm'%s's'"
    [InlineData("3h2m", false)]     // tests format "%h'h'%m'm'"
    [InlineData("3h1s", false)]     // tests format "%h'h'%s's'"
    [InlineData("3h", false)]       // tests format "%h'h'"
    [InlineData("2m1s", false)]     // tests format "%m'm'%s's'"
    [InlineData("2m", false)]       // tests format "%m'm'"
    [InlineData("1s", false)]       // tests format "%s's'"
    // Negatives
    [InlineData("-4d3h2m1s", true)] // tests format "-%d'd'%h'h'%m'm'%s's'"
    [InlineData("-4d3h2m", true)]   // tests format "-%d'd'%h'h'%m'm'"
    [InlineData("-4d3h1s", true)]   // tests format "-%d'd'%h'h'%s's'"
    [InlineData("-4d3h", true)]     // tests format "-%d'd'%h'h'"
    [InlineData("-4d2m1s", true)]   // tests format "-%d'd'%m'm'%s's'"
    [InlineData("-4d2m", true)]     // tests format "-%d'd'%m'm'"
    [InlineData("-4d1s", true)]     // tests format "-%d'd'%s's'"
    [InlineData("-4d", true)]       // tests format "-%d'd'"
    [InlineData("-3h2m1s", true)]   // tests format "-%h'h'%m'm'%s's'"
    [InlineData("-3h2m", true)]     // tests format "-%h'h'%m'm'"
    [InlineData("-3h1s", true)]     // tests format "-%h'h'%s's'"
    [InlineData("-3h", true)]       // tests format "-%h'h'"
    [InlineData("-2m1s", true)]     // tests format "-%m'm'%s's'"
    [InlineData("-2m", true)]       // tests format "-%m'm'"
    [InlineData("-1s", true)]       // tests format "-%s's'"
    public async Task TestTimeSpanParse(string input, bool isNegative)
    {
        TimeSpanTypeReader reader = new();
        TypeReaderResult result = await reader.ReadAsync(null!, input, null!);
        Assert.True(result.IsSuccess);

        TimeSpan? actual = (TimeSpan?)result.BestMatch;
        Assert.NotNull(actual);
        Assert.True(actual != TimeSpan.Zero);

        if (isNegative)
        {
            Assert.True(actual < TimeSpan.Zero);

            Assert.True(actual.Value.Seconds is 0 or -1);
            Assert.True(actual.Value.Minutes is 0 or -2);
            Assert.True(actual.Value.Hours is 0 or -3);
            Assert.True(actual.Value.Days is 0 or -4);
        }
        else
        {
            Assert.True(actual > TimeSpan.Zero);

            Assert.True(actual.Value.Seconds is 0 or 1);
            Assert.True(actual.Value.Minutes is 0 or 2);
            Assert.True(actual.Value.Hours is 0 or 3);
            Assert.True(actual.Value.Days is 0 or 4);
        }
    }
}
