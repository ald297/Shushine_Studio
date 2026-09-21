package com.shushinestudio.modelos;

import jakarta.persistence.*;
import lombok.*;

import java.time.LocalTime;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "horarios_estilistas")
public class HorarioEstilista {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id_horario")
    private Integer id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "id_estilista", nullable = false)
    private Estilista estilista;

    @Column(name = "dia_semana", nullable = false)
    private Integer diaSemana; // 1 = Lunes, ..., 7 = Domingo

    @Column(name = "hora_inicio", nullable = false)
    private LocalTime horaInicio;

    @Column(name = "hora_fin", nullable = false)
    private LocalTime horaFin;

    @Column(name = "hora_inicio_almuerzo")
    private LocalTime horaInicioAlmuerzo;

    @Column(name = "hora_fin_almuerzo")
    private LocalTime horaFinAlmuerzo;

    @Builder.Default
    @Column(nullable = false)
    private Boolean activo = true;
}
