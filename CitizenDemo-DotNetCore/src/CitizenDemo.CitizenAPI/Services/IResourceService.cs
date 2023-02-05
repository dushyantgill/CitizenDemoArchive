using CitizenDemo.CitizenAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.CitizenAPI.Services
{
    public interface IResourceService
    {
        Task ProvisionDefaultResource(Citizen citizen);
        Task DeprovisionAllResources(Citizen citizen);
    }
}
