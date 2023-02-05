using CitizenDemo.ResourceAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.ResourceAPI.Data
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly ResourceContext _context;

        public ResourceRepository(IDatabaseSettings settings)
        {
            _context = new ResourceContext(settings);
        }

        public async Task<IEnumerable<Resource>> GetResources()
        {
            return await _context.Resources.Find(_ => true).Limit(100).ToListAsync();
        }

        public async Task<Resource> GetResource(string resourceId)
        {
            var resourceIdFilter = Builders<Resource>.Filter.Eq(r => r.ResourceId, resourceId);

            return await _context.Resources.Find(resourceIdFilter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesByCitizenId(string citizenId)
        {
            var citizenIdFilter = Builders<Resource>.Filter.Eq(r => r.CitizenId, citizenId);

            return await _context.Resources.Find(citizenIdFilter).ToListAsync();
        }

        public async Task AddResource(Resource resource)
        {
            var resourceIdFilter = Builders<Resource>.Filter.Eq(r => r.ResourceId, resource.ResourceId);

            var result = await _context.Resources.Find(resourceIdFilter).FirstOrDefaultAsync();

            if (result == null)
            {
                await _context.Resources.InsertOneAsync(resource);
            }
            else
                throw new DuplicateNameException();
        }

        public async Task<bool> RemoveResource(string resourceId)
        {
            var resourceIdFilter = Builders<Resource>.Filter.Eq(r => r.ResourceId, resourceId);

            DeleteResult actionResult = await _context.Resources.DeleteOneAsync(resourceIdFilter);

            return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        public async Task<bool> ReplaceResource(string resourceId, Resource resource)
        {
            var resourceIdFilter = Builders<Resource>.Filter.Eq(r => r.ResourceId, resourceId);

            ReplaceOneResult actionResult = await _context.Resources.ReplaceOneAsync(resourceIdFilter, resource);

            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }
    }
}