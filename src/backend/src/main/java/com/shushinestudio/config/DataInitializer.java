package com.shushinestudio.config;

import com.shushinestudio.modelos.Categoria;
import com.shushinestudio.modelos.Estilista;
import com.shushinestudio.modelos.Servicio;
import com.shushinestudio.repositorios.ICategoriaRepository;
import com.shushinestudio.repositorios.IEstilistaRepository;
import com.shushinestudio.repositorios.IServicioRepository;
import com.shushinestudio.seguridad.modelos.Rol;
import com.shushinestudio.seguridad.modelos.Usuario;
import com.shushinestudio.seguridad.repositorios.RolRepository;
import com.shushinestudio.seguridad.repositorios.UsuarioRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;

@Component
public class DataInitializer implements CommandLineRunner {

    @Autowired
    private RolRepository rolRepository;

    @Autowired
    private UsuarioRepository usuarioRepository;

    @Autowired
    private ICategoriaRepository categoriaRepository;

    @Autowired
    private IServicioRepository servicioRepository;

    @Autowired
    private IEstilistaRepository estilistaRepository;

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Override
    public void run(String... args) {
        // 1. Inicializar Roles Oficiales del Salón Shushine Studio
        Rol rolAdmin = rolRepository.findByNombreIgnoreCase("ADMIN").orElseGet(() -> {
            Rol nuevoRol = Rol.builder()
                    .nombre("ADMIN")
                    .descripcion("Administrador con acceso integral a gestión, personal y reportes")
                    .build();
            return rolRepository.save(nuevoRol);
        });

        Rol rolCliente = rolRepository.findByNombreIgnoreCase("CLIENTE").orElseGet(() -> {
            Rol nuevoRol = Rol.builder()
                    .nombre("CLIENTE")
                    .descripcion("Cliente registrado para consulta de catálogo y reserva de citas")
                    .build();
            return rolRepository.save(nuevoRol);
        });

        rolRepository.findByNombreIgnoreCase("RECEPCIONISTA").orElseGet(() -> {
            Rol nuevoRol = Rol.builder()
                    .nombre("RECEPCIONISTA")
                    .descripcion("Personal de recepción para atención de citas y cobros")
                    .build();
            return rolRepository.save(nuevoRol);
        });

        // 2. Inicializar Usuario Administrador por Defecto
        if (usuarioRepository.findByLogin("admin").isEmpty()) {
            Usuario admin = Usuario.builder()
                    .nombre("Administrador")
                    .apellido("Shushine")
                    .telefono("70001122")
                    .login("admin")
                    .clave(passwordEncoder.encode("admin123"))
                    .rol(rolAdmin)
                    .activo(true)
                    .build();
            usuarioRepository.save(admin);
        }

        // 3. Inicializar Usuario Cliente de Prueba
        if (usuarioRepository.findByLogin("cliente").isEmpty()) {
            Usuario cliente = Usuario.builder()
                    .nombre("Camila")
                    .apellido("Calderon")
                    .telefono("70003344")
                    .login("cliente")
                    .clave(passwordEncoder.encode("cliente123"))
                    .rol(rolCliente)
                    .activo(true)
                    .build();
            usuarioRepository.save(cliente);
        }

        // 4. Inicializar Catálogo de Belleza Inicial si está vacío
        if (categoriaRepository.count() == 0) {
            Categoria catCorte = categoriaRepository.save(Categoria.builder()
                    .nombre("Corte y Estilo")
                    .descripcion("Cortes modernos, visagismo y estilismo personalizado para dama y caballero")
                    .iconoUrl("content_cut")
                    .tipo("Servicio")
                    .activo(true)
                    .build());

            Categoria catColor = categoriaRepository.save(Categoria.builder()
                    .nombre("Coloración y Mechas")
                    .descripcion("Balayage, babylights, tintes sin amoniaco y diseño de iluminación capilar")
                    .iconoUrl("brush")
                    .tipo("Servicio")
                    .activo(true)
                    .build());

            Categoria catTratamiento = categoriaRepository.save(Categoria.builder()
                    .nombre("Tratamientos Capilares")
                    .descripcion("Keratina vegetal, botox capilar orgánico y nutrición profunda")
                    .iconoUrl("spa")
                    .tipo("Servicio")
                    .activo(true)
                    .build());

            Categoria catUnas = categoriaRepository.save(Categoria.builder()
                    .nombre("Uñas y Manicura")
                    .descripcion("Manicura rusa, baño de acrílico, soft gel y nail art contemporáneo")
                    .iconoUrl("auto_fix_high")
                    .tipo("Servicio")
                    .activo(true)
                    .build());

            // 5. Inicializar Servicios de Prueba
            servicioRepository.save(Servicio.builder()
                    .codigoServicio("CORTE-01")
                    .categoria(catCorte)
                    .nombre("Corte Dama Signature & Lavado Especial")
                    .descripcion("Lavado con masaje capilar, asesoría de visagismo y corte con acabado secado natural.")
                    .precioBase(new BigDecimal("18.00"))
                    .esPrecioVariable(false)
                    .duracionMinutos(45)
                    .intervaloSeguimientoDias(30)
                    .imagenUrl("https://images.unsplash.com/photo-1560066984-138dadb4c035?auto=format&fit=crop&w=600&q=80")
                    .costoInsumos(new BigDecimal("2.50"))
                    .activo(true)
                    .build());

            servicioRepository.save(Servicio.builder()
                    .codigoServicio("COLOR-01")
                    .categoria(catColor)
                    .nombre("Balayage Iluminación & Matiz Glaze")
                    .descripcion("Técnica de aclarado a mano alzada para un efecto de luz degradado sin marcas.")
                    .precioBase(new BigDecimal("65.00"))
                    .esPrecioVariable(true)
                    .duracionMinutos(180)
                    .intervaloSeguimientoDias(60)
                    .imagenUrl("https://images.unsplash.com/photo-1522337360788-8b13dee7a37e?auto=format&fit=crop&w=600&q=80")
                    .costoInsumos(new BigDecimal("15.00"))
                    .activo(true)
                    .build());

            servicioRepository.save(Servicio.builder()
                    .codigoServicio("TRAT-01")
                    .categoria(catTratamiento)
                    .nombre("Hidratación Profunda con Keratina y Ozono")
                    .descripcion("Tratamiento restaurador de puentes de disulfuro para cabellos maltratados o decolorados.")
                    .precioBase(new BigDecimal("35.00"))
                    .esPrecioVariable(false)
                    .duracionMinutos(90)
                    .intervaloSeguimientoDias(21)
                    .imagenUrl("https://images.unsplash.com/photo-1527799820374-dcf8d9d4a388?auto=format&fit=crop&w=600&q=80")
                    .costoInsumos(new BigDecimal("8.00"))
                    .activo(true)
                    .build());

            servicioRepository.save(Servicio.builder()
                    .codigoServicio("UNAS-01")
                    .categoria(catUnas)
                    .nombre("Manicura Rusa & Esmaltado Semipermanente")
                    .descripcion("Limpieza milimétrica de cutícula con torno y esmaltado de alta duración.")
                    .precioBase(new BigDecimal("15.00"))
                    .esPrecioVariable(false)
                    .duracionMinutos(60)
                    .intervaloSeguimientoDias(15)
                    .imagenUrl("https://images.unsplash.com/photo-1632345031435-8727f6897d53?auto=format&fit=crop&w=600&q=80")
                    .costoInsumos(new BigDecimal("3.00"))
                    .activo(true)
                    .build());
        }

        // 6. Inicializar Estilistas del Salón si la tabla está vacía
        if (estilistaRepository.count() == 0) {
            estilistaRepository.save(Estilista.builder()
                    .nombreCompleto("Valentina Ramos Cruz")
                    .especialidadPrincipal("Coloración & Balayage")
                    .biografia("Especialista en técnicas de coloración avanzada con 6 años de experiencia. Certificada en balayage iluminado y corrección de color.")
                    .avatarUrl("https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=300&q=80")
                    .colorAgenda("#E91E8C")
                    .porcentajeComision(new BigDecimal("35.00"))
                    .activo(true)
                    .build());

            estilistaRepository.save(Estilista.builder()
                    .nombreCompleto("Sofía Martínez Leiva")
                    .especialidadPrincipal("Corte & Estilismo")
                    .biografia("Estilista graduada del Instituto Vidal Sassoon con dominio en cortes de visagismo y secados de alta gama.")
                    .avatarUrl("https://images.unsplash.com/photo-1438761681033-6461ffad8d80?auto=format&fit=crop&w=300&q=80")
                    .colorAgenda("#9C27B0")
                    .porcentajeComision(new BigDecimal("30.00"))
                    .activo(true)
                    .build());

            estilistaRepository.save(Estilista.builder()
                    .nombreCompleto("Andrea López Portillo")
                    .especialidadPrincipal("Uñas & Nail Art")
                    .biografia("Técnica en uñas acrílicas, gel y nail art con formación en Buenos Aires. Especialista en manicura rusa y diseños personalizados.")
                    .avatarUrl("https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=300&q=80")
                    .colorAgenda("#FF5722")
                    .porcentajeComision(new BigDecimal("32.00"))
                    .activo(true)
                    .build());

            estilistaRepository.save(Estilista.builder()
                    .nombreCompleto("Camila Herrera Molina")
                    .especialidadPrincipal("Pestañas & Tratamientos Faciales")
                    .biografia("Certificada en extensiones de pestañas clásicas, volumen ruso y lifting. Experta en limpieza facial profunda y cuidado de la piel.")
                    .avatarUrl("https://images.unsplash.com/photo-1531746020798-e6953c6e8e04?auto=format&fit=crop&w=300&q=80")
                    .colorAgenda("#00BCD4")
                    .porcentajeComision(new BigDecimal("30.00"))
                    .activo(true)
                    .build());
        }
    }
}
