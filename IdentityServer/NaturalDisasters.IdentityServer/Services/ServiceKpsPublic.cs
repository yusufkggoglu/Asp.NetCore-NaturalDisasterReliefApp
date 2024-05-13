using IdentityServer4.Models;
using KpsPublic;
using System.Threading.Tasks;

namespace NaturalDisasters.IdentityServer.Services
{
    public class ServiceKpsPublic
    {
        public async Task<bool> OnGetService(long tcKimlik, string name,string surname,int dogumYili)
        {
            var client = new KpsPublic.KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);
            var response = await client.TCKimlikNoDogrulaAsync(tcKimlik, name, surname, dogumYili);

            return response.Body.TCKimlikNoDogrulaResult;

        }
    }
}
