using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    public void QuitApp()
    {
        // Ini akan tutup activity Unity dan balik ke Flutter
        Application.Quit();
    }
}