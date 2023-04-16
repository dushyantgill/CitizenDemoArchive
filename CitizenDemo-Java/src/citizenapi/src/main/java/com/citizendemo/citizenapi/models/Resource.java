package com.citizendemo.citizenapi.models;

public class Resource {
    public String resourceId;
    public String citizenId;
    public String name;
    public String status;

    public Resource() {
    }

    public Resource(String resourceId, String citizenId, String name, String status) {
        this.resourceId = resourceId;
        this.citizenId = citizenId;
        this.name = name;
        this.status = status;
    }

    public String toJSONString() {
        return "{" +
                "\"resourceId\":\"" + this.resourceId + "\"," +
                "\"citizenId\":\"" + this.citizenId + "\"," +
                "\"name\":\"" + this.name + "\"," +
                "\"status\":\"" + this.status + "\"" +
                "}";
    }
}