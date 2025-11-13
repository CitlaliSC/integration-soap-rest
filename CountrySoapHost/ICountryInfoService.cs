using CoreWCF;

[ServiceContract]
public interface ICountryInfoService
{
    [OperationContract]
    string GetCountryInfo(string countryCode);
}