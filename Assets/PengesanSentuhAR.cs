using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PengesanSentuhAR : MonoBehaviour
{
    [Header("Kamera AR")]
    public Camera arKamera;

    [Header("Tetapan Pergerakan Canvas")]
    public Transform canvasMaklumat;
    public Vector3 jarakOffset = new Vector3(0.15f, 0.15f, 0f);

    [Header("Tetapan Cetakan Kad (Carousel)")]
    public GameObject kadPrefab;         // Masukkan Kad_Penyakit_Prefab di sini
    public Transform tempatSusunKad;     // Masukkan objek "Content" dari ScrollView di sini

    private Coroutine animasiSekarang;
    private PemegangDataSensor sensorAktif;
    private Vector3 skalaAsalCanvas;     // Untuk simpan saiz asal Canvas (contoh: 0.001)

    void Start()
    {
        // Simpan saiz asal Canvas supaya animasi tak jadikan ia gergasi
        if (canvasMaklumat != null)
        {
            skalaAsalCanvas = canvasMaklumat.localScale;
        }
    }

    void Update()
    {
        bool adaTekanan = false;
        Vector2 posisiTekanan = Vector2.zero;

        // Kesan KLIK MOUSE
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            adaTekanan = true;
            posisiTekanan = Mouse.current.position.ReadValue();
        }
        // Kesan SENTUHAN JARI
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            adaTekanan = true;
            posisiTekanan = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (adaTekanan)
        {
            Ray ray = arKamera.ScreenPointToRay(posisiTekanan);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                PemegangDataSensor sensor = hit.transform.GetComponent<PemegangDataSensor>();

                if (sensor != null)
                {
                    // 1. Simpan sensor ini sebagai sensor yang 'Aktif' sekarang
                    sensorAktif = sensor;

                    // 2. Ubah kedudukan Canvas ke lokasi sensor yang disentuh
                    if (canvasMaklumat != null)
                    {
                        canvasMaklumat.position = hit.transform.position + jarakOffset;
                    }

                    // 3. Tunjuk SEMUA data dalam bentuk kad bertindan
                    PaparkanData(sensorAktif);
                }
            }
        }
    }

    // --- FUNGSI UNTUK CETAK KAD CAROUSEL ---
    private void PaparkanData(PemegangDataSensor sensor)
    {
        if (sensor == null || sensor.senaraiPenyakit == null || sensor.senaraiPenyakit.Length == 0) return;

        // 1. Padam semua kad lama yang ada dalam ScrollView supaya tak bertindih
        foreach (Transform child in tempatSusunKad)
        {
            Destroy(child.gameObject);
        }

        // 2. Cetak kad baru satu per satu berdasarkan senarai penyakit yang ada
        foreach (PenyakitData data in sensor.senaraiPenyakit)
        {
            // Cipta kad baru
            GameObject kadBaru = Instantiate(kadPrefab, tempatSusunKad);

            // Masukkan data penyakit ke dalam kad tersebut
            KadMaklumatUI skripKad = kadBaru.GetComponent<KadMaklumatUI>();
            if (skripKad != null)
            {
                skripKad.IsikanData(data);
            }
        }

        // --- BAHAGIAN BARU (LANGKAH 4) ---
        // Memberitahu skrip CoverFlowUI untuk mengesan kad-kad baru yang baru dicetak
        CoverFlowUI cf = tempatSusunKad.GetComponent<CoverFlowUI>();
        if (cf != null)
        {
            cf.RefreshList();
        }
        // ---------------------------------

        // 3. Mainkan animasi melantun supaya nampak cantik bila data bertukar
        if (canvasMaklumat != null)
        {
            if (animasiSekarang != null) StopCoroutine(animasiSekarang);
            animasiSekarang = StartCoroutine(EfekMelantunUI());
        }
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