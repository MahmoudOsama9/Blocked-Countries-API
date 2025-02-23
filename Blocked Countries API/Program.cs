using Blocked_Countries_API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<BlockedCountryService>();
builder.Services.AddSingleton<BlockedAttemptsLogService>();
builder.Services.AddHostedService<TemporaryBlockCleanupService>();

var configuration = builder.Configuration;
string apiKey = configuration["GeoLocation:ApiKey"];
string baseUrl = configuration["GeoLocation:BaseUrl"];
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders();
app.UseAuthorization();
app.MapControllers();
app.Run();