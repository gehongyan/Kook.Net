using System;
using KaiHeiLa.Utils;
using Xunit;

namespace KaiHeiLa;

public class UrlValidationTests
{
    [Theory]
    [InlineData("http://kaiheila.net")]
    [InlineData("https://kaiheila.net")]
    public void UrlValidation_ValidUrl(string url)
    {
        Assert.True(UrlValidation.Validate(url));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UrlValidation_EmptyUrl(string url)
    {
        Assert.False(UrlValidation.Validate(url));
    }
    
    [Theory]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void UrlValidation_InvalidUrl(string url)
    {
        Assert.Throws<InvalidOperationException>(() => UrlValidation.Validate(url));
    }
    
    
}