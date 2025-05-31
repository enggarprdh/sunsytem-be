using _5unSystem.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>();
// Add services to the container.


var allOrigins = "allowOrigins";
builder.Services.AddCors(opt => opt.AddPolicy(allOrigins,
                                            policy =>
                                            {
                                                policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                                            }));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var generatorDB = new GeneratorDB();
generatorDB.EnsureDatabaseCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(allOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
