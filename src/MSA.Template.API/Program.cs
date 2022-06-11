using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MSA.Template.API.Middlewares;
using MSA.Template.API.Services;
using MSA.Template.Core;
using MSA.Template.Infrastructure;
using MSA.Template.Infrastructure.AmazonSQS;
using MSA.Template.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddControllers();
builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().AddSwaggerGenNewtonsoftSupport();

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule());
});

builder.Services.AddMasstransitUsingAmazonSqs(builder.Configuration, builder.Environment);

builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(nameof(MasterDbContext)));
});

builder.Services.AddDbContext<SlaveDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(nameof(SlaveDbContext)));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork>(p => p.GetRequiredService<MasterDbContext>());
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<IIdentityServiceProvider>(p => p.GetRequiredService<IdentityService>());
builder.Services.AddScoped<IIdentityService>(p => p.GetRequiredService<IdentityService>());

var app = builder.Build();

app.UseMiddleware<IdentityMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapGet("/", () => "");
app.MapControllers();
app.Services.GetRequiredService<MasterDbContext>().Database.Migrate();
app.Run();