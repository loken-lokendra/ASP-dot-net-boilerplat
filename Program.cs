using ASPDOTNETDEMO.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspDotNetDemo API v1");
        c.RoutePrefix = "swagger"; 
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
