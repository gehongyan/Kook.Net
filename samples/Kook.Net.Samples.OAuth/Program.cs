using System.Security.Claims;
using System.Text;
using AspNet.Security.OAuth.Kook;
using Kook;
using Kook.Rest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/signout";
    })
    .AddKook(options =>
    {
        options.ClientId = string.Empty;
        options.ClientSecret = string.Empty;
        // get_user_info is added by default
        options.Scope.Add("get_user_guilds");
        options.Events.OnCreatingTicket = async context =>
        {
            if (context.AccessToken is not { } accessToken)
                return;
            KookRestClient kookRestClient = new();
            await kookRestClient.LoginAsync(TokenType.Bearer, accessToken);
            RestSelfUser? currentUser = kookRestClient.CurrentUser;
            IReadOnlyCollection<RestGuild> guilds = await kookRestClient.GetGuildsAsync();
            IReadOnlyCollection<RestGuild> adminGuilds = await kookRestClient.GetAdminGuildsAsync();
        };
    });
WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.MapGet("/signin",
    () => Results.Challenge(new AuthenticationProperties { RedirectUri = "/" },
        [KookAuthenticationDefaults.AuthenticationScheme]));
app.MapGet("/signout",
    () => Results.SignOut(new AuthenticationProperties { RedirectUri = "/" },
        [CookieAuthenticationDefaults.AuthenticationScheme]));
app.MapGet("/",
    (HttpContext context) =>
    {
        if (context.User is { Identity.IsAuthenticated: true } claimsPrincipal)
        {
            StringBuilder contentBuilder = new();
            contentBuilder.AppendLine($"Hello, {claimsPrincipal.Identity.Name}!<br />");
            Dictionary<string, string> claims = claimsPrincipal.Claims.ToDictionary(x => x.Type, x => x.Value);
            if (claims.TryGetValue(ClaimTypes.NameIdentifier, out string? nameIdentifier))
                contentBuilder.AppendLine($"Identifier: {nameIdentifier}<br />");
            if (claims.TryGetValue(ClaimTypes.Name, out string? name))
                contentBuilder.AppendLine($"Name: {name}<br />");
            if (claims.TryGetValue(ClaimTypes.MobilePhone, out string? mobilePhone))
                contentBuilder.AppendLine($"Mobile phone: {mobilePhone}<br />");
            if (claims.TryGetValue(KookAuthenticationConstants.Claims.IdentifyNumber, out string? identifyNumber))
                contentBuilder.AppendLine($"Identify number: {identifyNumber}<br />");
            if (claims.TryGetValue(KookAuthenticationConstants.Claims.OperatingSystem, out string? operatingSystem))
                contentBuilder.AppendLine($"Operating system: {operatingSystem}<br />");
            if (claims.TryGetValue(KookAuthenticationConstants.Claims.AvatarUrl, out string? avatarUrl))
                contentBuilder.AppendLine($"Avatar URL: {avatarUrl}<br />");
            if (claims.TryGetValue(KookAuthenticationConstants.Claims.BannerUrl, out string? bannerUrl))
                contentBuilder.AppendLine($"Banner URL: {bannerUrl}<br />");
            if (claims.TryGetValue(KookAuthenticationConstants.Claims.IsMobileVerified, out string? isMobileVerified))
                contentBuilder.AppendLine($"Mobile verified: {isMobileVerified}<br />");
            contentBuilder.AppendLine("<a href=\"/signout\">Sign out</a>");
            return Results.Content(contentBuilder.ToString(), "text/html", Encoding.Default);
        }

        return Results.Content("<a href=\"/signin\">Sign in</a>", "text/html");
    });

await app.RunAsync();
