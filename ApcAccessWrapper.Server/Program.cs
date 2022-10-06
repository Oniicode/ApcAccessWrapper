using ApcAccessWrapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IApcAccessBinaryWrapper, ApcAccessBinaryWrapper>();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();