using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class KadMaklumatUI : MonoBehaviour
{
    [Header("Sambungan Isi Maklumat Kad")]
    public TextMeshProUGUI Teks_Nama;
    public TextMeshProUGUI Teks_Risiko;
    public TextMeshProUGUI Teks_Punca;
    public TextMeshProUGUI Teks_Simptom;
    public TextMeshProUGUI Teks_Tindakan;
    public TextMeshProUGUI Teks_MaklumatTambahan;

    [Header("SET A: Tajuk Statik Bahagian TENGAH")]
    public TextMeshProUGUI Title_Tengah_Risiko;
    public TextMeshProUGUI Title_Tengah_Punca;
    public TextMeshProUGUI Title_Tengah_Simptom;
    public TextMeshProUGUI Title_Tengah_Tindakan;
    public TextMeshProUGUI Title_Tengah_MaklumatTambahan;

    [Header("SET B: Tajuk Statik Bahagian TEPI")]
    public TextMeshProUGUI Title_Tepi_Risiko;
    public TextMeshProUGUI Title_Tepi_Punca;
    public TextMeshProUGUI Title_Tepi_Simptom;
    public TextMeshProUGUI Title_Tepi_Tindakan;
    public TextMeshProUGUI Title_Tepi_MaklumatTambahan;

    [Header("Sambungan Gambar & Ikon Di Kad")]
    public Image gambarPenyakit;
    public Image ikonAmaran;

    [Header("Sistem Censored (Baru)")]
    public GameObject panelBlurPenyakit;
    private bool imejBolehDilihat = true;

    [Header("Gambar Ikon Amaran")]
    public Sprite ikonRisiko1;
    public Sprite ikonRisiko2;
    public Sprite ikonRisiko3;

    [Header("Sistem Bunyi")]
    public AudioClip bunyiTekan;
    private AudioSource sumberAudio;

    [Header("Efek Visual")]
    public KawalanCahaya scriptCahaya;

    [Header("Sistem Tab (4 Tab)")]
    public GameObject panelPunca;
    public GameObject panelSimptom;
    public GameObject panelTindakan;
    public GameObject panelMaklumatTambahan;

    [HideInInspector] public GameObject panelGambarBesar_DiCanvas;
    [HideInInspector] public Image imejBesarUI;

    private Coroutine animasiPopUp;
    private PenyakitData dataSemasa;

    void Awake()
    {
        sumberAudio = gameObject.AddComponent<AudioSource>();
        sumberAudio.playOnAwake = false;

        GameObject canvasUtama = GameObject.Find("Canvas_Skrin");
        if (canvasUtama != null)
        {
            Transform panelT = canvasUtama.transform.Find("Panel_Gambar_Besar");
            if (panelT != null)
            {
                panelGambarBesar_DiCanvas = panelT.gameObject;
                imejBesarUI = panelT.Find("Imej_Besar").GetComponent<Image>();

                Transform butangTutupT = panelT.Find("Button");
                if (butangTutupT != null)
                {
                    Button butangTutup = butangTutupT.GetComponent<Button>();
                    if (butangTutup != null)
                    {
                        butangTutup.onClick.RemoveAllListeners();
                        butangTutup.onClick.AddListener(TutupGambarBesar);
                    }
                }
            }
        }

        if (gambarPenyakit != null)
        {
            Button butangGambarKecil = gambarPenyakit.GetComponent<Button>();
            if (butangGambarKecil == null) butangGambarKecil = gambarPenyakit.gameObject.AddComponent<Button>();
            butangGambarKecil.onClick.AddListener(BukaGambarBesar);
        }
    }

    public void IsikanData(PenyakitData dataTerkini)
    {
        dataSemasa = dataTerkini;
        KemasKiniBahasaUI();

        if (dataTerkini.gambar != null && gambarPenyakit != null) { gambarPenyakit.sprite = dataTerkini.gambar; }

        if (panelBlurPenyakit != null)
        {
            if (dataTerkini.isGraphic)
            {
                panelBlurPenyakit.SetActive(true);
                imejBolehDilihat = false;
            }
            else
            {
                panelBlurPenyakit.SetActive(false);
                imejBolehDilihat = true;
            }
        }

        if (!string.IsNullOrEmpty(dataTerkini.tahapRisiko))
        {
            string risiko = dataTerkini.tahapRisiko.ToLower();
            if (risiko.Contains("1"))
            {
                if (ikonAmaran != null) ikonAmaran.sprite = ikonRisiko1;
                if (scriptCahaya != null) scriptCahaya.TetapkanTahapRisiko(1);
            }
            else if (risiko.Contains("2"))
            {
                if (ikonAmaran != null) ikonAmaran.sprite = ikonRisiko2;
                if (scriptCahaya != null) scriptCahaya.TetapkanTahapRisiko(2);
            }
            else if (risiko.Contains("3"))
            {
                if (ikonAmaran != null) ikonAmaran.sprite = ikonRisiko3;
                if (scriptCahaya != null) scriptCahaya.TetapkanTahapRisiko(3);
            }
        }
        TukarTab(1);
    }

    public void TukarBahasaDariButang(bool statusBM)
    {
        MainkanBunyi();
        ARManager.gunaBahasaMelayu = statusBM;
        Debug.LogWarning("Butang Prefab Ditekan! Status BM = " + ARManager.gunaBahasaMelayu);
        KemasKiniBahasaUI();
    }

    public void KemasKiniBahasaUI()
    {
        if (dataSemasa == null) return;

        if (ARManager.gunaBahasaMelayu)
        {
            // Isi Kandungan (BM)
            if (Teks_Nama != null) Teks_Nama.text = dataSemasa.namaPenyakit;
            if (Teks_Risiko != null) Teks_Risiko.text = "Tahap Risiko: " + dataSemasa.tahapRisiko;
            if (Teks_Punca != null) Teks_Punca.text = dataSemasa.punca;
            if (Teks_Simptom != null) Teks_Simptom.text = dataSemasa.simptom;
            if (Teks_Tindakan != null) Teks_Tindakan.text = dataSemasa.tindakan;
            if (Teks_MaklumatTambahan != null) Teks_MaklumatTambahan.text = dataSemasa.maklumatTambahan;

            // Tajuk Tengah (BM)
            if (Title_Tengah_Risiko != null) Title_Tengah_Risiko.text = "Tahap Risiko";
            if (Title_Tengah_Punca != null) Title_Tengah_Punca.text = "Punca";
            if (Title_Tengah_Simptom != null) Title_Tengah_Simptom.text = "Simptom";
            if (Title_Tengah_Tindakan != null) Title_Tengah_Tindakan.text = "Tindakan";
            if (Title_Tengah_MaklumatTambahan != null) Title_Tengah_MaklumatTambahan.text = "Maklumat Tambahan";

            // Tajuk Tepi (BM)
            if (Title_Tepi_Risiko != null) Title_Tepi_Risiko.text = "Tahap Risiko";
            if (Title_Tepi_Punca != null) Title_Tepi_Punca.text = "Punca";
            if (Title_Tepi_Simptom != null) Title_Tepi_Simptom.text = "Simptom";
            if (Title_Tepi_Tindakan != null) Title_Tepi_Tindakan.text = "Tindakan";
            if (Title_Tepi_MaklumatTambahan != null) Title_Tepi_MaklumatTambahan.text = "Maklumat Tambahan";
        }
        else
        {
            // Isi Kandungan (EN)
            if (Teks_Nama != null) Teks_Nama.text = dataSemasa.namaPenyakitEN;
            if (Teks_Risiko != null) Teks_Risiko.text = "Risk Level: " + dataSemasa.tahapRisikoEN;
            if (Teks_Punca != null) Teks_Punca.text = dataSemasa.puncaEN;
            if (Teks_Simptom != null) Teks_Simptom.text = dataSemasa.simptomEN;
            if (Teks_Tindakan != null) Teks_Tindakan.text = dataSemasa.tindakanEN;
            if (Teks_MaklumatTambahan != null) Teks_MaklumatTambahan.text = dataSemasa.maklumatTambahanEN;

            // Tajuk Tengah (EN)
            if (Title_Tengah_Risiko != null) Title_Tengah_Risiko.text = "Risk Level";
            if (Title_Tengah_Punca != null) Title_Tengah_Punca.text = "Causes";
            if (Title_Tengah_Simptom != null) Title_Tengah_Simptom.text = "Symptoms";
            if (Title_Tengah_Tindakan != null) Title_Tengah_Tindakan.text = "Actions";
            if (Title_Tengah_MaklumatTambahan != null) Title_Tengah_MaklumatTambahan.text = "Additional Info";

            // Tajuk Tepi (EN)
            if (Title_Tepi_Risiko != null) Title_Tepi_Risiko.text = "Risk Level";
            if (Title_Tepi_Punca != null) Title_Tepi_Punca.text = "Causes";
            if (Title_Tepi_Simptom != null) Title_Tepi_Simptom.text = "Symptoms";
            if (Title_Tepi_Tindakan != null) Title_Tepi_Tindakan.text = "Actions";
            if (Title_Tepi_MaklumatTambahan != null) Title_Tepi_MaklumatTambahan.text = "Additional Info";
        }
    }

    public void KlikBukaCensor()
    {
        MainkanBunyi();
        if (panelBlurPenyakit != null)
        {
            panelBlurPenyakit.SetActive(false);
            imejBolehDilihat = true;
        }
    }

    private void MainkanBunyi()
    {
        if (bunyiTekan != null && sumberAudio != null) sumberAudio.PlayOneShot(bunyiTekan);
    }

    public void TukarTab(int nomborTab)
    {
        MainkanBunyi();
        if (panelPunca != null) panelPunca.SetActive(false);
        if (panelSimptom != null) panelSimptom.SetActive(false);
        if (panelTindakan != null) panelTindakan.SetActive(false);
        if (panelMaklumatTambahan != null) panelMaklumatTambahan.SetActive(false);

        if (nomborTab == 1 && panelPunca != null) panelPunca.SetActive(true);
        else if (nomborTab == 2 && panelSimptom != null) panelSimptom.SetActive(true);
        else if (nomborTab == 3 && panelTindakan != null) panelTindakan.SetActive(true);
        else if (nomborTab == 4 && panelMaklumatTambahan != null) panelMaklumatTambahan.SetActive(true);
    }

    public void BukaGambarBesar()
    {
        MainkanBunyi();
        if (!imejBolehDilihat) return;

        if (gambarPenyakit != null && gambarPenyakit.sprite != null && panelGambarBesar_DiCanvas != null && imejBesarUI != null)
        {
            imejBesarUI.sprite = gambarPenyakit.sprite;
            if (animasiPopUp != null) StopCoroutine(animasiPopUp);
            animasiPopUp = StartCoroutine(AnimasiBukaPopUp());
        }
    }

    public void TutupGambarBesar()
    {
        MainkanBunyi();
        if (panelGambarBesar_DiCanvas != null && panelGambarBesar_DiCanvas.activeSelf)
        {
            if (animasiPopUp != null) StopCoroutine(animasiPopUp);
            animasiPopUp = StartCoroutine(AnimasiTutupPopUp());
        }
    }

    IEnumerator AnimasiBukaPopUp()
    {
        panelGambarBesar_DiCanvas.SetActive(true);
        panelGambarBesar_DiCanvas.transform.localScale = Vector3.zero;
        float masa = 0f; float durasi = 0.25f;
        while (masa < durasi)
        {
            masa += Time.deltaTime;
            float peratus = masa / durasi;
            float bounce = 1f + 2.70158f * Mathf.Pow(peratus - 1f, 3f) + 1.70158f * Mathf.Pow(peratus - 1f, 2f);
            panelGambarBesar_DiCanvas.transform.localScale = Vector3.one * bounce;
            yield return null;
        }
        panelGambarBesar_DiCanvas.transform.localScale = Vector3.one;
    }

    IEnumerator AnimasiTutupPopUp()
    {
        float masa = 0f; float durasi = 0.2f;
        Vector3 skalaMula = panelGambarBesar_DiCanvas.transform.localScale;
        while (masa < durasi)
        {
            masa += Time.deltaTime;
            float peratus = masa / durasi;
            panelGambarBesar_DiCanvas.transform.localScale = Vector3.Scale(skalaMula, Vector3.zero);
            yield return null;
        }
        panelGambarBesar_DiCanvas.transform.localScale = Vector3.zero;
        panelGambarBesar_DiCanvas.SetActive(false);
    }
}