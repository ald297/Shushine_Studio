package com.shushinestudio.controladores;

import com.shushinestudio.dtos.disponibilidad.DisponibilidadSalida;
import com.shushinestudio.dtos.disponibilidad.EstilistaSalida;
import com.shushinestudio.servicios.interfaces.IDisponibilidadService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/estilistas")
@Tag(name = "Estilistas y Disponibilidad", description = "Endpoints de personal profesional y cálculo de franjas horarias libres")
public class EstilistaController {

    @Autowired
    private IDisponibilidadService disponibilidadService;

    @GetMapping
    @Operation(summary = "Listar estilistas activos", description = "Retorna el listado del equipo profesional del salón.")
    public ResponseEntity<List<EstilistaSalida>> obtenerEstilistas() {
        List<EstilistaSalida> estilistas = disponibilidadService.obtenerEstilistasActivos();
        return ResponseEntity.ok(estilistas);
    }

    @GetMapping("/{id}")
    @Operation(summary = "Obtener detalle de un estilista", description = "Retorna el perfil y especialidad del estilista.")
    public ResponseEntity<EstilistaSalida> obtenerEstilistaPorId(@PathVariable Integer id) {
        EstilistaSalida estilista = disponibilidadService.obtenerEstilistaPorId(id);
        if (estilista != null) {
            return ResponseEntity.ok(estilista);
        }
        return ResponseEntity.notFound().build();
    }

    @GetMapping("/{id}/disponibilidad")
    @Operation(
            summary = "Calcular disponibilidad horaria en tiempo real",
            description = "Calcula los bloques libres excluyendo jornada no laboral, almuerzo, bloqueos y citas existentes."
    )
    public ResponseEntity<DisponibilidadSalida> calcularDisponibilidad(
            @PathVariable Integer id,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate fecha,
            @RequestParam(required = false, defaultValue = "30") Integer duracionMinutos
    ) {
        DisponibilidadSalida disponibilidad = disponibilidadService.calcularDisponibilidad(id, fecha, duracionMinutos);
        return ResponseEntity.ok(disponibilidad);
    }
}
