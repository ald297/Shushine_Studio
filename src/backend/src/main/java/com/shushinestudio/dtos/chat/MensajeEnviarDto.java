package com.shushinestudio.dtos.chat;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import lombok.*;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class MensajeEnviarDto {

    @NotBlank(message = "El contenido del mensaje no puede estar vacío ni contener solo espacios.")
    @Size(max = 1000, message = "El mensaje no puede superar los 1000 caracteres.")
    private String contenido;
}
