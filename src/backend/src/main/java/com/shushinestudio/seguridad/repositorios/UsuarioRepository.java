package com.shushinestudio.seguridad.repositorios;

import com.shushinestudio.seguridad.modelos.Usuario;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface UsuarioRepository extends JpaRepository<Usuario, Integer> {
    Optional<Usuario> findByLogin(String login);
    boolean existsByLogin(String login);
}
