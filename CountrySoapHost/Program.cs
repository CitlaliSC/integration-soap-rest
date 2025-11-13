using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1. Configuración de Servicios
// ----------------------------------------------------
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata(); // <== habilita el WSDL
builder.Services.AddSingleton<ICountryInfoService, CountryInfoService>();

// Configuración de Swagger para el endpoint REST
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------------------------------------
// 2. Middleware REST
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// ----------------------------------------------------
// 3. Configuración del servicio SOAP
// ----------------------------------------------------
app.UseServiceModel(serviceBuilder =>
{
    // Servicio principal
    serviceBuilder.AddService<CountryInfoService>(serviceOptions =>
    {
        // Mostrar errores detallados durante desarrollo
        serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    // Endpoint SOAP
    serviceBuilder.AddServiceEndpoint<CountryInfoService, ICountryInfoService>(
        new BasicHttpBinding(),
        "/soap");

    // Habilitar Metadata (WSDL)
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

// ----------------------------------------------------
// 4. Endpoint REST simple
// ----------------------------------------------------
app.MapGet("/", () =>
    Results.Ok("Host funcionando. REST (Swagger) en /swagger/. SOAP en /soap?wsdl.")
);

// ----------------------------------------------------
// 5. Ejecutar
// ----------------------------------------------------
app.Run();
