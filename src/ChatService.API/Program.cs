using Asp.Versioning.ApiExplorer;
using ChatService.API.Extensions;
using ChatService.API.Hubs;
using ChatService.BLL.Extensions;
using ChatService.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDalServices(builder.Configuration);
builder.Services.AddBllServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

app.UseCors("DefaultCorsPolicy");

app.UseExceptionHandler();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

            foreach (var description in descriptions)
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();

                options.SwaggerEndpoint(url, name);
            }

            options.OAuthClientId("dev");
        });

    await app.InitializeDatabaseAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chat-hub");

app.MapControllers();

app.Run();

public partial class Program
{
}
