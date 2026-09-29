package com.shushinestudio.servicios.interfaces;

import com.shushinestudio.dtos.disponibilidad.DisponibilidadSalida;
import com.shushinestudio.dtos.disponibilidad.EstilistaGuardarDto;
import com.shushinestudio.dtos.disponibilidad.EstilistaSalida;

import java.time.LocalDate;
import java.util.List;

public interface IDisponibilidadService {
    List<EstilistaSalida> obtenerEstilistasActivos();
    List<EstilistaSalida> obtenerTodosEstilistas();
    EstilistaSalida obtenerEstilistaPorId(Integer id);
    DisponibilidadSalida calcularDisponibilidad(Integer estilistaId, LocalDate fecha, Integer duracionMinutos);
    EstilistaSalida crearEstilista(EstilistaGuardarDto dto);
    void actualizarEstadoEstilista(Integer id, Boolean activo);
    void eliminarEstilista(Integer id);
}
