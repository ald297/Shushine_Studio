#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Prueba de Estrés y Concurrencia Transaccional (RNF02) — Shushine Studio Web API
Simula múltiples peticiones HTTP concurrentes en el mismo milisegundo intentando reservar
el mismo estilista en el mismo slot horario para verificar la prevención estricta de doble reserva (ACID).
"""

import json
import time
import urllib.request
import urllib.error
import ssl
from concurrent.futures import ThreadPoolExecutor, as_completed

BASE_URL = "https://shushine-studio.onrender.com"
CTX = ssl.create_default_context()

def get_token(login, clave):
    url = f"{BASE_URL}/api/auth/login"
    data = json.dumps({"login": login, "clave": clave}).encode("utf-8")
    req = urllib.request.Request(url, data=data, headers={"Content-Type": "application/json"}, method="POST")
    with urllib.request.urlopen(req, context=CTX, timeout=15) as res:
        res_data = json.loads(res.read().decode("utf-8"))
        return res_data["token"]

def make_booking_request(thread_id, token, payload, barrier_time):
    # Sincronización en el milisegundo exacto
    sleep_needed = barrier_time - time.time()
    if sleep_needed > 0:
        time.sleep(sleep_needed)
        
    url = f"{BASE_URL}/api/citas"
    data = json.dumps(payload).encode("utf-8")
    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
        "Authorization": f"Bearer {token}"
    }
    
    req = urllib.request.Request(url, data=data, headers=headers, method="POST")
    start = time.time()
    status_code = None
    response_body = None
    
    try:
        with urllib.request.urlopen(req, context=CTX, timeout=20) as res:
            status_code = res.status
            response_body = json.loads(res.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        status_code = e.code
        try:
            response_body = json.loads(e.read().decode("utf-8"))
        except Exception:
            response_body = {"detail": str(e)}
    except Exception as e:
        status_code = 0
        response_body = {"error": str(e)}
        
    duration_ms = round((time.time() - start) * 1000, 2)
    return {
        "thread_id": thread_id,
        "status_code": status_code,
        "duration_ms": duration_ms,
        "response": response_body
    }

def run_concurrency_test():
    print("=================================================================")
    print("⚡ PRUEBA DE ESTRÉS Y CONCURRENCIA TRANSACCIONAL (RNF02)")
    print(f"Objetivo: {BASE_URL}/api/citas")
    print("=================================================================\n")
    
    print("1. Obteniendo token JWT para usuario cliente...")
    token = get_token("cliente", "cliente123")
    print("✅ Token obtenido exitosamente.\n")
    
    # Parámetros del slot horario a disputar
    target_date = "2026-11-15"
    target_time = "10:00"
    stylist_id = 1
    
    payload = {
        "estilistaId": stylist_id,
        "fechaCita": target_date,
        "horaInicio": target_time,
        "servicioIds": [1],
        "notasCliente": "Prueba de concurrencia simultánea RNF02",
        "metodoPagoPreferente": "TARJETA"
    }
    
    num_threads = 10
    print(f"2. Preparando {num_threads} peticiones simultáneas...")
    print(f"   - Estilista ID: {stylist_id}")
    print(f"   - Fecha disputada: {target_date}")
    print(f"   - Horario disputado: {target_time}\n")
    
    # Fijar barrera de sincronización en 2 segundos en el futuro
    barrier_time = time.time() + 2.0
    
    results = []
    with ThreadPoolExecutor(max_workers=num_threads) as executor:
        futures = [
            executor.submit(make_booking_request, i + 1, token, payload, barrier_time)
            for i in range(num_threads)
        ]
        for f in as_completed(futures):
            results.append(f.result())
            
    print("3. Resultados de las peticiones concurrentes:")
    print("-----------------------------------------------------------------")
    print(f"{'Hilo':<8} | {'Status Code':<12} | {'Latencia':<10} | {'Resultado / Detalle'}")
    print("-----------------------------------------------------------------")
    
    success_count = 0
    conflict_count = 0
    
    for r in sorted(results, key=lambda x: x["thread_id"]):
        code = r["status_code"]
        dur = f"{r['duration_ms']} ms"
        tid = f"Hilo #{r['thread_id']}"
        detail = ""
        
        if code == 201:
            success_count += 1
            cita_cod = r["response"].get("codigoCita", "N/A")
            detail = f"✅ CITA CREADA EXITOSA ({cita_cod})"
        elif code in (409, 400):
            conflict_count += 1
            msg = r["response"].get("detail", r["response"].get("title", "Conflicto de horario"))
            detail = f"🛡️ BLOQUEO CONCURRENCIA ({msg})"
        else:
            detail = f"❌ Respuesta inesperada: {r['response']}"
            
        print(f"{tid:<8} | {code:<12} | {dur:<10} | {detail}")
        
    print("-----------------------------------------------------------------")
    print(f"\n📊 RESUMEN TRANSACCIONAL RNF02:")
    print(f"- Peticiones totales disparadas: {num_threads}")
    print(f"- Reservas aprobadas (201 Created): {success_count} (Esperado: 1)")
    print(f"- Reservas bloqueadas por solapamiento (409 Conflict): {conflict_count} (Esperado: {num_threads - 1})")
    
    concurrency_passed = (success_count == 1 and conflict_count == (num_threads - 1))
    if concurrency_passed:
        print("\n🏆 CERTIFICACIÓN ACID RNF02: ¡PRUEBA SUPERADA CON ÉXITO ABSOLUTO!")
        print("El motor transaccional de Shushine Studio garantiza la prevención total de overbooking.")
    else:
        print(f"\n⚠️ Resultado no esperado: success={success_count}, conflict={conflict_count}")
        
    # Guardar reporte en JSON
    with open("/tmp/concurrency_test_results.json", "w") as f:
        json.dump({
            "targetDate": target_date,
            "targetTime": target_time,
            "totalRequests": num_threads,
            "successCount": success_count,
            "conflictCount": conflict_count,
            "passed": concurrency_passed,
            "details": results
        }, f, indent=2)

if __name__ == "__main__":
    run_concurrency_test()
