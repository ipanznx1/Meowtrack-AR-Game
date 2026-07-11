using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KesanTimbulKucing : MonoBehaviour
{
    public List<GameObject> senaraiModelKucing;
    public float masaTungguShader = 2.0f; // Berapa lama masa scan shader kau amik

    void Start()
    {
        // 1. Sembunyikan model masa awal scan
        foreach (GameObject model in senaraiModelKucing)
            if (model != null) model.SetActive(false);

        StartCoroutine(TungguShaderSelesai());
    }

    IEnumerator TungguShaderSelesai()
    {
        // Tunggu shader ScanReveal selesai bergerak ke atas
        yield return new WaitForSeconds(masaTungguShader);

        // 2. Aktifkan model kucing sepenuhnya
        foreach (GameObject model in senaraiModelKucing)
            if (model != null) model.SetActive(true);

        // 3. Panggil tutorial sensor
        PengurusTutorialAR tutorial = FindAnyObjectByType<PengurusTutorialAR>();
        if (tutorial != null)
        {
            tutorial.KucingDahSpawn();
        }
    }
}