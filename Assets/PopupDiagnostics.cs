using UnityEngine;

// Attach to Canvas_Maklumat or scene to debug popup visibility issues
public class PopupDiagnostics : MonoBehaviour
{
    [ContextMenu("Check Canvas_Maklumat Setup")]
    public void DiagnosticsReport()
    {
        Debug.Log("========== POPUP DIAGNOSTICS REPORT ==========");

        // Check Canvas_Maklumat
        var canvasMaklumat = GameObject.Find("Canvas_Maklumat");
        if (canvasMaklumat == null)
        {
            Debug.LogError("❌ Canvas_Maklumat not found in scene!");
            return;
        }
        Debug.Log("✓ Canvas_Maklumat found");

        Canvas canvas = canvasMaklumat.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas component missing on Canvas_Maklumat!");
            return;
        }

        Debug.Log($"Canvas RenderMode: {canvas.renderMode} (0=Overlay, 1=Camera, 2=WorldSpace)");
        Debug.Log($"Canvas Active: {canvasMaklumat.activeSelf}");
        Debug.Log($"Canvas SortingOrder: {canvas.sortingOrder}");
        Debug.Log($"Canvas OverrideSorting: {canvas.overrideSorting}");
        Debug.Log($"Canvas Position: {canvasMaklumat.transform.position}");
        Debug.Log($"Canvas Rotation: {canvasMaklumat.transform.rotation}");
        Debug.Log($"Canvas Scale: {canvasMaklumat.transform.localScale}");
        
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Debug.Log($"Canvas WorldCamera: {canvas.worldCamera?.name ?? "NULL"}");
        }

        // Check PengesanSentuhAR
        var pengesan = FindObjectOfType<PengesanSentuhAR>();
        if (pengesan == null)
        {
            Debug.LogError("❌ PengesanSentuhAR not found in scene!");
        }
        else
        {
            Debug.Log("✓ PengesanSentuhAR found");
            var canvasField = pengesan.GetType().GetField("canvasMaklumat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (canvasField != null)
            {
                var assignedCanvas = canvasField.GetValue(pengesan) as RectTransform;
                Debug.Log($"  canvasMaklumat assigned: {(assignedCanvas != null ? assignedCanvas.name : "NULL")}");
            }
        }

        // Check PemegangDataSensor
        var sensors = FindObjectsByType<PemegangDataSensor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"✓ Found {sensors.Length} PemegangDataSensor objects");
        foreach (var sensor in sensors)
        {
            bool hasCollider = sensor.GetComponent<Collider>() != null;
            bool hasChildCollider = sensor.GetComponentInChildren<Collider>() != null;
            Debug.Log($"  - {sensor.name}: hasCollider={hasCollider}, hasChildCollider={hasChildCollider}, diseases={sensor.senaraiPenyakit.Length}");
        }

        // Check KadMaklumatUI
        var kadUI = FindObjectOfType<KadMaklumatUI>();
        if (kadUI == null)
        {
            Debug.LogWarning("⚠ KadMaklumatUI not found (it's usually on cards, should be OK)");
        }
        else
        {
            Debug.Log("✓ KadMaklumatUI found");
        }

        // Check Raycast from camera
        var arKamera = Camera.main;
        if (arKamera == null)
        {
            Debug.LogError("❌ No Camera.main found!");
        }
        else
        {
            Debug.Log($"✓ Camera.main found at {arKamera.name}");
            Ray ray = arKamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.Log($"  Raycast from center hits: {hit.transform.name}");
            }
            else
            {
                Debug.LogWarning("  ⚠ Center raycast misses all objects");
            }
        }

        Debug.Log("========== END DIAGNOSTICS ==========");
    }
}
