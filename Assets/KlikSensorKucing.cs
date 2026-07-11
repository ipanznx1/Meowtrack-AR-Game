using UnityEngine;

public class KlikSensorKucing : MonoBehaviour
{
    [Header("Tarik dua-dua Kawasan_Leret ke sini")]
    public GameObject kawasanLeretBM;
    public GameObject kawasanLeretEN;

    // Status bahasa (Default: True = Melayu, False = English)
    public static bool gunaBahasaMelayu = true;

    private void OnMouseDown()
    {
        // Sistem check user nak bahasa apa masa klik sensor badan kucing
        if (gunaBahasaMelayu)
        {
            if (kawasanLeretBM != null) kawasanLeretBM.SetActive(true);
            if (kawasanLeretEN != null) kawasanLeretEN.SetActive(false);
        }
        else
        {
            if (kawasanLeretEN != null) kawasanLeretEN.SetActive(true);
            if (kawasanLeretBM != null) kawasanLeretBM.SetActive(false);
        }

        Debug.Log("Sensor badan kucing ditekan! Kad bahasa yang betul dibuka.");
    }

    // Fungsi ni untuk dipanggil oleh butang bahasa nanti
    public void SetBahasaMelayu(bool statusBM)
    {
        gunaBahasaMelayu = statusBM;
    }
}