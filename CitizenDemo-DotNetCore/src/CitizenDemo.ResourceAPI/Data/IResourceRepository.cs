using CitizenDemo.ResourceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.ResourceAPI.Data
{
    public interface IResourceRepository
    {
        Task<IEnumerable<Resource>> GetResources();
        Task<Resource> GetResource(string resourceId);
        Task<IEnumerable<Resource>> GetResourcesByCitizenId(string citizenId);
        Task AddResource(Resource resource);
        Task<bool> RemoveResource(string resourceId);
        Task<bool> ReplaceResource(string resourceId, Resource resource);
    }
}