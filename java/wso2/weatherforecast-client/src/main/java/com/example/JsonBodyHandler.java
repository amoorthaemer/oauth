package com.example;

import java.io.IOException;
import java.io.UncheckedIOException;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;

import com.fasterxml.jackson.databind.ObjectMapper;

public class JsonBodyHandler<T> implements HttpResponse.BodyHandler<T> {

    private Class<T> wClass;
    private ObjectMapper mapper;

    public JsonBodyHandler(Class<T> wClass) {
        this(wClass, new ObjectMapper());
    }

    public JsonBodyHandler(Class<T> wClass, ObjectMapper mapper) {
        super();
        this.wClass = wClass;
        this.mapper = mapper;
    }

    @Override
    public HttpResponse.BodySubscriber<T> apply(HttpResponse.ResponseInfo responseInfo) {
        return asJSON(wClass, mapper);
    }

    public static <T> HttpResponse.BodySubscriber<T> asJSON(Class<T> targetType, ObjectMapper mapper) {
        HttpResponse.BodySubscriber<String> upstream = HttpResponse.BodySubscribers.ofString(StandardCharsets.UTF_8);

        return HttpResponse.BodySubscribers.mapping(
                upstream,
                (String body) -> {
                    try {
                        return mapper.readValue(body, targetType);
                    } catch (IOException e) {
                        throw new UncheckedIOException(e);
                    }
                });
    }
}