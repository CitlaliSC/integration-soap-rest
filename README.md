# integration-soap-rest

Proyecto de ejemplo que expone un servicio SOAP (CoreWCF) que consulta la API REST Countries y devuelve información de país.

## Requisitos

- .NET SDK 9.0 (o compatible instalada en el sistema)
- (Opcional) Postman o curl para probar las llamadas SOAP

## Estructura relevante

- `CountrySoapHost/` — proyecto principal que contiene el host SOAP
  - `Program.cs` — configuración del host y mapeo de `/soap`
  - `ICountryInfoService.cs` — contrato del servicio
  - `CountryInfoService.cs` — implementación que consulta `restcountries.com`

## Ejecutar localmente

1. Abrir una terminal en la carpeta del proyecto:

```cmd
cd "integracionSOAP\integration-soap-rest\CountrySoapHost"
```

2. Ejecutar con el perfil HTTPS (recomendado):

```cmd
dotnet run --launch-profile "https"
```

- Con este perfil la aplicación escuchará en `https://localhost:7217` y `http://localhost:5121` (según `launchSettings.json`).
- Para ver el WSDL: `https://localhost:7217/soap?wsdl` (o `http://localhost:5121/soap?wsdl` si ejecutas el perfil `http`).

3. (Si usas HTTPS) confiar en el certificado de desarrollo si es necesario:

```cmd
dotnet dev-certs https --trust
```

## Probar con Postman

- URL (HTTP): `http://localhost:5121/soap`
- URL (HTTPS): `https://localhost:7217/soap`
- Método: `POST`
- Headers:
  - `Content-Type: text/xml; charset=utf-8`
  - `SOAPAction: "http://tempuri.org/ICountryInfoService/GetCountryInfo"`  (confirmar el `SOAPAction` exacto en el WSDL)

Cuerpo (raw XML):

```xml
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
  <soapenv:Header/>
  <soapenv:Body>
    <tem:GetCountryInfo>
      <tem:countryCode>US</tem:countryCode>
    </tem:GetCountryInfo>
  </soapenv:Body>
</soapenv:Envelope>
```

Notas de resolución de errores comunes

- Error `WRONG_VERSION_NUMBER` o `EPROTO`: significa que se intentó hacer TLS (https) contra un puerto que está sirviendo solo HTTP. Usa la URL y el puerto correctos: `https://localhost:7217` para HTTPS o `http://localhost:5121` para HTTP.
- Si el WSDL no carga en HTTPS porque el certificado no es confiable, ejecuta `dotnet dev-certs https --trust` o desactiva temporalmente la verificación SSL en Postman.
- Si recibes `415 Unsupported Media Type`, asegúrate de que `Content-Type` sea `text/xml; charset=utf-8`.
