using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq; // Necesario para construir el XML

public class CountryInfoService : ICountryInfoService
{
    // Cliente HTTP estático para reutilizarlo y optimizar recursos
    private static readonly HttpClient client = new HttpClient();

    // Implementación del método SOAP. 
    // Lo hacemos síncrono para WCF, pero llamamos a una versión async interna.
    public string GetCountryInfo(string countryCode)
    {
        // En WCF tradicional (y CoreWCF síncrono), se usa GetAwaiter().GetResult()
        return GetCountryInfoAsync(countryCode).GetAwaiter().GetResult();
    }

    private async Task<string> GetCountryInfoAsync(string countryCode)
    {
        try
        {
            var url = $"https://restcountries.com/v3.1/alpha/{countryCode}";
            var response = await client.GetAsync(url);
            
            // Lanza una excepción si el código de estado no es de éxito (4xx o 5xx)
            response.EnsureSuccessStatusCode(); 

            var json = await response.Content.ReadAsStringAsync();
            
            // La API de REST Countries devuelve un array, aunque sea un solo país
            JArray jsonArray = JArray.Parse(json);
            if (jsonArray.Count == 0)
            {
                 return $"<error>Country code {countryCode} not found.</error>";
            }

            // Usamos dynamic para acceder fácilmente a las propiedades JSON
            dynamic data = jsonArray[0];
            
            string name = data.name.common;
            string capital = data.capital[0];
            // La API de capital es un array, tomamos el primer elemento
            string capitalName = data.capital[0]; 
            long population = data.population;
            
            // Transformación de JSON a XML (Resultado esperado)
            return $"<country><name>{name}</name><capital>{capitalName}</capital><population>{population}</population></country>";
        }
        catch (HttpRequestException ex)
        {
             // Manejo de errores REST (ej. 404 Not Found de la API REST)
             return $"<error>Error en API REST: {ex.Message}</error>";
        }
        catch (Exception ex)
        {
            // Manejo de errores de parseo u otros
            return $"<error>Error interno: {ex.Message}</error>";
        }
    }
}