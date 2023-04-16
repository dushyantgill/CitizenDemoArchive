package com.citizendemo.loadgenerator;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class LoadGeneratorApplication {
    public static void main(String[] args) {
        Logger logger = LoggerFactory.getLogger(LoadGeneratorApplication.class);
        logger.info("LoadGenerator starting ...");
        SpringApplication.run(LoadGeneratorApplication.class, args);
    }
}
