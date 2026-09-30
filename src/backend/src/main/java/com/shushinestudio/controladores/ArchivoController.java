package com.shushinestudio.controladores;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.Base64;
import java.util.Map;

@RestController
@RequestMapping("/api/archivos")
@Tag(name = "Archivos y Multimedia", description = "Endpoints para subida y procesamiento de imágenes para chat, solicitudes de diseño y portafolio")
public class ArchivoController {

    @PostMapping(value = "/subir", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    @SecurityRequirement(name = "Bearer Authentication")
    @Operation(summary = "Subir imagen adjunta", description = "Procesa una imagen y retorna su URL segura para vincular a chats, solicitudes o perfiles.")
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
            String base64 = Base64.getEncoder().encodeToString(bytes);
            String dataUrl = "data:" + contentType + ";base64," + base64;

            return ResponseEntity.status(HttpStatus.CREATED).body(Map.of(
                    "url", dataUrl,
                    "nombreOriginal", archivo.getOriginalFilename() != null ? archivo.getOriginalFilename() : "imagen.jpg",
                    "tamanoBytes", archivo.getSize(),
                    "contentType", contentType
            ));
        } catch (IOException e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(Map.of("error", "Error al procesar el archivo: " + e.getMessage()));
        }
    }
}
