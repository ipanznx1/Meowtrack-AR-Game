using UnityEngine;
using UnityEngine.EventSystems; // WAJIB ADA UNTUK DIRECT TOUCH

public class BahagianBadan : MonoBehaviour, IPointerDownHandler
{
    [Header("Info Bahagian Ini")]
    public string namaBahagian = "Telinga Kucing";

    [Header("Rujukan UI Popup (Scroll Kiri-Kanan)")]
    public GameObject panelInfoUI;

    [Header("Gambar Tutorial Sentuh")]
    public GameObject gambarPenanda; // Tarik objek 'Ikon_Sentuh' ke sini

    // Fungsi ni gerenti jalan bila skrin telefon dikesan sentuh tepat pada collider
    public void OnPointerDown(PointerEventData eventData)
    {
        if (panelInfoUI != null)
        {
            // 1. Buka panel info
            panelInfoUI.SetActive(true);

            // 2. Padamkan bulatan putih yang besar tu terus!
            if (gambarPenanda != null)
            {
                gambarPenanda.SetActive(false);
            }

            Debug.Log("Sistem Touch Berjaya! " + namaBahagian + " dipadam.");
            // Inform popup flow manager that the body was pressed
            var pap = FindAnyObjectByType<PengurusAliranPopupAR>();
            if (pap != null)
            {
                pap.BadanKucingDitekan();
            }
        }
    }
}