using UnityEngine;

[CreateAssetMenu(fileName = "InfoPenyakit", menuName = "KucingAR/DataPenyakit")]
public class PenyakitData : ScriptableObject
{
    [Header("Maklumat Asas")]
    public string tagBahagian;      // Mengikut tag sensor anda (Mata, Telinga, dll)
    public string namaPenyakit;     // Nama penyakit asal (BM)
    public string namaPenyakitEN;   // Nama penyakit versi English

    [Header("Tahap Risiko (Asal - BM)")]
    public string tahapRisiko;      // Data BM anda - Dijamin selamat, tak hilang!

    [Header("Tahap Risiko (English Baru)")]
    public string tahapRisikoEN;    // Data EN anda - Dijamin selamat, tak hilang!

    [Header("Visual")]
    public Sprite gambar;
    public bool isGraphic;

    [Header("Butiran Penyakit (Asal - BM)")]
    [TextArea(2, 5)] public string punca;            // Data BM anda - Dijamin selamat!
    [TextArea(3, 8)] public string simptom;          // Data BM anda - Dijamin selamat!
    [TextArea(2, 5)] public string tindakan;         // Data BM anda - Dijamin selamat!
    [TextArea(3, 8)] public string maklumatTambahan; // Data BM anda - Dijamin selamat!

    [Header("Butiran Penyakit (English Baru)")]
    [TextArea(2, 5)] public string puncaEN;          // Data EN anda - Dijamin selamat!
    [TextArea(3, 8)] public string simptomEN;        // Data EN anda - Dijamin selamat!
    [TextArea(2, 5)] public string tindakanEN;       // Data EN anda - Dijamin selamat!
    [TextArea(3, 8)] public string maklumatTambahanEN; // Data EN anda - Dijamin selamat!
}