using UnityEngine;
using System.Collections;

public class ConnectomeSplatterReveal : MonoBehaviour
{
    public Renderer meshRenderer; // Seret Model_Badan ke sini
    public float revealDuration = 4.0f; // Berapa lama scan dari kaki ke kepala

    private Material scanMaterial;

    void Start()
    {
        // Pastikan kita guna instance material supaya tak kacau objek lain
        scanMaterial = meshRenderer.material;

        // Mula dari 0 (kaki)
        scanMaterial.SetFloat("_ScanProgress", 0f);

        StartCoroutine(ProsesScanReveal());
    }

    IEnumerator ProsesScanReveal()
    {
        float timer = 0;
        while (timer < revealDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / revealDuration;

            // Gerakkan progres scan dalam shader
            scanMaterial.SetFloat("_ScanProgress", progress);

            yield return null;
        }

        // Pastikan dia penuh 100% di akhir
        scanMaterial.SetFloat("_ScanProgress", 1f);
    }
}