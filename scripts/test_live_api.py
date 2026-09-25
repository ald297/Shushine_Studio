#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de Pruebas en Vivo para Shushine Studio Web API
Ejecuta una batería completa de pruebas HTTP sobre todos los endpoints desplegados en Render.
"""

import json
import time
import urllib.request
import urllib.error
import ssl

BASE_URL = "https://shushine-studio.onrender.com"
CTX = ssl.create_default_context()

results = []

def make_request(method, path, data=None, token=None, expected_status=200):
    url = f"{BASE_URL}{path}"
    headers = {
        "User-Agent": "ShushineStudioTestRunner/1.0",
        "Accept": "application/json"
    }
    body = None
    if data is not None:
        headers["Content-Type"] = "application/json"
        body = json.dumps(data).encode("utf-8")
    
    if token:
        headers["Authorization"] = f"Bearer {token}"
        
    req = urllib.request.Request(url, data=body, headers=headers, method=method)
    
    start_time = time.time()
    status_code = None
    response_data = None
    error_msg = None
    
    try:
        with urllib.request.urlopen(req, context=CTX, timeout=30) as response:
            status_code = response.status
            raw_body = response.read().decode("utf-8")
            if raw_body:
                try:
                    response_data = json.loads(raw_body)
                except Exception:
                    response_data = raw_body[:200]
    except urllib.error.HTTPError as e:
        status_code = e.code
        raw_body = e.read().decode("utf-8")
        try:
            response_data = json.loads(raw_body)
        except Exception:
            response_data = raw_body[:200]
        error_msg = str(e)
    except Exception as e:
        status_code = 0
        error_msg = str(e)
        
    duration_ms = round((time.time() - start_time) * 1000, 2)
    if isinstance(expected_status, list):
        success = status_code in expected_status
    else:
        success = (status_code == expected_status)
    
    res = {
        "method": method,
        "path": path,
        "expected": expected_status,
        "actual": status_code,
        "duration_ms": duration_ms,
        "success": success,
        "error": error_msg,
        "response": response_data
    }
    results.append(res)
    status_icon = "✅" if success else "❌"
    print(f"{status_icon} [{status_code}] {method:6} {path:45} ({duration_ms} ms)")
    return res

def run_all_tests():
    print("=================================================================")
    print("🚀 INICIANDO PRUEBAS INTEGRALES EN VIVO — SHUSHINE STUDIO WEB API")
    print(f"Servidor Objetivo: {BASE_URL}")
    print("=================================================================\n")
    
    # -------------------------------------------------------------
    # 1. AUTENTICACIÓN
    # -------------------------------------------------------------
    print(">>> 1. MÓDULO AUTENTICACIÓN")
    login_admin_res = make_request("POST", "/api/auth/login", {"login": "admin", "clave": "admin123"}, expected_status=200)
    admin_token = login_admin_res["response"].get("token") if login_admin_res["success"] else None
    
    login_client_res = make_request("POST", "/api/auth/login", {"login": "cliente", "clave": "cliente123"}, expected_status=200)
    client_token = login_client_res["response"].get("token") if login_client_res["success"] else None
    
    # Registro de nuevo usuario dinámico
    temp_login = f"test_{int(time.time())}"
    make_request("POST", "/api/auth/registro", {
        "nombre": "Prueba",
        "apellido": "EnVivo",
        "telefono": "78901234",
        "login": temp_login,
        "clave": "pass1234",
        "rolId": 2
    }, expected_status=201)
    
    # GET /api/auth/me
    make_request("GET", "/api/auth/me", token=admin_token, expected_status=200)
    
    # -------------------------------------------------------------
    # 2. CATEGORÍAS
    # -------------------------------------------------------------
    print("\n>>> 2. MÓDULO CATEGORÍAS")
    make_request("GET", "/api/categorias/lista", expected_status=200)
    make_request("GET", "/api/categorias?page=0&size=5", token=admin_token, expected_status=200)
    make_request("GET", "/api/categorias/1", expected_status=200)
    
    # Crear, modificar y eliminar categoría
    new_cat_res = make_request("POST", "/api/categorias", {
        "nombre": f"Cat Temp {int(time.time())}",
        "descripcion": "Categoría creada automáticamente para test",
        "iconoUrl": "spa",
        "tipo": "Servicio"
    }, token=admin_token, expected_status=201)
    
    created_cat_id = None
    if new_cat_res["success"] and isinstance(new_cat_res["response"], dict):
        created_cat_id = new_cat_res["response"].get("id")
        
    if created_cat_id:
        make_request("PUT", f"/api/categorias/{created_cat_id}", {
            "nombre": f"Cat Modificada {int(time.time())}",
            "descripcion": "Descripción actualizada",
            "iconoUrl": "brush",
            "tipo": "Servicio"
        }, token=admin_token, expected_status=200)
        make_request("DELETE", f"/api/categorias/{created_cat_id}", token=admin_token, expected_status=[200, 204])

    # -------------------------------------------------------------
    # 3. SERVICIOS
    # -------------------------------------------------------------
    print("\n>>> 3. MÓDULO SERVICIOS")
    make_request("GET", "/api/servicios/lista", expected_status=200)
    make_request("GET", "/api/servicios?page=0&size=5", expected_status=200)
    make_request("GET", "/api/servicios/1", expected_status=200)
    make_request("GET", "/api/servicios/categoria/1", expected_status=200)
    make_request("GET", "/api/servicios/buscar?query=corte", expected_status=200)
    
    # Crear, modificar y eliminar servicio
    new_srv_code = f"TMP-{int(time.time()) % 10000}"
    new_srv_res = make_request("POST", "/api/servicios", {
        "codigoServicio": new_srv_code,
        "categoriaId": 1,
        "nombre": f"Servicio Test {new_srv_code}",
        "descripcion": "Servicio temporal para test",
        "precioBase": 25.50,
        "esPrecioVariable": False,
        "duracionMinutos": 45,
        "intervaloSeguimientoDias": 30,
        "imagenUrl": "https://images.unsplash.com/photo-1560869713-7d0a29430803",
        "costoInsumos": 5.00
    }, token=admin_token, expected_status=201)
    
    created_srv_id = None
    if new_srv_res["success"] and isinstance(new_srv_res["response"], dict):
        created_srv_id = new_srv_res["response"].get("id")
        
    if created_srv_id:
        make_request("PUT", f"/api/servicios/{created_srv_id}", {
            "codigoServicio": new_srv_code,
            "categoriaId": 1,
            "nombre": f"Servicio Test Modificado {new_srv_code}",
            "descripcion": "Descripción actualizada",
            "precioBase": 30.00,
            "esPrecioVariable": False,
            "duracionMinutos": 60,
            "intervaloSeguimientoDias": 45,
            "imagenUrl": "https://images.unsplash.com/photo-1560869713-7d0a29430803",
            "costoInsumos": 6.00
        }, token=admin_token, expected_status=200)
        make_request("DELETE", f"/api/servicios/{created_srv_id}", token=admin_token, expected_status=[200, 204])

    # -------------------------------------------------------------
    # 4. ESTILISTAS Y DISPONIBILIDAD
    # -------------------------------------------------------------
    print("\n>>> 4. MÓDULO ESTILISTAS Y DISPONIBILIDAD")
    make_request("GET", "/api/estilistas", expected_status=200)
    make_request("GET", "/api/estilistas/1", expected_status=200)
    make_request("GET", "/api/estilistas/1/disponibilidad?fecha=2026-10-15&servicioId=1", expected_status=200)

    # -------------------------------------------------------------
    # 5. CITAS Y RESERVAS
    # -------------------------------------------------------------
    print("\n>>> 5. MÓDULO CITAS")
    make_request("GET", "/api/citas?page=0&size=5", token=admin_token, expected_status=200)
    make_request("GET", "/api/citas/timeline?fecha=2026-10-15", token=admin_token, expected_status=200)
    make_request("GET", "/api/citas/mis-citas", token=client_token, expected_status=200)
    
    # Crear una nueva cita con cliente autenticado
    epoch = int(time.time())
    minute_slot = "10:30" if (epoch % 2 == 0) else "11:00"
    cita_post_res = make_request("POST", "/api/citas", {
        "estilistaId": 1,
        "fechaCita": "2026-10-25",
        "horaInicio": minute_slot,
        "servicioIds": [1],
        "notasCliente": "Prueba automática de reserva desde script en vivo",
        "metodoPagoPreferente": "TARJETA"
    }, token=client_token, expected_status=201)
    
    created_cita_id = None
    created_cita_cod = None
    if cita_post_res["success"] and isinstance(cita_post_res["response"], dict):
        created_cita_id = cita_post_res["response"].get("id")
        created_cita_cod = cita_post_res["response"].get("codigoCita")

    if created_cita_id:
        make_request("GET", f"/api/citas/{created_cita_id}", token=admin_token, expected_status=200)
        if created_cita_cod:
            make_request("GET", f"/api/citas/codigo/{created_cita_cod}", token=admin_token, expected_status=200)
            
        make_request("PATCH", f"/api/citas/{created_cita_id}/estado", {
            "id": created_cita_id,
            "nuevoEstado": "CONFIRMADA",
            "motivoCancelacion": None
        }, token=admin_token, expected_status=200)

    # Crear cita presencial Walk-in
    walkin_res = make_request("POST", "/api/citas/walkin", {
        "nombreCliente": "Cliente Espontaneo Presencial",
        "telefonoCliente": "71112233",
        "estilistaId": 2,
        "fechaCita": "2026-10-26",
        "horaInicio": "14:00",
        "servicioIds": [3],
        "metodoPagoPreferente": "EFECTIVO",
        "notas": "Cita espontánea en recepción"
    }, token=admin_token, expected_status=201)

    walkin_cita_id = walkin_res["response"].get("id") if walkin_res["success"] else None

    # -------------------------------------------------------------
    # 6. PAGOS Y FACTURACIÓN
    # -------------------------------------------------------------
    print("\n>>> 6. MÓDULO PAGOS Y FACTURACIÓN")
    cita_para_pago = walkin_cita_id or created_cita_id
    if cita_para_pago:
        pago_res = make_request("POST", "/api/pagos", {
            "citaId": cita_para_pago,
            "monto": 22.00,
            "metodoPago": "EFECTIVO",
            "referenciaPos": "REC-778899",
            "motivoAjustePrecio": None
        }, token=admin_token, expected_status=201)
        
        # Consultar factura por ID de cita
        factura_res = make_request("GET", f"/api/pagos/factura/cita/{cita_para_pago}", token=admin_token, expected_status=200)
        num_factura = None
        if factura_res["success"] and isinstance(factura_res["response"], dict):
            num_factura = factura_res["response"].get("numeroFactura")
            
        if num_factura:
            make_request("GET", f"/api/pagos/factura/{num_factura}", token=admin_token, expected_status=200)

    # -------------------------------------------------------------
    # 7. DASHBOARD ADMINISTRATIVO
    # -------------------------------------------------------------
    print("\n>>> 7. MÓDULO DASHBOARD ADMINISTRATIVO")
    make_request("GET", "/api/admin/dashboard", token=admin_token, expected_status=200)
    
    # -------------------------------------------------------------
    # RESUMEN DE EJECUCIÓN
    # -------------------------------------------------------------
    total = len(results)
    passed = sum(1 for r in results if r["success"])
    failed = total - passed
    avg_latency = round(sum(r["duration_ms"] for r in results) / total, 2)
    
    print("\n=================================================================")
    print("📊 RESUMEN FINAL DE PRUEBAS")
    print(f"Total de endpoints probados: {total}")
    print(f"Exitosos (PASS): {passed} ✅")
    print(f"Fallidos (FAIL): {failed} ❌")
    print(f"Tasa de éxito: {round((passed/total)*100, 2)}%")
    print(f"Latencia promedio: {avg_latency} ms")
    print("=================================================================")
    
    with open("/tmp/live_api_test_results.json", "w") as f:
        json.dump(results, f, indent=2)

if __name__ == "__main__":
    run_all_tests()
