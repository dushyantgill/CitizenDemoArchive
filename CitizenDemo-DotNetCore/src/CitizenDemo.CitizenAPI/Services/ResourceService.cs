using CitizenDemo.CitizenAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging.Configuration;

namespace CitizenDemo.CitizenAPI.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string? _url;

        public ResourceService(IHttpClientFactory clientFactory, IResourceServiceSettings resourceServiceSettings)
        {
            _clientFactory = clientFactory;
            _url = resourceServiceSettings.Url;
        }

        public async Task ProvisionDefaultResource(Citizen citizen)
        {
            var resource = new Resource()
            {
                CitizenId = citizen.CitizenId,
                Name = "Phone Authentication",
                Status = "To Be Assigned"
            };

            using (var httpClient = _clientFactory.CreateClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(resource), Encoding.UTF8, "application/json");
                await httpClient.PostAsync(_url, content);
            }
        }

        public async Task DeprovisionAllResources(Citizen citizen)
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                string getUrl = String.Format(_url + "/search?citizenId={0}", citizen.CitizenId);

                var getResponse = await httpClient.GetAsync(getUrl);
                if (getResponse.IsSuccessStatusCode)
                {
                    string resourcesResponse = await getResponse.Content.ReadAsStringAsync();
                    var resources = JsonConvert.DeserializeObject<List<Resource>>(resourcesResponse);

                    if (resources is not null)
                    {
                        foreach (var resource in resources)
                        {
                            string deleteUrl = String.Format(_url + "/{0}", resource.ResourceId);
                            await httpClient.DeleteAsync(deleteUrl);
                        }
                    }
                }
            }
        }
    }
}
