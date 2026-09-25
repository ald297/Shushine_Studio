#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para actualizar el estado de los Work Items de Alex a 'Done' en Azure Boards.
Uso:
    python3 scripts/update_azure_boards_done.py <AZURE_DEVOPS_PAT>
"""

import sys
import os
import json
import base64
import urllib.request
import urllib.error
import urllib.parse

ORGANIZATION = "cc25003"
PROJECT = "Shunshine Studio"
PROJECT_ENC = urllib.parse.quote(PROJECT)

def get_pat():
    if len(sys.argv) > 1:
        return sys.argv[1].strip()
    return os.environ.get("AZURE_DEVOPS_PAT", "").strip()

def search_and_update_work_items(pat):
    auth_str = f":{pat}"
    b64_auth = base64.b64encode(auth_str.encode("utf-8")).decode("utf-8")
    headers = {
        "Authorization": f"Basic {b64_auth}",
        "Content-Type": "application/json"
    }

    # 1. Consultar work items asignados a Alex mediante WIQL
    wiql_url = f"https://dev.azure.com/{ORGANIZATION}/{PROJECT_ENC}/_apis/wit/wiql?api-version=7.0"
    query = {
        "query": f"SELECT [System.Id], [System.Title], [System.State], [System.WorkItemType] FROM WorkItems WHERE [System.AreaPath] UNDER '{PROJECT}' ORDER BY [System.Id]"
    }
    
    req = urllib.request.Request(wiql_url, data=json.dumps(query).encode("utf-8"), headers=headers, method="POST")
    try:
        with urllib.request.urlopen(req) as resp:
            data = json.loads(resp.read().decode("utf-8"))
            work_items = data.get("workItems", [])
            print(f"📋 Total de Work Items encontrados en Azure Boards: {len(work_items)}")
    except urllib.error.HTTPError as e:
        print(f"❌ Error al consultar Work Items: HTTP {e.code} - {e.read().decode('utf-8')}")
        return
    except Exception as e:
        print(f"❌ Error de conexión: {e}")
        return

    if not work_items:
        print("⚠️ No hay Work Items creados en el tablero aún. Recuerda importar 'azure_boards_import.csv' en Boards -> Work Items -> Import.")
        return

    # Lista de IDs
    ids = [str(item["id"]) for item in work_items]
    
    # Procesar en lotes de 100
    for chunk in [ids[i:i+100] for i in range(0, len(ids), 100)]:
        ids_str = ",".join(chunk)
        details_url = f"https://dev.azure.com/{ORGANIZATION}/{PROJECT_ENC}/_apis/wit/workitems?ids={ids_str}&api-version=7.0"
        det_req = urllib.request.Request(details_url, headers=headers)
        
        try:
            with urllib.request.urlopen(det_req) as resp:
                details = json.loads(resp.read().decode("utf-8")).get("value", [])
        except Exception as e:
            print(f"Error al obtener detalles: {e}")
            continue

        for item in details:
            wi_id = item["id"]
            fields = item.get("fields", {})
            title = fields.get("System.Title", "")
            assigned = fields.get("System.AssignedTo", {}).get("displayName", "") if isinstance(fields.get("System.AssignedTo"), dict) else str(fields.get("System.AssignedTo", ""))
            wi_type = fields.get("System.WorkItemType", "")
            current_state = fields.get("System.State", "")

            # Si está asignado a Alex y es una tarea del backend ya completada
            is_alex = "Alex" in assigned or "ald297" in assigned or "alfaro" in assigned.lower()
            if is_alex and current_state not in ("Done", "Closed", "Resolved"):
                new_state = "Done"
                
                # Actualizar mediante JSON Patch
                patch_url = f"https://dev.azure.com/{ORGANIZATION}/{PROJECT_ENC}/_apis/wit/workitems/{wi_id}?api-version=7.0"
                patch_headers = {
                    "Authorization": f"Basic {b64_auth}",
                    "Content-Type": "application/json-patch+json"
                }
                patch_body = [
                    {"op": "add", "path": "/fields/System.State", "value": new_state}
                ]
                
                patch_req = urllib.request.Request(patch_url, data=json.dumps(patch_body).encode("utf-8"), headers=patch_headers, method="PATCH")
                try:
                    with urllib.request.urlopen(patch_req) as p_resp:
                        print(f"✅ WI #{wi_id} [{wi_type}] '{title[:45]}...' -> {new_state}")
                except Exception as e:
                    print(f"⚠️ Error actualizando WI #{wi_id}: {e}")

def main():
    pat = get_pat()
    if not pat:
        print("❌ Error: Se requiere el token PAT de Azure DevOps.")
        print("Uso: python3 scripts/update_azure_boards_done.py <AZURE_DEVOPS_PAT>")
        sys.exit(1)
        
    print(f"🚀 Conectando a Azure Boards ({ORGANIZATION}/{PROJECT})...")
    search_and_update_work_items(pat)

if __name__ == "__main__":
    main()
