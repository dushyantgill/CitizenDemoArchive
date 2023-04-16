using CitizenDemo.LoadGenerator.Services;
using CitizenDemo.LoadGenerator.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CitizenDemo.LoadGenerator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IHttpClientFactory _clientFactory;
        private Int64 _executionCount;
        private CitizenServiceSettings _citizenServiceSettings;
        private SampleData _sampleData;

        public Worker(ILogger<Worker> logger, IHttpClientFactory clientFactory, CitizenServiceSettings citizenServiceSettings)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _citizenServiceSettings = citizenServiceSettings;
            _executionCount = 0;
            _sampleData = new SampleData();
            using (StreamReader r = new StreamReader("./SampleData/GivenNames.json"))
            {
                string json = r.ReadToEnd();
                _sampleData.GivenNames = JsonConvert.DeserializeObject<List<string>>(json);
            }
            using (StreamReader r = new StreamReader("./SampleData/Surnames.json"))
            {
                string json = r.ReadToEnd();
                _sampleData.Surnames = JsonConvert.DeserializeObject<List<string>>(json);
            }
            using (StreamReader r = new StreamReader("./SampleData/StreetNames.json"))
            {
                string json = r.ReadToEnd();
                _sampleData.StreetNames = JsonConvert.DeserializeObject<List<string>>(json);
            }
            using (StreamReader r = new StreamReader("./SampleData/Cities.json"))
            {
                string json = r.ReadToEnd();
                _sampleData.Cities = JsonConvert.DeserializeObject<List<CityData>>(json);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Sync Worker: running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // every 15 seconds, create 10 citizens
                    if (_executionCount % 15 == 0)
                    {
                        for (var count = 0; count < 10; count++)
                        {
                            var city = _sampleData.Cities?[new Random().Next(0, _sampleData.Cities.Count)];
                            var streetName = _sampleData.StreetNames?[new Random().Next(0, _sampleData.StreetNames.Count)];

                            var citizen = new Citizen()
                            {
                                CitizenId = Guid.NewGuid().ToString(),
                                GivenName = _sampleData.GivenNames?[new Random().Next(0, _sampleData.GivenNames.Count)],
                                Surname = _sampleData.Surnames?[new Random().Next(0, _sampleData.Surnames.Count)],
                                PhoneNumber = string.Format("({0}) {1}{2}{3} - {4}{5}{6}{7}", city?.AreaCode,
                                    new Random().Next(0, 9).ToString(), new Random().Next(0, 9).ToString(),
                                    new Random().Next(0, 9).ToString(), new Random().Next(0, 9).ToString(),
                                    new Random().Next(0, 9).ToString(), new Random().Next(0, 9).ToString(),
                                    new Random().Next(0, 9).ToString()),
                                StreetAddress = string.Format("{0}{1}{2} {3}", new Random().Next(0, 9).ToString(),
                                    new Random().Next(0, 9).ToString(), new Random().Next(0, 9).ToString(),
                                    _sampleData.StreetNames?[new Random().Next(0, _sampleData.StreetNames.Count)]),
                                City = city?.City,
                                State = city?.State,
                                PostalCode = city?.PostalCode,
                                Country = city?.Country
                            };
                            _ = CreateCitizen(citizen);
                        }
                    }

                    // every 5 seconds, execute 100 searches
                    if (_executionCount % 5 == 0)
                    {
                        for (var count = 0; count < 25; count++)
                        {
                            var givenName = _sampleData.GivenNames?[new Random().Next(0, _sampleData.GivenNames.Count)];
                            if (givenName is not null) _ = SearchCitizens(givenName, null, null, null, null);
                        }

                        for (var count = 0; count < 25; count++)
                        {
                            var surname = _sampleData.Surnames?[new Random().Next(0, _sampleData.Surnames.Count)];
                            if (surname is not null) _ = SearchCitizens(surname, null, null, null, null);
                        }

                        for (var count = 0; count < 25; count++)
                        {
                            var city = _sampleData.Cities?[new Random().Next(0, _sampleData.Cities.Count)];
                            if (city is not null) _ = SearchCitizens(null, city.PostalCode, null, null, null);
                        }

                        for (var count = 0; count < 25; count++)
                        {
                            var city = _sampleData.Cities?[new Random().Next(0, _sampleData.Cities.Count)];
                            if(city is not null) _ = SearchCitizens(null, null, city.City, city.State, city.Country);
                        }
                    }

                    // every minute, delete 15 citizens
                    if (_executionCount % 60 == 0)
                    {
                        for (var count = 0; count < 15; count++)
                        {
                            var citizens = await GetCitizens();
                            if(citizens is not null) 
                            {
                                var citizen = citizens[new Random().Next(0, citizens.Count)];
                                _ = DeleteCitizen(citizen.CitizenId);
                            }
                        }
                    }
                    _executionCount++;
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Sync Worker: encountered a problem during execution");
                }
            }
        }

        private async Task CreateCitizen(Citizen? citizen)
        {
            if (citizen is not null)
            {
                using (var httpClient = _clientFactory.CreateClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(citizen), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync(_citizenServiceSettings.Url, content);
                }

                _logger.LogInformation("Sync Worker: Created citizen {0}", citizen.ToString());
            }
        }

        private async Task SearchCitizens(string? name, string? postalCode, string? city, string? state, string? country)
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                string getUrl = String.Format(_citizenServiceSettings.Url +
                    "/search?name={0}&postalCode={1}&city={2}&state={3}&country={4}",
                    name, postalCode, city, state, country);
                await httpClient.GetAsync(getUrl);
            }

            _logger.LogInformation("Sync Worker: Searched citizens by name={0}&postalCode={1}&city={2}&state={3}&country={4}"
                , name, postalCode, city, state, country);
        }

        private async Task<List<Citizen>?> GetCitizens()
        {
            List<Citizen>? citizens = null;

            using (var httpClient = _clientFactory.CreateClient())
            {
                var getResponse = await httpClient.GetAsync(_citizenServiceSettings.Url);
                if (getResponse.IsSuccessStatusCode)
                {
                    string citizenResponse = await getResponse.Content.ReadAsStringAsync();
                    citizens = JsonConvert.DeserializeObject<List<Citizen>>(citizenResponse);
                }
            }

            _logger.LogInformation("Sync Worker: Retrieved citizens");
            return citizens;
        }

        private async Task DeleteCitizen(string? citizenId)
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                string deleteUrl = String.Format(_citizenServiceSettings.Url + "/{0}", citizenId);
                await httpClient.DeleteAsync(deleteUrl);
            }

            _logger.LogInformation("Sync Worker: Deleted citizen {0}", citizenId);
        }
    }
}
