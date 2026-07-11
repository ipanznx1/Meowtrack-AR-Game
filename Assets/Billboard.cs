using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Rujukan kepada Kamera Utama AR
    private Camera my_cam;

    void Start()
    {
        // Cari kamera kat phone
        my_cam = Camera.main;
    }

    void LateUpdate()
    {
        if (my_cam == null) return;

        // Paksa UI sentiasa menghadap kamera
        transform.LookAt(transform.position + my_cam.transform.rotation * Vector3.forward,
                         my_cam.transform.rotation * Vector3.up);
    }
}