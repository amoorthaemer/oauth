package com.example.WeatherForecastClient;

import java.util.Objects;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.RequestEntity;
import org.springframework.http.ResponseEntity;
import org.springframework.security.oauth2.client.AuthorizedClientServiceOAuth2AuthorizedClientManager;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClient;
import org.springframework.security.oauth2.core.OAuth2AccessToken;
import org.springframework.web.client.RestTemplate;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;

@Configuration
@SpringBootApplication
public class Application implements CommandLineRunner {

	Logger logger = LoggerFactory.getLogger(CommandLineRunner.class);

	public static void main(String[] args) {
		SpringApplication.run(Application.class, args);
	}

	@Autowired
	private AuthorizedClientServiceOAuth2AuthorizedClientManager authorizedClientServiceAndManager;

	@Override
	public void run(String... args) throws Exception {
		OAuth2AuthorizeRequest authorizeRequest = OAuth2AuthorizeRequest.withClientRegistrationId("wso2")
			.principal("Weatherforecast Service")
			.build();

		OAuth2AuthorizedClient authorizedClient = this.authorizedClientServiceAndManager.authorize(authorizeRequest);
		OAuth2AccessToken accessToken = Objects.requireNonNull(authorizedClient).getAccessToken();

		logger.info("Issued: " + accessToken.getIssuedAt().toString() + ", Expires:" + accessToken.getExpiresAt().toString());
		logger.info("Scopes: " + accessToken.getScopes().toString());
		logger.info("Token: " + accessToken.getTokenValue());

		ObjectMapper mapper = new ObjectMapper()
			.registerModule(new JavaTimeModule());

		// Add the JWT to the RestTemplate headers
		HttpHeaders headers = new HttpHeaders();
		headers.add("Authorization", "Bearer " + accessToken.getTokenValue());

		RequestEntity<Void> request = RequestEntity
			.get("http://localhost:8081")
			.headers(headers)
			.build();

		// Make the actual HTTP GET request
		RestTemplate template = new RestTemplate();

		ResponseEntity<String> getMessageResponse = template.exchange(request, String.class);

		String message = getMessageResponse.getBody();
		logger.info("Reply = " + message);

		request = RequestEntity
			.get("http://localhost:8081/forecasts")
			.headers(headers)
			.accept(MediaType.APPLICATION_JSON)
			.build();
			
		ResponseEntity<WeatherForecast[]> getForecastsResponse = template.exchange(request, WeatherForecast[].class);

		WeatherForecast[] forecasts = getForecastsResponse.getBody();
		logger.info("Reply = " + mapper.writeValueAsString(forecasts));
	}	
}
