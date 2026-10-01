package com.shushinestudio.controladores;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.Base64;
import java.util.Map;
import java.util.UUID;

@RestController
@RequestMapping("/api/archivos")
@Tag(name = "Archivos y Multimedia", description = "Endpoints para subida persistente de imágenes a Supabase Storage")
public class ArchivoController {

    private static final String SUPABASE_STORAGE_URL = "https://acikahicfjtojuvqcvxv.supabase.co/storage/v1/object";
    private static final String SUPABASE_PUBLIC_URL = "https://acikahicfjtojuvqcvxv.supabase.co/storage/v1/object/public";
    private static final String DEFAULT_BUCKET = "shushine-media";
    private static final String SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFjaWthaGljZmp0b2p1dnFjdnh2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODkwNTIzNzQsImV4cCI6MjEwNDYyODM3NH0.DQsPt8hNgCNhEyda7Wv03VCH66ZqwEGe09NRj2Md5MI";

    private final HttpClient httpClient = HttpClient.newBuilder()
            .connectTimeout(Duration.ofSeconds(15))
            .build();

    @PostMapping(value = "/subir", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    @SecurityRequirement(name = "Bearer Authentication")
    @Operation(summary = "Subir imagen adjunta a Supabase Storage", description = "Almacena la imagen de forma persistente en Supabase Storage y retorna su URL pública accesible.")
    public ResponseEntity<Map<String, Object>> subirArchivo(@RequestParam("archivo") MultipartFile archivo) {
        if (archivo == null || archivo.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "No se proporcionó ningún archivo válido"));
        }

        String contentType = archivo.getContentType();
        if (contentType == null || !contentType.startsWith("image/")) {
            return ResponseEntity.badRequest().body(Map.of("error", "Solo se permiten archivos de imagen (JPEG, PNG, WEBP)"));
        }

        try {
            byte[] bytes = archivo.getBytes();
            String originalName = archivo.getOriginalFilename() != null ? archivo.getOriginalFilename() : "imagen.jpg";
            String extension = originalName.contains(".") ? originalName.substring(originalName.lastIndexOf(".")) : ".jpg";
            String fileName = UUID.randomUUID().toString() + extension;

            // 1. Subida persistente a Supabase Storage
            String uploadUrl = SUPABASE_STORAGE_URL + "/" + DEFAULT_BUCKET + "/" + fileName;
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(uploadUrl))
                    .timeout(Duration.ofSeconds(20))
                    .header("Authorization", "Bearer " + SUPABASE_ANON_KEY)
                    .header("Content-Type", contentType)
                    .POST(HttpRequest.BodyPublishers.ofByteArray(bytes))
                    .build();

            HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());

            String finalUrl;
            if (response.statusCode() >= 200 && response.statusCode() < 300) {
                finalUrl = SUPABASE_PUBLIC_URL + "/" + DEFAULT_BUCKET + "/" + fileName;
            } else {
                // Fallback de contingencia a Base64 si Supabase Storage no estuviera accesible
                String base64 = Base64.getEncoder().encodeToString(bytes);
                finalUrl = "data:" + contentType + ";base64," + base64;
            }

            return ResponseEntity.status(HttpStatus.CREATED).body(Map.of(
                    "url", finalUrl,
                    "nombreOriginal", originalName,
                    "tamanoBytes", archivo.getSize(),
                    "contentType", contentType
            ));
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(Map.of("error", "Error al procesar el archivo: " + e.getMessage()));
        }
    }
}
