package com.shushinestudio.dtos.auth;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class UsuarioPerfil {
    private Integer id;
    private String login;
    private String nombre;
    private String apellido;
    private String telefono;
    private String rol;
    private Boolean activo;
}
