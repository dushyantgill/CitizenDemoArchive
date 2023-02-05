using CitizenDemo.ResourceAPI.Data;
using CitizenDemo.ResourceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CitizenDemo.ResourceAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly ILogger _logger;

        public ResourcesController(IResourceRepository resourceRepository, ILogger<ResourcesController> logger)
        {
            _resourceRepository = resourceRepository;
            _logger = logger;
        }

        // GET /resources/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _resourceRepository.GetResources();
                _logger.LogDebug("GetResources called");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetResources failed.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // GET /resources/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpGet("{resourceId}")]
        public async Task<ActionResult<Resource>> Get(string resourceId)
        {
            try
            {
                var result = await _resourceRepository.GetResource(resourceId);
                if (result == null) return NotFound(); 
                _logger.LogDebug("GetResource called with param {0}", resourceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetResource failed with param {0}", resourceId);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // GET /resources/search?citizen-id=739ad8de-7b3b-45c1-a90c-697ef16317ce
        [HttpGet("search")]
        public async Task<ActionResult<Resource[]>> Search(string citizenId)
        {
            try
            {
                var results = await _resourceRepository.GetResourcesByCitizenId(citizenId);
                _logger.LogDebug("SearchResources called with params {0}", new string[] { citizenId });
                if (!results.Any()) return NotFound();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SearchResources failed with params {0}", new string[] { citizenId });
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // POST /resources/
        [HttpPost]
        public async Task<ActionResult<Resource>> Create(Resource resource)
        {
            #region Field Validation
            if (String.IsNullOrEmpty(resource.Name)) return BadRequest("Oops! Sorry, can't create a resource without name.");
            if (String.IsNullOrEmpty(resource.Status)) return BadRequest("Oops! Sorry, can't create a resource  without status."); 
            if (String.IsNullOrEmpty(resource.CitizenId)) return BadRequest("Oops! Sorry, can't create a resource without a citizenId."); 
            if (String.IsNullOrEmpty(resource.ResourceId)) resource.ResourceId = Guid.NewGuid().ToString();
            #endregion

            try
            {   
                await _resourceRepository.AddResource(resource);
                _logger.LogDebug("AddResource called with param {0}", resource);

                var createdResource = await _resourceRepository.GetResource(resource.ResourceId);
                if (createdResource != null) return Created($"/Resources/{resource.ResourceId}", createdResource);
                //Return failure if the created resource can't be retrieved successfully
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (DuplicateNameException ex)
            {
                _logger.LogWarning(ex, "AddResource failed for resource {0} due to duplicate name", resource);
                return this.StatusCode(StatusCodes.Status409Conflict, "Oops! Sorry, a resource with that resourceId already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AddResource failed with param {0}", resource);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // DELETE /resources/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpDelete("{resourceId}")]
        public async Task<ActionResult> DeleteV1(string resourceId)
        {
            try
            {
                var resource = await _resourceRepository.GetResource(resourceId);
                if (resource == null) return NotFound("Oops! Sorry, can't find that resource.");

                var deleteAccepted = await _resourceRepository.RemoveResource(resourceId);
                _logger.LogDebug("DeleteResource called with param {0}", resourceId);

                if (deleteAccepted) return this.StatusCode(StatusCodes.Status202Accepted);
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "DeleteResource failed with param {0}", resourceId);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // PUT /resources/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpPut("{resourceId}")]
        public async Task<ActionResult<Resource>> PutV1(string resourceId, Resource resourceUpdates)
        {
            try
            {
                var oldResource = await _resourceRepository.GetResource(resourceId);
                if (oldResource == null) return NotFound("Oops! Sorry, can't find that resource.");

                #region Field Validation
                if (!String.IsNullOrEmpty(resourceUpdates.InternalId)) return BadRequest("Oops! Sorry, can't update internalId.");
                if (!String.IsNullOrEmpty(resourceUpdates.ResourceId)) return BadRequest("Oops! Sorry, can't update resourceId.");
                if (!String.IsNullOrEmpty(resourceUpdates.CitizenId)) return BadRequest("Oops! Sorry, can't update citizenId.");
                if (!String.IsNullOrEmpty(resourceUpdates.Name)) oldResource.Name = resourceUpdates.Name;
                if (!String.IsNullOrEmpty(resourceUpdates.Status)) oldResource.Status = resourceUpdates.Status;
                #endregion

                var updateAccepted = await _resourceRepository.ReplaceResource(resourceId, resourceUpdates);
                _logger.LogDebug("ReplaceResource called with params {0}", new string[] { resourceId, resourceUpdates.ToString() });

                if (updateAccepted)
                {
                    //return updated resource
                    var result = await _resourceRepository.GetResource(resourceId);
                    return Ok(oldResource);
                }
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ReplaceResource failed with params {0}", new string[] { resourceId, resourceUpdates.ToString() });
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }
    }
}
