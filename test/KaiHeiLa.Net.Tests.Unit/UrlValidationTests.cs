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
    
    [Theory]
    [InlineData("http://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")]
    [InlineData("https://img.kookapp.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")]
    [InlineData("http://kaiheila.oss-cn-beijing.aliyuncs.com/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")]
    [InlineData("https://img.kookapp.cn/attachments/2022-08/19/GHUrXywopm1hc0u0.png")]
    public void UrlValidation_ValidAssetUrl(string url)
    {
        Assert.True(UrlValidation.ValidateKaiHeiLaAssetUrl(url));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UrlValidation_EmptyAssetUrl(string url)
    {
        Assert.False(UrlValidation.ValidateKaiHeiLaAssetUrl(url));
    }
    
    [Theory]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    [InlineData("https://img.kookapp.cn/attachments/2022-08/19/")]
    [InlineData("img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")]
    [InlineData("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku")]
    [InlineData("https://kaiheila.net/guides/getting_started/images/install/install-vs-dependencies.png")]
    public void UrlValidation_InvalidAssetUrl(string url)
    {
        Assert.Throws<InvalidOperationException>(() => UrlValidation.ValidateKaiHeiLaAssetUrl(url));
    }
}