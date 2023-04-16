package com.citizendemo.resourceapi.controllers;

import com.citizendemo.resourceapi.models.Resource;
import com.citizendemo.resourceapi.repositories.ResourceRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@RestController
public class ResourceController {
    @Autowired
    private ResourceRepository resourceRepository;
    private Logger logger = LoggerFactory.getLogger(ResourceController.class);

    @RequestMapping(value = "/resources", method = RequestMethod.GET)
    public List<Resource> getAllResources() {
        List<Resource> resources = new ArrayList<Resource>();
        Pageable page = PageRequest.of(0, 100);
        try {
            logger.info("getAllResources called");
            resources = resourceRepository.findAll(page).getContent();
        } catch (Exception e) {
            logger.warn("getAllResources failed", e);
        }
        logger.info("getAllResources returning " + resources.size() + " resources");
        return resources;
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.GET)
    public Resource getResourceByResourceId(@PathVariable("resourceId") String resourceId) {
        Resource resource = null;
        try {
            logger.info("getResourceByResourceId called with param " + resourceId);

            Optional<Resource> result = resourceRepository.findByResourceId(resourceId);
            if (result.isPresent()) {
                resource = result.get();
                logger.info("getResourceByResourceId returning a resource");
            }
        } catch (Exception e) {
            logger.warn("getResourceByResourceId failed with param " + resourceId, e);
        }
        return resource;
    }

    @RequestMapping(value = "/resources", method = RequestMethod.POST)
    public Resource createResource(@RequestBody Resource resource) {
        Resource createdResource = null;
        try {
            logger.info("createResource called with param " + resource.toString());
            Optional<Resource> existingResource = resourceRepository.findByResourceId(resource.resourceId);
            if (existingResource.isPresent()) {
                logger.info("createResource not creating, found existing resource with resourceId " + resource.resourceId);
                return null;
            }
            createdResource = resourceRepository.save(resource);
            logger.info("createResource created resource with internalId " + createdResource.internalId);
        } catch (Exception e) {
            logger.warn("createResource failed with param " + resource.toString(), e);
        }
        return createdResource;
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.PUT)
    public Resource updateResource(@PathVariable("resourceId") String resourceId, @RequestBody Resource resourceUpdates) {
        Resource updatedResource = null;
        try {
            logger.info("updateResource called with param " + resourceUpdates.toString());
            Optional<Resource> oldResource = resourceRepository.findByResourceId(resourceId);
            if (oldResource.isPresent()) {
                Resource newResource = oldResource.get();
                if (resourceUpdates.name != null && !resourceUpdates.name.trim().isEmpty())
                    newResource.name = resourceUpdates.name;
                if (resourceUpdates.status != null && !resourceUpdates.status.trim().isEmpty())
                    newResource.status = resourceUpdates.status;
                updatedResource = resourceRepository.save(newResource);
                logger.debug("updateResource returning updated resource");
            } else {
                logger.debug("updateResource did not find resource with resourceId " + resourceId);
            }
        } catch (Exception e) {
            logger.warn("updateResource failed with param " + resourceUpdates.toString(), e);
        }
        return updatedResource;
    }

    @RequestMapping(value = "/resources/{resourceId}", method = RequestMethod.DELETE)
    public void deleteResource(@PathVariable("resourceId") String resourceId) {
        try {
            logger.info("deleteResource called with param " + resourceId);
            Optional<Resource> resource = resourceRepository.findByResourceId(resourceId);
            if (resource.isPresent()) {
                resourceRepository.delete(resource.get());
                logger.info("deleteResource deleted resource with resourceId" + resourceId);
            } else {
                logger.info("deleteResource did not find resource with resourceId " + resourceId);
            }
        } catch (Exception e) {
            logger.warn("deleteResource failed with param " + resourceId);
        }
    }

    @RequestMapping(value = "/resources", method = RequestMethod.DELETE)
    public void deleteAllResourcesOfCitizen(@RequestParam("citizenId") String citizenId) {
        try {
            logger.info("deleteAllResourcesOfCitizen called with param " + citizenId);
            if (citizenId != null && !citizenId.trim().isEmpty()) {
                resourceRepository.deleteAllByCitizenId(citizenId);
                logger.info("deleteAllResourcesOfCitizen deleted all resources of citizen with citizenId " + citizenId);
            }
        } catch (Exception e) {
            logger.warn("deleteAllResourcesOfCitizen failed with param " + citizenId);
        }
    }
}
