package com.citizendemo.resourceapi.controllers;

import com.citizendemo.resourceapi.models.Resource;
import com.citizendemo.resourceapi.repositories.ResourceRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
public class ResourceController {
    @Autowired
    private ResourceRepository resourceRepository;

    @RequestMapping(value = "/resources", method = RequestMethod.GET)
    public List<Resource> getAllResources() {
        return resourceRepository.findAll();
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.GET)
    public Optional<Resource> getResourceById(@PathVariable("resourceId") String resourceId) {
        return resourceRepository.findByResourceId(resourceId);
    }

    @RequestMapping(value = "/resources", method = RequestMethod.POST)
    public Resource addNewResource(@RequestBody Resource resource){
        Optional<Resource> existingResource = resourceRepository.findByResourceId(resource.resourceId);
        if(existingResource.isPresent()) return null;
        return resourceRepository.save(resource);
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.PUT)
    public Resource updateResource(@PathVariable("resourceId") String resourceId, @RequestBody Resource resourceUpdates){
        Optional<Resource> oldResource = resourceRepository.findByResourceId(resourceId);
        if(oldResource.isPresent()) {
            Resource newResource = oldResource.get();
            if (resourceUpdates.name != null && !resourceUpdates.name.trim().isEmpty()) newResource.name = resourceUpdates.name;
            if (resourceUpdates.status != null && !resourceUpdates.status.trim().isEmpty()) newResource.status = resourceUpdates.status;
            return resourceRepository.save(newResource);
        }
        return null;
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.DELETE)
    public void deleteResource(@PathVariable("resourceId") String resourceId) {
        Optional<Resource> resource = resourceRepository.findByResourceId(resourceId);
        if(resource.isPresent()) resourceRepository.delete(resource.get());
    }

    @RequestMapping(value = "/resources", method = RequestMethod.DELETE)
    public void deleteAllResourcesOfCitizen(@RequestParam("citizenId") String citizenId) {
        if(citizenId != null && !citizenId.trim().isEmpty()) resourceRepository.deleteAllByCitizenId(citizenId);
    }
}
