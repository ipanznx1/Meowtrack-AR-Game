using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ARManager : MonoBehaviour
{
    [Header("AR Setup")]
    public GameObject kucingPrefab;
    private GameObject spawnedKucing;
    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [Header("Database Penyakit")]
    public List<PenyakitData> senaraiPenyakit;

    [Header("UI Panels (Paparan Info Bawaan AR)")]
    public GameObject panelUtama;
    public TextMeshProUGUI teksNamaPenyakit;
    public TextMeshProUGUI teksTahapRisiko;
    public TextMeshProUGUI teksPunca;
    public TextMeshProUGUI teksSimptom;
    public TextMeshProUGUI teksTindakan;
    public TextMeshProUGUI teksMaklumatTambahan;
    public Image paparanGambar;

    [Header("Butang UI Penukar Bahasa (Tepi Canvas)")]
    public Button butangBM;
    public Button butangEN;

    // Status bahasa & data penyakit yang tengah aktif
    public static bool gunaBahasaMelayu = true;
    private PenyakitData dataTengahAktif;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (panelUtama != null) panelUtama.SetActive(false);

        if (kucingPrefab == null)
        {
            Debug.LogWarning("[ARManager] kucingPrefab is not assigned in the Inspector. The app will try to find a prefab named like 'kucing' in the project at runtime.");
        }

        // KOD AUTOMATIK: Mendengar klik dari Butang Tepi Canvas secara langsung dari script
        if (butangBM != null) butangBM.onClick.AddListener(() => TukarBahasaApp(true));
        if (butangEN != null) butangEN.onClick.AddListener(() => TukarBahasaApp(false));
    }

    void Update()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Vector2 inputPos = touch.position;
#else
        // New Input System path: support mouse clicks in editor and touchscreen on devices
        Vector2 inputPos = Vector2.zero;
        bool gotInput = false;
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            gotInput = true;
        }
        else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            inputPos = UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
            gotInput = true;
        }
        if (!gotInput) return;
#endif

        if (spawnedKucing == null)
        {
            if (raycastManager.Raycast(inputPos, hits, TrackableType.PlaneWithinPolygon))
            {
                if (kucingPrefab == null)
                {
                    // Try to auto-find a cat prefab asset by name and component signature
                    GameObject found = null;
                    var all = Resources.FindObjectsOfTypeAll<GameObject>();
                    for (int i = 0; i < all.Length; i++)
                    {
                        var go = all[i];
                        if (go == null) continue;
                        string nameLow = go.name.ToLower();
                        if (!nameLow.Contains("kucing")) continue;
                        if (go.scene.IsValid()) continue; // Skip scene objects

                        bool hasCatComponent = go.GetComponentInChildren<PemegangDataSensor>() != null || go.GetComponentInChildren<KlikSensorKucing>() != null;
                        if (hasCatComponent)
                        {
                            found = go;
                            break;
                        }
                    }

                    if (found != null)
                    {
                        kucingPrefab = found;
                        Debug.LogWarning("[ARManager] kucingPrefab was null; auto-linked to cat prefab candidate: " + found.name + ". Assign in Inspector to avoid this message.");
                    }
                    else
                    {
                        Debug.LogError("[ARManager] Cannot spawn kucing because kucingPrefab is not assigned. Please assign a prefab in the ARManager Inspector.");
                        return;
                    }
                }

                spawnedKucing = Instantiate(kucingPrefab, hits[0].pose.position, hits[0].pose.rotation);

                // Lock the spawned object's pose to prevent it from moving unexpectedly
                var lockComp = spawnedKucing.AddComponent<KunciKedudukan>();
                lockComp.LockToPose(hits[0].pose.position, hits[0].pose.rotation);

                // Notify greeting watcher / popup flow that the cat was placed
                GreetingWatcher gw = FindAnyObjectByType<GreetingWatcher>();
                if (gw != null)
                {
                    gw.NotifyKucingPlaced();
                }
                else
                {
                    // Fallback: directly inform popup flow manager
                    PengurusAliranPopupAR pap = FindAnyObjectByType<PengurusAliranPopupAR>();
                    if (pap != null)
                    {
                        pap.KucingDahDiletak();
                    }
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string tagSensor = hit.collider.tag;
                PaparInfo(tagSensor);
            }
        }
    }

    void PaparInfo(string bahagian)
    {
        List<PenyakitData> pilihan = senaraiPenyakit.FindAll(x => x.tagBahagian == bahagian);

        if (pilihan.Count > 0)
        {
            if (panelUtama != null)
            {
                panelUtama.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[ARManager] panelUtama is not assigned. The AR info panel will not be shown.");
            }

            var arRaycast = GetComponent<ARRaycastManager>();
            if (arRaycast != null)
            {
                arRaycast.enabled = false;
            }
            else
            {
                Debug.LogWarning("[ARManager] ARRaycastManager component missing on this GameObject.");
            }

            int index = Random.Range(0, pilihan.Count);
            dataTengahAktif = pilihan[index];

            // 1. Update teks internal bawaan ARManager
            KemasKiniTeksUI();

            // 2. KOD UTAMA: Cari jika ada KadMaklumatUI (Prefab) yang terbuka, lalu paksa dia isi data & update bahasa!
            KadMaklumatUI kadAktif = Object.FindFirstObjectByType<KadMaklumatUI>();
            if (kadAktif != null)
            {
                kadAktif.IsikanData(dataTengahAktif);
            }

            if (dataTengahAktif.gambar != null)
            {
                if (paparanGambar != null)
                {
                    paparanGambar.sprite = dataTengahAktif.gambar;
                    paparanGambar.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("[ARManager] paparanGambar is not assigned.");
                }
            }
            else
            {
                if (paparanGambar != null)
                {
                    paparanGambar.gameObject.SetActive(false);
                }
            }
        }
    }

    // Fungsi untuk update teks di panel utama ARManager (Jika kamu masih menggunakannya)
    public void KemasKiniTeksUI()
    {
        if (dataTengahAktif == null) return;

        if (gunaBahasaMelayu)
        {
            if (teksNamaPenyakit != null) teksNamaPenyakit.text = dataTengahAktif.namaPenyakit;
            if (teksTahapRisiko != null) teksTahapRisiko.text = "Tahap Risiko: " + dataTengahAktif.tahapRisiko;
            if (teksPunca != null) teksPunca.text = "Punca: \n" + dataTengahAktif.punca;
            if (teksSimptom != null) teksSimptom.text = "Simptom: \n" + dataTengahAktif.simptom;
            if (teksTindakan != null) teksTindakan.text = "Tindakan: \n" + dataTengahAktif.tindakan;
            if (teksMaklumatTambahan != null) teksMaklumatTambahan.text = "Maklumat Tambahan: \n" + dataTengahAktif.maklumatTambahan;
        }
        else
        {
            if (teksNamaPenyakit != null) teksNamaPenyakit.text = dataTengahAktif.namaPenyakitEN;
            if (teksTahapRisiko != null) teksTahapRisiko.text = "Risk Level: " + dataTengahAktif.tahapRisikoEN;
            if (teksPunca != null) teksPunca.text = "Cause: \n" + dataTengahAktif.puncaEN;
            if (teksSimptom != null) teksSimptom.text = "Symptoms: \n" + dataTengahAktif.simptomEN;
            if (teksTindakan != null) teksTindakan.text = "Action: \n" + dataTengahAktif.tindakanEN;
            if (teksMaklumatTambahan != null) teksMaklumatTambahan.text = "Additional Info: \n" + dataTengahAktif.maklumatTambahanEN;
        }
    }

    // FUNGSI UTAMA: Dipanggil ketika menekan butang bahasa di TEPI CANVAS LUAR
    public void TukarBahasaApp(bool statusBM)
    {
        gunaBahasaMelayu = statusBM;
        Debug.LogWarning("Bahasa global ditukar dari BUTANG TEPI! Status BM = " + gunaBahasaMelayu);

        // Update teks panel internal ARManager
        KemasKiniTeksUI();

        // Cari prefab KadMaklumatUI di skrin, lalu paksa dia update teks deskripsi DAN teks tajuk butang tab!
        KadMaklumatUI kadAktif = Object.FindFirstObjectByType<KadMaklumatUI>();
        if (kadAktif != null)
        {
            kadAktif.KemasKiniBahasaUI();
            Debug.Log("Kad Prefab dijumpai! Teks deskripsi & judul butang tab berhasil ditukar.");
        }
        else
        {
            Debug.LogWarning("Kad Prefab 'KadMaklumatUI' tidak sedang aktif di skrin.");
        }
    }

    public void TutupPopup()
    {
        if (panelUtama != null) panelUtama.SetActive(false);
        GetComponent<ARRaycastManager>().enabled = true;
        dataTengahAktif = null;
    }
}