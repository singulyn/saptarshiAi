using SaptariX.Platform.Identity;
using SaptariX.Platform.Organization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddIdentityPlatform();
builder.Services.AddOrganizationPlatform();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
