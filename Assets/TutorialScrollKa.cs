using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Guna sistem touch skrin

public class TutorialScrollKad : MonoBehaviour, IBeginDragHandler
{
    [Header("Rujukan Gambar Jari")]
    public GameObject tutorialSwipeUI; // Tarik objek 'Tutorial_SwipeKad' ke sini

    // Fungsi ni akan dipanggil secara OTOMATIK setiap kali panel info/maklumat ni dibuka
    void OnEnable()
    {
        if (tutorialSwipeUI != null)
        {
            tutorialSwipeUI.SetActive(true); // Hidupkan balik imej jari

            // Paksa animasi jari gerak kiri-kanan start dari frame kosong balik
            Animator anim = tutorialSwipeUI.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }
        }
    }

    // Fungsi ni gerenti jalan automatik saat PERTAMA Kali user heret/swipe kad tu
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (tutorialSwipeUI != null && tutorialSwipeUI.activeSelf)
        {
            // User dah mula heret kad, jadi kita padam tutorial jari terus!
            tutorialSwipeUI.SetActive(false);
            Debug.Log("User dah mula leret kad, tutorial dipadam!");
        }
    }
}