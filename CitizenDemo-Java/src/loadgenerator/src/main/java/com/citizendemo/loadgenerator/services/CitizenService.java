package com.citizendemo.loadgenerator.services;

import com.citizendemo.loadgenerator.models.Citizen;

import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.*;
import org.springframework.instrument.classloading.ReflectiveLoadTimeWeaver;
import org.springframework.web.client.RestTemplate;

import java.util.List;

public class CitizenService {
    private static RestTemplate restTemplate = new RestTemplate();
    public static void CreateCitizen (Citizen citizen, String citizenServiceURL) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<String> request = new HttpEntity<String>(citizen.toJSONString(), headers);
        restTemplate.postForObject(citizenServiceURL, request, String.class);
    }
    public static List<Citizen> GetCitizens (String citizenServiceURL) {
        ResponseEntity<List<Citizen>> response = restTemplate.exchange(
                citizenServiceURL,
                HttpMethod.GET,
                null,
                new ParameterizedTypeReference<List<Citizen>>(){});
        List<Citizen> citizens = response.getBody();
        return citizens;
    }
    public static List<Citizen> SearchCitizens (String name, String postalCode, String city, String state, String country, String citizenServiceURL) {
        StringBuilder searchQueryString = new StringBuilder();
        searchQueryString.append("?name=");
        searchQueryString.append(name);
        searchQueryString.append("&postalCode=");
        searchQueryString.append(postalCode);
        searchQueryString.append("&city=");
        searchQueryString.append(city);
        searchQueryString.append("&state=");
        searchQueryString.append(state);
        searchQueryString.append("&country=");
        searchQueryString.append(country);
        ResponseEntity<List<Citizen>> response = restTemplate.exchange(
                citizenServiceURL + "/search" + searchQueryString.toString(),
                HttpMethod.GET,
                null,
                new ParameterizedTypeReference<List<Citizen>>(){});
        List<Citizen> citizens = response.getBody();
        return citizens;
    }
    public static void DeleteCitizens (Citizen citizen, String citizenServiceURL) {
        restTemplate.delete(citizenServiceURL + "/" + citizen.citizenId);
    }
}