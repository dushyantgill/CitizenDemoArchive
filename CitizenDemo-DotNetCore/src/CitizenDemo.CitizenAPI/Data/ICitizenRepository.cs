using CitizenDemo.CitizenAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.CitizenAPI.Data
{
    public interface ICitizenRepository
    {
        Task<IEnumerable<Citizen>> GetCitizens();
        Task<Citizen> GetCitizen(string citizenId);
        Task<IEnumerable<Citizen>> SearchCitizens(string name, string postalCode, string city, string state, string country);
        Task AddCitizen(Citizen citizen);
        Task<bool> RemoveCitizen(string citizenId);
        Task<bool> ReplaceCitizen(string citizenId, Citizen citizen);
    }
}