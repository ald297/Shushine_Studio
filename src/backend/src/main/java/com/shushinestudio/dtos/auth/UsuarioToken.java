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
public class UsuarioToken {
    private String token;
    private Integer id;
    private String login;
    private String nombre;
    private String rol;
}
