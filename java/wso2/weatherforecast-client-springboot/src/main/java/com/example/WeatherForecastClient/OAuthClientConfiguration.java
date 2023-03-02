package com.example.WeatherForecastClient;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.oauth2.client.*;
import org.springframework.security.oauth2.client.registration.ClientRegistration;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.oauth2.client.registration.InMemoryClientRegistrationRepository;
import org.springframework.security.oauth2.core.AuthorizationGrantType;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;

@Configuration
public class OAuthClientConfiguration {

    // Create the client registration repository
    @Bean
    public ClientRegistrationRepository clientRegistrationRepository()
        throws JsonMappingException, JsonProcessingException
    {
        String json = new FileResources()
            .getFileFromResourceAsString("json/oauth-clients.json");

        ObjectMapper mapper = new ObjectMapper();
        TypeReference<Map<String, OAuthClient>> mapType = new TypeReference<Map<String, OAuthClient>>() {};

        Map<String, OAuthClient> clients = mapper.readValue(json, mapType);

        List<ClientRegistration> clientRegistrations = new ArrayList<ClientRegistration>();
            
        if (clients.size() > 0) {
            for (Map.Entry<String, OAuthClient> entry : clients.entrySet()) {
                String id = entry.getKey();
                OAuthClient client = entry.getValue();

                ClientRegistration clientRegistation = ClientRegistration
                    .withRegistrationId(id)
                    .tokenUri(client.tokenUri)
                    .clientId(client.clientId)
                    .clientSecret(client.clientSecret)
                    .scope(client.scopes)
                    .authorizationGrantType(new AuthorizationGrantType(client.authorizationGrantType))
                    .build();

                clientRegistrations.add(clientRegistation);
            }
        }

        return new InMemoryClientRegistrationRepository(clientRegistrations);
    }

    // Create the authorized client service
    @Bean
    public OAuth2AuthorizedClientService auth2AuthorizedClientService(ClientRegistrationRepository clientRegistrationRepository) {
        return new InMemoryOAuth2AuthorizedClientService(clientRegistrationRepository);
    }

    // Create the authorized client manager and service manager using the
    // beans created and configured above
    @Bean
    public AuthorizedClientServiceOAuth2AuthorizedClientManager authorizedClientServiceAndManager (
        ClientRegistrationRepository clientRegistrationRepository,
        OAuth2AuthorizedClientService authorizedClientService)
    {
        OAuth2AuthorizedClientProvider authorizedClientProvider =
            OAuth2AuthorizedClientProviderBuilder.builder()
                .clientCredentials()
                .build();

        AuthorizedClientServiceOAuth2AuthorizedClientManager authorizedClientManager =
            new AuthorizedClientServiceOAuth2AuthorizedClientManager(
                clientRegistrationRepository,
                authorizedClientService
            );

        authorizedClientManager.setAuthorizedClientProvider(authorizedClientProvider);

        return authorizedClientManager;
    }

    private class FileResources {

        public InputStream getFileFromResourceAsStream(String fileName) {
            // The class loader that loaded the class
            ClassLoader classLoader = getClass().getClassLoader();
            InputStream inputStream = classLoader.getResourceAsStream(fileName);

            // the stream holding the file content
            if (inputStream == null) {
                throw new IllegalArgumentException("file not found! " + fileName);
            } else {
                return inputStream;
            }

        }

        public String getFileFromResourceAsString(String fileName) {
            InputStream stream = getFileFromResourceAsStream(fileName);
            InputStreamReader reader = new InputStreamReader(stream, StandardCharsets.UTF_8);
            Scanner scanner = new Scanner(reader);

            StringBuilder builder = new StringBuilder();
            while (scanner.hasNext()) {
                builder.append(scanner.next());
            }
            scanner.close();

            return builder.toString();
        }
    }

}
