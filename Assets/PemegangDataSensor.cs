using UnityEngine;

public class PemegangDataSensor : MonoBehaviour
{
    [Header("Masukkan SEMUA Fail Penyakit Di Sini")]
    public PenyakitData[] senaraiPenyakit;

    private int indeksSemasa = 0; // Untuk ingat kita kat fail nombor berapa

    // 1. Fungsi ambil data sekarang (Tanpa gerakkan senarai)
    public PenyakitData DapatkanDataSemasa()
    {
        if (senaraiPenyakit.Length == 0) return null;
        return senaraiPenyakit[indeksSemasa];
    }

    // 2. Fungsi ke depan (Bila butang Next ditekan)
    public PenyakitData KeSeterusnya()
    {
        if (senaraiPenyakit.Length == 0) return null;

        indeksSemasa = indeksSemasa + 1;

        // Kalau dah sampai fail terakhir, patah balik ke mula
        if (indeksSemasa >= senaraiPenyakit.Length)
        {
            indeksSemasa = 0;
        }

        return senaraiPenyakit[indeksSemasa];
    }

    // 3. Fungsi patah balik (Bila butang Prev ditekan)
    public PenyakitData KeSebelumnya()
    {
        if (senaraiPenyakit.Length == 0) return null;

        indeksSemasa = indeksSemasa - 1;

        // Kalau patah balik dari awal, pergi ke fail paling hujung
        if (indeksSemasa < 0)
        {
            indeksSemasa = senaraiPenyakit.Length - 1;
        }

        return senaraiPenyakit[indeksSemasa];
    }
}