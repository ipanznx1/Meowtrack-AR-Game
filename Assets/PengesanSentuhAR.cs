using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PengesanSentuhAR : MonoBehaviour
{
    [Header("Kamera AR")]
    public Camera arKamera;

    [Header("Tetapan Pergerakan Canvas")]
    public RectTransform canvasMaklumat;
    [Tooltip("If true and the canvas is WorldSpace, attach the canvas to the hit transform so the popup follows the cat/object.")]
    public bool attachCanvasToHit = true;
    [Tooltip("If true, always detach the canvas to scene root before showing so other popup systems won't deactivate its parent.")]
    public bool keepCanvasAlwaysVisible = true;
    [Tooltip("If true, lock the canvas world position after it's shown so it won't move again on subsequent interactions.")]
    public bool lockCanvasAfterShow = true;
    public Vector3 jarakOffset = new Vector3(0.15f, 0.15f, 0f);
    public float worldSpaceDistance = 0.8f;
    public float worldSpaceHeight = 0.2f;
    public bool detachWorldSpaceCanvas = true;
    [Tooltip("Lateral offset (meters) applied to world-space popup to shift it left/right relative to camera right vector.")]
    public float lateralOffset = -0.15f;
    [Tooltip("Lateral offset (pixels) applied to screen-space popup to shift it left/right on screen.")]
    public float screenLateralOffset = -40f;
    public Vector2 screenOffset = new Vector2(40f, 20f);
    [Tooltip("If true, preserve the Canvas_Maklumat transform as positioned in the Editor and do not auto-reposition or reparent it at runtime.")]
    public bool respectEditorTransform = false;

    [Header("Tetapan Cetakan Kad (Carousel)")]
    public GameObject kadPrefab;         // Masukkan Kad_Penyakit_Prefab di sini
    public Transform tempatSusunKad;     // Masukkan objek "Content" dari ScrollView di sini

    private Coroutine animasiSekarang;
    private PemegangDataSensor sensorAktif;
    private Vector3 skalaAsalCanvas;     // Untuk simpan saiz asal Canvas (contoh: 0.001)
    private bool infoPopupVisible = false;
    private Vector3 lastHitPoint = Vector3.zero;
    private bool canvasLocked = false;

    void Start()
    {
        if (arKamera == null)
        {
            arKamera = Camera.main;
            if (arKamera == null)
            {
                Debug.LogWarning("[PengesanSentuhAR] arKamera was not assigned and Camera.main is null.");
            }
        }

        if (canvasMaklumat == null)
        {
            Debug.LogError("[PengesanSentuhAR] canvasMaklumat is not assigned. The health info popup cannot show.");
        }
        if (kadPrefab == null)
        {
            Debug.LogError("[PengesanSentuhAR] kadPrefab is not assigned. The health info cards cannot spawn.");
        }
        if (tempatSusunKad == null)
        {
            Debug.LogError("[PengesanSentuhAR] tempatSusunKad is not assigned. The health info carousel has no container.");
        }

        // Simpan saiz asal Canvas supaya animasi tak jadikan ia gergasi
        if (canvasMaklumat != null)
        {
            skalaAsalCanvas = canvasMaklumat.localScale;
            Canvas canvas = canvasMaklumat.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;

                // Respect designer preference: if we're preserving editor transform, don't detach or move the canvas at Start.
                if (!respectEditorTransform && canvas.renderMode == RenderMode.WorldSpace && detachWorldSpaceCanvas)
                {
                    canvasMaklumat.SetParent(null, true);
                    Debug.Log("[PengesanSentuhAR] Detached world-space canvas to scene root at Start to keep popup stable.");
                }
            }
        }
    }

    void Update()
    {
        bool adaTekanan = false;
        Vector2 posisiTekanan = Vector2.zero;

        // Try Unity Input System first
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            adaTekanan = true;
            posisiTekanan = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log("[PengesanSentuhAR] Touchscreen input detected via Input System.");
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            adaTekanan = true;
            posisiTekanan = Mouse.current.position.ReadValue();
            Debug.Log("[PengesanSentuhAR] Mouse input detected via Input System.");
        }

        // Legacy input fallback, useful if the Input System is not available or configured differently
#if ENABLE_LEGACY_INPUT_MANAGER
        if (!adaTekanan)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
            {
                adaTekanan = true;
                posisiTekanan = Input.GetTouch(0).position;
                Debug.Log("[PengesanSentuhAR] Touch input detected via Legacy Input.");
            }
            else if (Input.GetMouseButtonDown(0))
            {
                adaTekanan = true;
                posisiTekanan = Input.mousePosition;
                Debug.Log("[PengesanSentuhAR] Mouse input detected via Legacy Input.");
            }
        }
#endif

        if (!adaTekanan && Input.touchCount > 0)
        {
            // Non-legacy touch check for builds where Input System may still expose Input.touchCount.
            Touch touch = Input.GetTouch(0);
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                adaTekanan = true;
                posisiTekanan = touch.position;
                Debug.Log("[PengesanSentuhAR] Touch input detected via Input.touchCount fallback.");
            }
        }

        if (adaTekanan)
        {
            if (arKamera == null)
            {
                Debug.LogWarning("[PengesanSentuhAR] No AR camera assigned on touch.");
            }
            else
            {
                Ray ray = arKamera.ScreenPointToRay(posisiTekanan);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    PemegangDataSensor sensor = FindSensorFromHit(hit.transform);

                    if (sensor != null)
                    {
                        // 1. Simpan sensor ini sebagai sensor yang 'Aktif' sekarang
                        sensorAktif = sensor;

                        // 2. Ubah kedudukan Canvas ke lokasi sensor yang disentuh setiap kali
                        if (canvasMaklumat != null)
                        {
                                if (!canvasLocked)
                                {
                                    PositionCanvasAtHitPoint(hit.point, hit.transform);
                                }
                            lastHitPoint = hit.point;
                        }

                        // 3. Tunjuk SEMUA data dalam bentuk kad bertindan
                        bool shown = PaparkanData(sensorAktif);
                        infoPopupVisible = shown;
                    }
                    else
                    {
                        Debug.Log("[PengesanSentuhAR] Raycast hit " + hit.transform.name + " (layer=" + LayerMask.LayerToName(hit.transform.gameObject.layer) + ") but no PemegangDataSensor found.");
                    }
                }
                else
                {
                    Debug.Log("[PengesanSentuhAR] Touch raycast missed all colliders.");
                }
            }
        }

        if (infoPopupVisible && canvasMaklumat != null)
        {
            KeepCanvasFacingCamera();
        }
    }

    private void PositionCanvasAtHitPoint(Vector3 hitPoint, Transform hitTransform)
    {
        if (canvasMaklumat == null)
        {
            Debug.LogWarning("[PengesanSentuhAR] canvasMaklumat is null.");
            return;
        }

        Canvas canvas = canvasMaklumat.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            if (respectEditorTransform)
            {
                // Designer wants the canvas left where they placed it in the scene. Activate and ensure scale/rotation are sane.
                canvasMaklumat.gameObject.SetActive(true);
                canvasMaklumat.localScale = skalaAsalCanvas;
                if (arKamera != null)
                    canvasMaklumat.rotation = Quaternion.LookRotation(arKamera.transform.position - canvasMaklumat.position);
                Debug.Log("[PengesanSentuhAR] respectEditorTransform is true — preserving editor-placed canvas transform at runtime.");
                return;
            }
            Vector3 directionFromCamera = (hitPoint - arKamera.transform.position).normalized;
            Vector3 worldPosition = hitPoint + directionFromCamera * worldSpaceDistance + arKamera.transform.right * lateralOffset + new Vector3(0f, worldSpaceHeight, 0f);

            if (keepCanvasAlwaysVisible || detachWorldSpaceCanvas)
            {
                if (canvasMaklumat.parent != null)
                {
                    canvasMaklumat.SetParent(null, true);
                }
                canvasMaklumat.position = worldPosition;
            }
            else if (attachCanvasToHit && hitTransform != null)
            {
                if (canvasMaklumat.parent != hitTransform)
                {
                    canvasMaklumat.SetParent(hitTransform, true);
                }
                canvasMaklumat.localPosition = hitTransform.InverseTransformPoint(worldPosition);
            }
            else
            {
                if (canvasMaklumat.parent != null)
                {
                    canvasMaklumat.SetParent(null, true);
                }
                canvasMaklumat.position = worldPosition;
            }

            canvasMaklumat.rotation = Quaternion.LookRotation(arKamera.transform.position - canvasMaklumat.position);
            canvasMaklumat.localScale = skalaAsalCanvas;
            canvasMaklumat.gameObject.SetActive(true);
            Debug.Log("[PengesanSentuhAR] Positioned world-space canvas at: " + canvasMaklumat.position + " (attached=" + attachCanvasToHit + ")");
            return;
        }

        if (arKamera == null)
        {
            Debug.LogWarning("[PengesanSentuhAR] arKamera is null — cannot position canvas.");
            return;
        }

        Vector3 screenPos = arKamera.WorldToScreenPoint(hitPoint + jarakOffset);
        screenPos += (Vector3)screenOffset + new Vector3(screenLateralOffset, 0f, 0f);
        RectTransform parentRect = canvasMaklumat.parent as RectTransform;
        if (parentRect == null && canvasMaklumat.parent != null)
            parentRect = canvasMaklumat.parent.GetComponent<RectTransform>();

        if (parentRect == null)
        {
            parentRect = canvasMaklumat;
            Debug.Log("[PengesanSentuhAR] Using canvasMaklumat rect as root rectangle because canvas has no parent.");
        }

        if (parentRect == null)
        {
            Debug.LogWarning("[PengesanSentuhAR] Root canvas RectTransform not found — cannot compute anchoredPosition.");
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, arKamera, out Vector2 localPoint))
        {
            canvasMaklumat.anchoredPosition = localPoint;
            Debug.Log("[PengesanSentuhAR] Canvas positioned at anchoredPosition: " + canvasMaklumat.anchoredPosition);
        }
        else
        {
            Debug.LogWarning("[PengesanSentuhAR] ScreenPointToLocalPointInRectangle failed to compute localPoint.");
        }
    }

    // --- FUNGSI UNTUK CETAK KAD CAROUSEL ---
    private bool PaparkanData(PemegangDataSensor sensor)
    {
        if (sensor == null)
        {
            Debug.LogWarning("[PengesanSentuhAR] PaparkanData called with null sensor.");
            return false;
        }

        if (sensor.senaraiPenyakit == null || sensor.senaraiPenyakit.Length == 0)
        {
            Debug.LogWarning("[PengesanSentuhAR] PemegangDataSensor has no penyakit data assigned.");
            return false;
        }

        Debug.Log("[PengesanSentuhAR] Showing " + sensor.senaraiPenyakit.Length + " penyakit cards for sensor " + sensor.name + ".");

        if (tempatSusunKad == null)
        {
            Debug.LogError("[PengesanSentuhAR] tempatSusunKad is null — cannot display disease cards.");
            return false;
        }

        if (kadPrefab == null)
        {
            Debug.LogError("[PengesanSentuhAR] kadPrefab is null — cannot instantiate disease cards.");
            return false;
        }

        // 1. Padam semua kad lama yang ada dalam ScrollView supaya tak bertindih
        foreach (Transform child in tempatSusunKad)
        {
            Destroy(child.gameObject);
        }

        // 2. Cetak kad baru satu per satu berdasarkan senarai penyakit yang ada
        foreach (PenyakitData data in sensor.senaraiPenyakit)
        {
            if (data == null) continue;

            GameObject kadBaru = Instantiate(kadPrefab, tempatSusunKad);
            KadMaklumatUI skripKad = kadBaru.GetComponent<KadMaklumatUI>();
            if (skripKad != null)
            {
                skripKad.IsikanData(data);
            }
            else
            {
                Debug.LogWarning("[PengesanSentuhAR] Instantiated card prefab does not contain KadMaklumatUI.");
            }
        }

        CoverFlowUI cf = tempatSusunKad.GetComponent<CoverFlowUI>();
        if (cf != null)
        {
            cf.RefreshList();
        }
        else
        {
            Debug.Log("[PengesanSentuhAR] No CoverFlowUI found on tempatSusunKad; card list will still be shown normally.");
        }

        if (canvasMaklumat != null)
        {
            // If designer marked that the editor-placed transform should be respected, just show the canvas as-is
            if (respectEditorTransform)
            {
                canvasMaklumat.gameObject.SetActive(true);
                canvasMaklumat.localScale = skalaAsalCanvas;
                if (arKamera != null)
                    canvasMaklumat.rotation = Quaternion.LookRotation(arKamera.transform.position - canvasMaklumat.position);
                if (animasiSekarang != null) StopCoroutine(animasiSekarang);
                animasiSekarang = StartCoroutine(EfekMelantunUI());
                if (lockCanvasAfterShow)
                {
                    canvasLocked = true;
                    Debug.Log("[PengesanSentuhAR] Canvas locked (respectEditorTransform) at editor position: " + canvasMaklumat.position);
                }

                return true;
            }

            // Ensure canvas is forced to WorldSpace and detached to scene root so UI layout/scroll changes won't move it.
            Canvas parentCanvas = canvasMaklumat.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                if (parentCanvas.renderMode != RenderMode.WorldSpace)
                {
                    parentCanvas.renderMode = RenderMode.WorldSpace;
                    if (arKamera != null) parentCanvas.worldCamera = arKamera;
                    Debug.Log("[PengesanSentuhAR] Forcing parent Canvas to WorldSpace to stabilize popup.");
                }
            }

            if (keepCanvasAlwaysVisible)
            {
                if (canvasMaklumat.parent != null)
                {
                    canvasMaklumat.SetParent(null, true);
                    Debug.Log("[PengesanSentuhAR] Detached canvasMaklumat to scene root to keep it visible.");
                }
            }

            // If we have a recent hit point, position the canvas there in world space so it won't shift with layout
            if (lastHitPoint != Vector3.zero && arKamera != null)
            {
                Vector3 dir = (lastHitPoint - arKamera.transform.position).normalized;
                Vector3 worldPos = lastHitPoint + dir * worldSpaceDistance + new Vector3(0f, worldSpaceHeight, 0f);
                canvasMaklumat.position = worldPos;
                canvasMaklumat.rotation = Quaternion.LookRotation(arKamera.transform.position - worldPos);
                canvasMaklumat.localScale = skalaAsalCanvas;
            }

            canvasMaklumat.gameObject.SetActive(true);
            if (animasiSekarang != null) StopCoroutine(animasiSekarang);
            animasiSekarang = StartCoroutine(EfekMelantunUI());
            // Optionally lock canvas position so it won't be repositioned by further touches
            if (lockCanvasAfterShow)
            {
                canvasLocked = true;
                Debug.Log("[PengesanSentuhAR] Canvas locked after showing at position: " + canvasMaklumat.position);
            }
        }

        return true;
    }

    private void KeepCanvasFacingCamera()
    {
        if (arKamera == null || canvasMaklumat == null) return;
        if (respectEditorTransform) return;

        Canvas canvas = canvasMaklumat.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvasMaklumat.rotation = Quaternion.LookRotation(arKamera.transform.position - canvasMaklumat.position);
        }
    }

    private PemegangDataSensor FindSensorFromHit(Transform hitTransform)
    {
        if (hitTransform == null) return null;

        // Direct component on the hit object.
        PemegangDataSensor sensor = hitTransform.GetComponent<PemegangDataSensor>();
        if (sensor != null) return sensor;

        // Try parent chain (useful if collider is on child object).
        sensor = hitTransform.GetComponentInParent<PemegangDataSensor>();
        if (sensor != null) return sensor;

        // Try children as a last resort.
        return hitTransform.GetComponentInChildren<PemegangDataSensor>();
    }

    // Debug helper: force show the first available sensor's info cards (useful for mobile testing)
    public bool DebugShowFirstSensor()
    {
        var first = FindObjectOfType<PemegangDataSensor>();
        if (first == null)
        {
            Debug.LogWarning("[PengesanSentuhAR] No PemegangDataSensor found for DebugShowFirstSensor().");
            return false;
        }

        Debug.Log("[PengesanSentuhAR] DebugShowFirstSensor() called. Showing cards for: " + first.name);
        return PaparkanData(first);
    }

    // --- ANIMASI MELANTUN (BOUNCE) ---
    IEnumerator EfekMelantunUI()
    {
        canvasMaklumat.gameObject.SetActive(true);
        canvasMaklumat.localScale = Vector3.zero; // Mula dari saiz 0

        float masa = 0f;
        float durasi = 0.35f;

        while (masa < durasi)
        {
            masa += Time.deltaTime;
            float peratus = masa / durasi;
            float bounce = 1f + 2.70158f * Mathf.Pow(peratus - 1f, 3f) + 1.70158f * Mathf.Pow(peratus - 1f, 2f);

            // Darabkan lantunan dengan saiz asal (supaya kekal kecil dalam AR)
            canvasMaklumat.localScale = skalaAsalCanvas * bounce;
            yield return null;
        }

        canvasMaklumat.localScale = skalaAsalCanvas; // Tetapkan saiz ke asal bila tamat
    }
}