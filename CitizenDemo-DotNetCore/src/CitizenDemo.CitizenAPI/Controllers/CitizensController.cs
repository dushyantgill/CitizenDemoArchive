using CitizenDemo.CitizenAPI.Data;
using CitizenDemo.CitizenAPI.Services;
using CitizenDemo.CitizenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CitizenDemo.CitizenAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CitizensController : ControllerBase
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IResourceService _resourceService;
        private readonly ILogger _logger;

        public CitizensController(ICitizenRepository citizenRepository, IResourceService resourceService, ILogger<CitizensController> logger)
        {
            _citizenRepository = citizenRepository;
            _resourceService = resourceService;
            _logger = logger;
        }

        // GET /citizens/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _citizenRepository.GetCitizens();
                _logger.LogDebug("GetCitizens called");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetCitizens failed.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // GET /citizens/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpGet("{citizenId}")]
        public async Task<ActionResult<Citizen>> Get(string citizenId)
        {
            try
            {
                var result = await _citizenRepository.GetCitizen(citizenId);
                if (result == null) { return NotFound(); }
                _logger.LogDebug("GetCitizen called with param {0}", citizenId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetCitizen failed with param {0}", citizenId);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // GET /citizens/search?name=gill&postalCode=98119&city=seattle&state=wa&country=us
        [HttpGet("search")]
        public async Task<ActionResult<Resource[]>> Search(string name, string postalCode, string city, string state, string country)
        {
            try
            {
                var results = await _citizenRepository.SearchCitizens(name, postalCode, city, state, country);
                _logger.LogDebug("SearchCitizens called for with params {0}", new string[] { name, postalCode, city, state, country });
                if (!results.Any()) return NotFound();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SearchCitizens failed with params {0}", new string[] { name, postalCode, city, state, country });
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // POST /citizens/
        [HttpPost]
        public async Task<ActionResult<Citizen>> Create(Citizen citizen)
        {
            #region Field Validation
            if (String.IsNullOrEmpty(citizen.GivenName)) return BadRequest("Oops! Sorry, can't create a citizen without givenName.");
            if (String.IsNullOrEmpty(citizen.Surname)) return BadRequest("Oops! Sorry, can't create a citizen without surname."); 
            if (String.IsNullOrEmpty(citizen.StreetAddress)) return BadRequest("Oops! Sorry, can't create a citizen without streetAddress."); 
            if (String.IsNullOrEmpty(citizen.City)) return BadRequest("Oops! Sorry, can't create a citizen without city."); 
            if (String.IsNullOrEmpty(citizen.State)) return BadRequest("Oops! Sorry, can't create a citizen without state."); 
            if (String.IsNullOrEmpty(citizen.PostalCode)) return BadRequest("Oops! Sorry, can't create a citizen without postalCode."); 
            if (String.IsNullOrEmpty(citizen.Country)) return BadRequest("Oops! Sorry, can't create a citizen without country.");
            if (String.IsNullOrEmpty(citizen.CitizenId)) citizen.CitizenId = Guid.NewGuid().ToString();
            #endregion

            try
            {   
                await _citizenRepository.AddCitizen(citizen);
                _logger.LogDebug("AddCitizen called with param {0}", citizen);

                //After creating citizen, call ResourceAPI to provision default resource for citizen
                await _resourceService.ProvisionDefaultResource(citizen);
                _logger.LogDebug("ProvisionDefaultResource called for citizen {0}", citizen.CitizenId);

                var createdCitizen = await _citizenRepository.GetCitizen(citizen.CitizenId);
                _logger.LogDebug("GetCitizen called with param {0}", citizen.CitizenId);

                if (createdCitizen != null) return Created($"/Citizens/{citizen.CitizenId}", createdCitizen);
                //Return failure if the created citizen can't be retrieved successfully
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (DuplicateNameException ex)
            {
                _logger.LogWarning(ex, "AddCitizen failed for citizen {0} due to duplicate name", citizen);
                return this.StatusCode(StatusCodes.Status409Conflict, "Oops! Sorry, a citizen with that citizenId already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AddCitizen failed with param {0}", citizen);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // DELETE /citizens/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpDelete("{citizenId}")]
        public async Task<ActionResult> Delete(string citizenId)
        {
            try
            {
                var citizen = await _citizenRepository.GetCitizen(citizenId);
                if (citizen == null) return NotFound("Oops! Sorry, can't find that citizen.");

                var deleteAccepted = await _citizenRepository.RemoveCitizen(citizenId);
                _logger.LogDebug("DeleteCitizen called with param {0}", citizenId);

                //After deleting citizen, call ResourceAPI to deprovision resources of citizen
                await _resourceService.DeprovisionAllResources(citizen);
                _logger.LogDebug("DeprovisionAllResources called for citizen {0}", citizenId);

                if (deleteAccepted) return this.StatusCode(StatusCodes.Status202Accepted);
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "DeleteCitizen failed with param {0}", citizenId);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }

        // PUT /citizens/739ad8de-7b3b-45c1-a90c-697ef16317ce/
        [HttpPut("{citizenId}")]
        public async Task<ActionResult<Citizen>> PutV1(string citizenId, Citizen citizenUpdates)
        {
            try
            {
                var oldCitizen = await _citizenRepository.GetCitizen(citizenId);
                if (oldCitizen == null) return NotFound("Oops! Sorry, can't find that citizen.");

                #region Field Validation
                if (!String.IsNullOrEmpty(citizenUpdates.CitizenId)) return BadRequest("Oops! Sorry, can't update citizenId.");
                if (!String.IsNullOrEmpty(citizenUpdates.GivenName)) oldCitizen.GivenName = citizenUpdates.GivenName;
                if (!String.IsNullOrEmpty(citizenUpdates.Surname)) oldCitizen.Surname = citizenUpdates.Surname;
                if (!String.IsNullOrEmpty(citizenUpdates.PhoneNumber)) oldCitizen.PhoneNumber = citizenUpdates.PhoneNumber;
                if (!String.IsNullOrEmpty(citizenUpdates.StreetAddress)) oldCitizen.StreetAddress = citizenUpdates.StreetAddress;
                if (!String.IsNullOrEmpty(citizenUpdates.City)) oldCitizen.City = citizenUpdates.City;
                if (!String.IsNullOrEmpty(citizenUpdates.State)) oldCitizen.State = citizenUpdates.State;
                if (!String.IsNullOrEmpty(citizenUpdates.PostalCode)) oldCitizen.PostalCode = citizenUpdates.PostalCode;
                if (!String.IsNullOrEmpty(citizenUpdates.Country)) oldCitizen.Country = citizenUpdates.Country;
                #endregion

                var updateAccepted = await _citizenRepository.ReplaceCitizen(citizenId, oldCitizen);
                _logger.LogDebug("ReplaceCitizen called with params {0}", new string[] { citizenId, citizenUpdates.ToString() });

                if (updateAccepted)
                {
                    //return updated citizen
                    var result = await _citizenRepository.GetCitizen(citizenId);
                    return Ok(oldCitizen);
                }
                else return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something might have gone wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ReplaceCitizen failed with params {0}", new string[] { citizenId, citizenUpdates.ToString() });
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Oops! Sorry, something bad happened.");
            }
        }
    }
}
