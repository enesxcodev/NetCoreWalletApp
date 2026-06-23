using Application.Extension;
using Persistence.Extension;
using Scalar.AspNetCore;
using WebApi.Extension;
using Serilog; // 1. Burayı ekle

var builder = WebApplication.CreateBuilder(args);

// 1. Önce Temel API/OpenAPI Servislerini Ekle
builder.Services.AddOpenApi();

// 2. 🚀 ALTYAPI VE BAĞIMLILIKLARI DOĞRU SIRAYLA YÜKLE
builder.Services.AddContext(builder.Configuration);          // Önce Veritabanı (AppDbContext)
builder.Services.PersistenceRegisters(builder.Configuration); // Sonra Altyapı (MassTransit, IdentityCore, Repositories)
builder.Services.AddApplication(builder.Configuration);      // Sonra MediatR Handler'lar (MassTransit'i görebilsin diye)
builder.Services.AddApi();                                  // En son API katmanı bağımlılıkları

// 3. Controller'ları Ekle (Yukarıdaki tüm JWT şemalarını görerek mühürlesin)
builder.Services.AddControllers();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();

// 4. Middleware (İstek Hattı) Sıralaması
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

// 🎯 Kapıdaki Bekçiler (Sıralama Hayatidir)
app.UseAuthentication(); // 1. Önce Kimlik Doğrulama (Kimsin?)
app.UseAuthorization();  // 2. Sonra Yetkilendirme (İçeri girebilir misin?)
app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();