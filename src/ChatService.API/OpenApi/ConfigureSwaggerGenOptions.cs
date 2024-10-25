using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChatService.API.OpenApi;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"AutoPark.Chat.API v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
            };

            options.SwaggerDoc(description.GroupName, openApiInfo);
        }

        var tokenUri = new Uri(configuration.GetRequiredSection("Identity:Url").Value + "/connect/token");

        options.AddSecurityDefinition(
            "OAuth2",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        RefreshUrl = tokenUri,
                        TokenUrl = tokenUri,
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" },
                            { "autopark", "autopark " },
                            { "offline_access", "offline_access " },
                        },
                    },
                },
            });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "OAuth2",
                        },
                        Name = "Bearer",
                        Scheme = "Bearer",
                    },
                    new string[] { }
                },
            });
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}
