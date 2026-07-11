using UnityEngine;

public class PengurusTutorialAR : MonoBehaviour
{
    public CanvasGroup groupIkonSensor;
    private bool aktif = false;

    void Start()
    {
        if (groupIkonSensor != null) groupIkonSensor.gameObject.SetActive(false);
    }

    void Update()
    {
        if (aktif && groupIkonSensor != null)
        {
            groupIkonSensor.alpha = 0.4f + (Mathf.Sin(Time.time * 3f) + 1f) / 2f * 0.6f;
        }
    }

    // Fungsi ni dipanggil oleh ObjectSpawner & KesanTimbulKucing
    public void TunjukkanTutorialSensor()
    {
        aktif = true;
        if (groupIkonSensor != null) groupIkonSensor.gameObject.SetActive(true);
    }

    public void KucingDahSpawn() { TunjukkanTutorialSensor(); }

    // Fungsi ni dipanggil oleh SensorInteraksi
    public void TutorialSelesai()
    {
        aktif = false;
        if (groupIkonSensor != null) groupIkonSensor.gameObject.SetActive(false);
    }
}