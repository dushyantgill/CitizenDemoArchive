package com.citizendemo.citizenapi.services;

import com.citizendemo.citizenapi.models.Citizen;
import com.citizendemo.citizenapi.models.Resource;
import org.springframework.beans.factory.annotation.*;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

public class ResourceService {
    private static RestTemplate restTemplate = new RestTemplate();

    public static void ProvisionDefaultResource(Citizen citizen, String resourceServiceURL) {
        Resource resource = new Resource(
                java.util.UUID.randomUUID().toString(),
                citizen.citizenId,
                "Stimulus 2021H2",
                "New"
        );
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<String> request = new HttpEntity<String>(resource.toJSONString(), headers);

        restTemplate.postForObject(resourceServiceURL, request, String.class);
    }

    public static void DeprovisionAllResources(Citizen citizen, String resourceServiceURL) {
        restTemplate.delete(resourceServiceURL + "?citizenId=" + citizen.citizenId);
    }
}