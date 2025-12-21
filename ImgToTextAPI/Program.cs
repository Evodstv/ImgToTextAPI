using Microsoft.AspNetCore.Cors.Infrastructure;

using Microsoft.OpenApi;

using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});


// Добавление контроллеров

builder.Services.AddControllers();



// Регистрация сервиса OCR

builder.Services.AddScoped<ImgToTextService>();



// Swagger

builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo

    {

        Title = "ImgToText Web API",

        Version = "v1",

        Description = "API для распознавания текста на изображениях"

    });



    // Подключаем XML-документацию

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);

});



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


/*
if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}



app.UseHttpsRedirection();

app.UseCors();


app.UseAuthorization();



app.MapControllers();



app.Run();
 */

// Разрешаем Swagger работать везде, а не только в Development
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImgToText Web API v1");
    // Если хочешь открывать Swagger сразу по адресу http://ip:5000/ без /swagger
    c.RoutePrefix = string.Empty; 
});

// app.UseHttpsRedirection(); // На VPS с этим портом часто мешает, если нет SSL. Можно закомментировать для теста.
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
