package com.shushinestudio.servicios.interfaces;

import com.shushinestudio.dtos.disponibilidad.DisponibilidadSalida;
import com.shushinestudio.dtos.disponibilidad.EstilistaSalida;

import java.time.LocalDate;
import java.util.List;

public interface IDisponibilidadService {
    List<EstilistaSalida> obtenerEstilistasActivos();
    EstilistaSalida obtenerEstilistaPorId(Integer id);
    DisponibilidadSalida calcularDisponibilidad(Integer estilistaId, LocalDate fecha, Integer duracionMinutos);
}
