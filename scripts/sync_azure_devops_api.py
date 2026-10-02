#!/usr/bin/env python3
"""
Script para sincronización automática de Épicas, Historias y Tareas hacia Azure DevOps Boards.
Uso:
    python3 scripts/sync_azure_devops_api.py <PERSONAL_ACCESS_TOKEN>
    o configurando la variable de entorno:
    export AZURE_DEVOPS_PAT="tu_token_aqui"
    python3 scripts/sync_azure_devops_api.py
"""

import sys
import os
import json
import base64
import urllib.request
import urllib.error
import csv

ORGANIZATION = "cc25003"
PROJECT = "Shunshine Studio"
BASE_URL = f"https://dev.azure.com/{ORGANIZATION}/{PROJECT}/_apis/wit/workitems"

def get_pat():
    if len(sys.argv) > 1:
        return sys.argv[1].strip()
    pat = os.getenv("AZURE_DEVOPS_PAT")
    if pat:
        return pat.strip()
    return None

def create_work_item(pat, item_type, title, assigned_to, iteration, description, criteria="", points="", hours=""):
    url = f"{BASE_URL}/${item_type}?api-version=7.0"
    
    # Preparar el cuerpo del Work Item según el estándar JSON Patch de Azure DevOps
    patch_doc = [
        {"op": "add", "path": "/fields/System.Title", "value": title},
        {"op": "add", "path": "/fields/System.AreaPath", "value": PROJECT},
        {"op": "add", "path": "/fields/System.IterationPath", "value": iteration},
        {"op": "add", "path": "/fields/System.Description", "value": description}
    ]
    
    if assigned_to:
        patch_doc.append({"op": "add", "path": "/fields/System.AssignedTo", "value": assigned_to})
        
    if criteria and item_type == "User Story":
        patch_doc.append({"op": "add", "path": "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", "value": criteria})
        
    if points and item_type == "User Story":
        try:
            patch_doc.append({"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.StoryPoints", "value": float(points)})
        except ValueError:
            pass

    if hours and item_type == "Task":
        try:
            patch_doc.append({"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.OriginalEstimate", "value": float(hours)})
            patch_doc.append({"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.RemainingWork", "value": float(hours)})
        except ValueError:
            pass

    auth_str = f":{pat}"
    b64_auth = base64.b64encode(auth_str.encode("utf-8")).decode("utf-8")
    
    headers = {
        "Content-Type": "application/json-patch+json",
        "Authorization": f"Basic {b64_auth}"
    }
    
    data = json.dumps(patch_doc).encode("utf-8")
    req = urllib.request.Request(url, data=data, headers=headers, method="POST")
    
    try:
        with urllib.request.urlopen(req) as response:
            res_data = json.loads(response.read().decode("utf-8"))
            return True, res_data.get("id"), None
    except urllib.error.HTTPError as e:
        err_content = e.read().decode("utf-8")
        return False, None, f"HTTP {e.code}: {err_content}"
    except Exception as e:
        return False, None, str(e)

def main():
    pat = get_pat()
    if not pat:
        print("\n❌ Error: No se proporcionó un Personal Access Token (PAT) de Azure DevOps.")
        print("Uso:")
        print("    python3 scripts/sync_azure_devops_api.py <TU_PAT>")
        print("O configurar variable:")
        print("    export AZURE_DEVOPS_PAT='tu_token'")
        print("    python3 scripts/sync_azure_devops_api.py\n")
        print("Alternativa directa recomendada:")
        print("    Importar directamente el archivo 'azure_boards_import.csv' en:")
        print("    Azure DevOps -> Boards -> Work Items -> Import Work Items.\n")
        sys.exit(1)

    csv_path = os.path.join(os.path.dirname(__file__), "..", "azure_boards_import.csv")
    if not os.path.exists(csv_path):
        print(f"❌ Error: Archivo {csv_path} no encontrado.")
        sys.exit(1)

    print(f"🚀 Iniciando sincronización con Azure DevOps ({ORGANIZATION}/{PROJECT})...\n")
    
    success_count = 0
    fail_count = 0

    with open(csv_path, mode="r", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            item_type = row["Work Item Type"]
            title = row["Title"]
            assigned = row["Assigned To"]
            iteration = row["Iteration Path"]
            desc = row["Description"]
            criteria = row.get("Acceptance Criteria", "")
            points = row.get("Story Points", "")
            hours = row.get("Original Estimate", "")

            # Mapear nombre estándar para User Story / Task en Azure DevOps
            api_type = item_type
            if item_type == "User Story":
                api_type = "User Story"

            print(f"Subiendo [{item_type}] {title[:60]}...", end=" ")
            ok, item_id, err = create_work_item(pat, api_type, title, assigned, iteration, desc, criteria, points, hours)
            
            if ok:
                print(f"✅ Creado (ID: {item_id})")
                success_count += 1
            else:
                print(f"❌ Falló: {err}")
                fail_count += 1

    print(f"\n✨ Proceso finalizado: {success_count} creados, {fail_count} fallidos.")

if __name__ == "__main__":
    main()
