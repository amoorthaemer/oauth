package com.example;

import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.HttpResponse.BodyHandlers;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import com.nimbusds.oauth2.sdk.*;
import com.nimbusds.oauth2.sdk.auth.*;
import com.nimbusds.oauth2.sdk.id.*;
import com.nimbusds.oauth2.sdk.token.*;

public final class App {
    private App() {
    }

    /**
     * Demo of OAuth 2.0 authentication
     * @param args The arguments of the program.
     * @throws IOException
     * @throws ParseException
     * @throws URISyntaxException
     * @throws InterruptedException
     */
    public static void main(String[] args) throws ParseException, IOException, URISyntaxException, InterruptedException {
        // Construct the client credentials grant
        AuthorizationGrant clientGrant = new ClientCredentialsGrant();

        // The credentials to authenticate the client at the token endpoint
        ClientID clientID = new ClientID("KXa2TwKGQNp92ffy57SbvaXJen4a");
        Secret clientSecret = new Secret("KW5omMPDBjP2YDJCxC2Da0kHwgka");
        ClientAuthentication clientAuth = new ClientSecretBasic(clientID, clientSecret);

        // The request scope for the token (may be optional)
        Scope scope = new Scope("weatherapp");

        // The token endpoint
        URI tokenEndpoint = new URI("https://is.parel25.nl:9443/oauth2/token");

        // Make the token request
        TokenRequest request = new TokenRequest(tokenEndpoint, clientAuth, clientGrant, scope);
        TokenResponse response = TokenResponse.parse(request.toHTTPRequest().send());

        if (! response.indicatesSuccess()) {
            TokenErrorResponse errorResponse = response.toErrorResponse();
            System.out.println("Error: " + errorResponse.toString());

        } else {
            AccessTokenResponse successResponse = response.toSuccessResponse();

            // Get the access token
            AccessToken accessToken = successResponse.getTokens().getAccessToken();        
            System.out.println("Token: " + accessToken.toString());

            HttpClient httpClient = HttpClient.newBuilder()
                .build();

            HttpRequest getMessageRequest = HttpRequest.newBuilder()
                .uri(new URI("http://localhost:8081"))
                .GET()
                .header("Authorization", "Bearer " + accessToken.toString())
                .build();

            HttpResponse<String> getMessageResponse = httpClient.send(getMessageRequest, BodyHandlers.ofString());
            if (getMessageResponse.statusCode() == 200) {
                System.out.println(getMessageResponse.body());
            } else {
                System.out.println("Statuscode: " + getMessageResponse.statusCode());
            }

            ObjectMapper mapper = new ObjectMapper()
                .registerModule(new JavaTimeModule());

            HttpRequest getForecastsRequest = HttpRequest.newBuilder()
                .uri(new URI("http://localhost:8081/forecasts"))
                .GET()
                .header("Authorization", "Bearer " + accessToken.toString())
                .header("Accept", "application/json")
                .build();

            HttpResponse<WeatherForecast[]> getForecastsResponse = httpClient.send(
                getForecastsRequest,
                new JsonBodyHandler<>(WeatherForecast[].class, mapper)
            );

            if (getForecastsResponse.statusCode() == 200) {
                WeatherForecast[] forecasts = getForecastsResponse.body();
                System.out.println(mapper.writerWithDefaultPrettyPrinter().writeValueAsString(forecasts));
            } else {
                System.out.println("Statuscode " + getForecastsResponse.statusCode());
            }
        }
    }
}
