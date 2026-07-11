using UnityEngine;

public class KawalSaizAR : MonoBehaviour
{
    [Header("Tetapan Saiz (Pinch to Zoom)")]
    public float kelajuanSkala = 0.005f;
    public float saizMinimum = 0.1f;
    public float saizMaksimum = 5.0f;

    [Header("Tetapan Pusing (Twist to Rotate)")]
    public float kelajuanPusing = 0.5f; // Kelajuan kucing berpusing

    void Update()
    {
        // Pastikan ada DUA jari menyentuh skrin
        if (Input.touchCount == 2)
        {
            Touch jari1 = Input.GetTouch(0);
            Touch jari2 = Input.GetTouch(1);

            // Dapatkan posisi jari pada frame sebelum ini
            Vector2 posisiJari1Lepas = jari1.position - jari1.deltaPosition;
            Vector2 posisiJari2Lepas = jari2.position - jari2.deltaPosition;

            // --- 1. PENGIRAAN UNTUK SAIZ (SCALE) ---
            float jarakLepas = (posisiJari1Lepas - posisiJari2Lepas).magnitude;
            float jarakSekarang = (jari1.position - jari2.position).magnitude;
            float bezaJarak = jarakSekarang - jarakLepas;

            BesarkanObjek(bezaJarak);

            // --- 2. PENGIRAAN UNTUK PUSING (ROTATE) ---
            Vector2 arahLepas = posisiJari1Lepas - posisiJari2Lepas;
            Vector2 arahSekarang = jari1.position - jari2.position;

            // Cari perbezaan sudut antara jari yang lepas dan sekarang
            float bezaSudut = Vector2.SignedAngle(arahLepas, arahSekarang);

            PusingkanObjek(bezaSudut);
        }
    }

    void BesarkanObjek(float beza)
    {
        Vector3 saizBaru = transform.localScale + Vector3.one * (beza * kelajuanSkala);

        saizBaru.x = Mathf.Clamp(saizBaru.x, saizMinimum, saizMaksimum);
        saizBaru.y = Mathf.Clamp(saizBaru.y, saizMinimum, saizMaksimum);
        saizBaru.z = Mathf.Clamp(saizBaru.z, saizMinimum, saizMaksimum);

        transform.localScale = saizBaru;
    }

    void PusingkanObjek(float sudut)
    {
        // Pusingkan objek pada paksi Y (kiri kanan) di dunia AR
        // Kalau kucing berpusing terbalik arah, letak tanda tolak (-) depan sudut
        transform.Rotate(Vector3.up, sudut * kelajuanPusing, Space.World);
    }
}