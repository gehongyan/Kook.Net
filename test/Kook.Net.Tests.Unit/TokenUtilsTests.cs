using System;
using Xunit;

namespace Kook;

/// <summary>
///     Tests for the <see cref="Kook.TokenUtils"/> methods.
/// </summary>
[Trait("Category", "Unit")]
public class TokenUtilsTests
{
    /// <summary>
    ///     Tests the usage of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
    ///     to see that when a null, empty or whitespace-only string is passed as the token,
    ///     it will throw an ArgumentNullException.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")] // string.Empty isn't a constant type
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("\t")]
    public void NullOrWhitespaceToken(string? token)
    {
        // an ArgumentNullException should be thrown, regardless of the TokenType
        foreach (TokenType tokenType in (TokenType[])Enum.GetValues(typeof(TokenType)))
            Assert.Throws<ArgumentNullException>(() => TokenUtils.ValidateToken(tokenType, token!));
    }

    /// <summary>
    ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
    ///     to see that valid Bot tokens do not throw Exceptions.
    ///     Valid Bot tokens can be strings of length 33 or above.
    /// </summary>
    [Theory]
    // missing two single characters from the end, 33 char. still should be valid
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw")]
    // normal valid token has 35 characters with two euqal characters at the end
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    // sometimes that token contains more than 2 slashes, so we test that as well
    [InlineData("1/MTEwMjQ=/Cw+A+pydai/97FPSOV7jEQ==")]
    // more than 35 characters
    [InlineData("1/MTEwMjQ=/Cw+A+pydai/97FPSOV7jEQ==========")]
    public void BotTokenDoesNotThrowExceptions(string token) => TokenUtils.ValidateToken(TokenType.Bot, token);

    /// <summary>
    ///     Tests the usage of <see cref="TokenUtils.ValidateToken(TokenType, string)"/> with
    ///     a Bot token that is invalid.
    /// </summary>
    [Theory]
    [InlineData("This is invalid")]
    // TODO: needs bearer token example
    // client ID
    [InlineData("1kOLIN9Xei67aafG")]
    // client secret
    [InlineData("LR3Bk68Xn0EvarU8")]
    // 32 char bot token
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeoj")]
    // ends with invalid characters
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw== ")]
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==\n")]
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==\t")]
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==\r\n")]
    // starts with invalid characters
    [InlineData(" 1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    [InlineData("\n1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    [InlineData("\t1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    [InlineData("\r\n1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    [InlineData("This is an invalid token, but it passes the check for string length.")]
    // valid token, but passed in twice
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==")]
    public void BotTokenInvalidThrowsArgumentException(string token) =>
        Assert.Throws<ArgumentException>(() => TokenUtils.ValidateToken(TokenType.Bot, token));

    /// <summary>
    ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
    ///     to see that an <see cref="ArgumentException"/> is thrown when an invalid
    ///     <see cref="TokenType"/> is supplied as a parameter.
    /// </summary>
    [Theory]
    // out of range TokenType
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(7)]
    public void UnrecognizedTokenType(int type) =>
        Assert.Throws<ArgumentException>(() =>
            TokenUtils.ValidateToken((TokenType)type, "1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw=="));

    /// <summary>
    ///     Checks the <see cref="TokenUtils.CheckBotTokenValidity(string)"/> method for expected output.
    /// </summary>
    /// <param name="token"> The Bot Token to test.</param>
    /// <param name="expected"> The expected result. </param>
    [Theory]
    // this method only checks the second part of the token
    [InlineData("/MTEzNzc=/", true)]
    [InlineData("1/MTEzNzc=/CG6PYkIyTgidPRRR7Oeojw==", true)]
    [InlineData("this part is invalid/MTEzNzc=/this part is also invalid", true)]
    [InlineData("/MTEzNzc=", false)]
    [InlineData("MTEzNzc=", false)]
    [InlineData("xxxx/MTEzNzc=/xxxxx", true)]
    // should not throw an unexpected exception
    [InlineData("", false)]
    [InlineData(null, false)]
    public void CheckBotTokenValidity(string? token, bool expected) =>
        Assert.Equal(expected, TokenUtils.CheckBotTokenValidity(token!));

    [Theory]
    // cannot pass a ulong? as a param in InlineData, so have to have a separate param
    // indicating if a value is null
    [InlineData("NDI4NDc3OTQ0MDA5MTk1NTIw", false, 428477944009195520)]
    [InlineData("MTEzNzc=", false, 11377)]
    // should return null w/o throwing other exceptions
    [InlineData("", true, 0)]
    [InlineData(" ", true, 0)]
    [InlineData(null, true, 0)]
    [InlineData("these chars aren't allowed @U#)*@#!)*", true, 0)]
    public void DecodeBase64UserId(string? encodedUserId, bool isNull, ulong expectedUserId)
    {
        ulong? result = TokenUtils.DecodeBase64AsNumber(encodedUserId!);
        if (isNull)
            Assert.Null(result);
        else
            Assert.Equal(expectedUserId, result);
    }
}
