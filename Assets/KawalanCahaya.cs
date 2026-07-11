using UnityEngine;
using UnityEngine.UI;

public class KawalanCahaya : MonoBehaviour
{
    public Image imejKad;

    public Color warnaSelamat = Color.green;
    public Color warnaAmaran = Color.yellow;
    public Color warnaBahaya = Color.red;

    [Range(1f, 10f)]
    public float intensitiCahaya = 5f;

    public enum TahapRisiko { Selamat, Amaran, Bahaya }
    public TahapRisiko risikoSekarang;

    private TahapRisiko risikoSebelumnya;

    void Start()
    {
        if (imejKad == null)
        {
            imejKad = GetComponent<Image>();
        }

        risikoSebelumnya = risikoSekarang;
        SetWarnaCahaya(risikoSekarang);
    }

    void Update()
    {
        if (risikoSekarang != risikoSebelumnya)
        {
            SetWarnaCahaya(risikoSekarang);
            risikoSebelumnya = risikoSekarang;
        }
    }

    void SetWarnaCahaya(TahapRisiko risiko)
    {
        Color warnaPilihan;

        switch (risiko)
        {
            case TahapRisiko.Selamat: warnaPilihan = warnaSelamat; break;
            case TahapRisiko.Amaran: warnaPilihan = warnaAmaran; break;
            case TahapRisiko.Bahaya: warnaPilihan = warnaBahaya; break;
            default: warnaPilihan = Color.white; break;
        }

        imejKad.color = warnaPilihan * intensitiCahaya;
    }

    // ========================================================
    // INI FUNGSI BARU UNTUK SCRIPT LAIN HANTAR NOMBOR 1, 2, 3
    // ========================================================
    public void TetapkanTahapRisiko(int nombor)
    {
        if (nombor == 1)
        {
            risikoSekarang = TahapRisiko.Selamat;
        }
        else if (nombor == 2)
        {
            risikoSekarang = TahapRisiko.Amaran;
        }
        else if (nombor >= 3) // Tahap 3 dan ke atas dianggap Bahaya
        {
            risikoSekarang = TahapRisiko.Bahaya;
        }
    }
}