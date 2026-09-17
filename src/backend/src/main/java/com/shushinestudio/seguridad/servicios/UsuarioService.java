package com.shushinestudio.seguridad.servicios;

import com.shushinestudio.dtos.auth.UsuarioLogin;
import com.shushinestudio.dtos.auth.UsuarioPerfil;
import com.shushinestudio.dtos.auth.UsuarioRegistrar;
import com.shushinestudio.dtos.auth.UsuarioToken;
import com.shushinestudio.seguridad.modelos.Rol;
import com.shushinestudio.seguridad.modelos.Usuario;
import com.shushinestudio.seguridad.repositorios.UsuarioRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class UsuarioService {

    @Autowired
    private UsuarioRepository usuarioRepository;

    @Autowired
    private RolService rolService;

    @Autowired
    private JwtService jwtService;

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Autowired
    private AuthenticationManager authenticationManager;

    @Transactional(readOnly = true)
    public UsuarioToken login(UsuarioLogin loginRequest) {
        authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(loginRequest.getLogin(), loginRequest.getClave())
        );

        Usuario usuario = usuarioRepository.findByLogin(loginRequest.getLogin())
                .orElseThrow(() -> new IllegalArgumentException("Credenciales no válidas para el usuario: " + loginRequest.getLogin()));

        String token = jwtService.getToken(usuario);

        return UsuarioToken.builder()
                .token(token)
                .id(usuario.getId())
                .login(usuario.getLogin())
                .nombre(usuario.getNombre() + (usuario.getApellido() != null ? " " + usuario.getApellido() : ""))
                .rol(usuario.getRol() != null ? usuario.getRol().getNombre() : "CLIENTE")
                .build();
    }

    @Transactional
    public UsuarioToken registro(UsuarioRegistrar registroRequest) {
        if (usuarioRepository.existsByLogin(registroRequest.getLogin())) {
            throw new IllegalArgumentException("El nombre de usuario '" + registroRequest.getLogin() + "' ya se encuentra registrado.");
        }

        Rol rol;
        if (registroRequest.getRolId() != null) {
            rol = rolService.obtenerPorId(registroRequest.getRolId());
            if (rol == null) {
                throw new IllegalArgumentException("El rol especificado con ID " + registroRequest.getRolId() + " no existe.");
            }
        } else {
            rol = rolService.obtenerPorNombre("CLIENTE")
                    .orElseThrow(() -> new IllegalStateException("El rol por defecto CLIENTE no está configurado en el sistema."));
        }

        Usuario nuevoUsuario = Usuario.builder()
                .nombre(registroRequest.getNombre())
                .apellido(registroRequest.getApellido())
                .telefono(registroRequest.getTelefono())
                .login(registroRequest.getLogin())
                .clave(passwordEncoder.encode(registroRequest.getClave()))
                .rol(rol)
                .activo(true)
                .build();

        Usuario usuarioGuardado = usuarioRepository.save(nuevoUsuario);
        String token = jwtService.getToken(usuarioGuardado);

        return UsuarioToken.builder()
                .token(token)
                .id(usuarioGuardado.getId())
                .login(usuarioGuardado.getLogin())
                .nombre(usuarioGuardado.getNombre() + (usuarioGuardado.getApellido() != null ? " " + usuarioGuardado.getApellido() : ""))
                .rol(usuarioGuardado.getRol().getNombre())
                .build();
    }

    @Transactional(readOnly = true)
    public UsuarioPerfil obtenerPerfil(String login) {
        Usuario usuario = usuarioRepository.findByLogin(login)
                .orElseThrow(() -> new IllegalArgumentException("Usuario no encontrado con login: " + login));

        return UsuarioPerfil.builder()
                .id(usuario.getId())
                .login(usuario.getLogin())
                .nombre(usuario.getNombre())
                .apellido(usuario.getApellido())
                .telefono(usuario.getTelefono())
                .rol(usuario.getRol() != null ? usuario.getRol().getNombre() : null)
                .activo(usuario.getActivo())
                .build();
    }
}
