using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class DeviceCheck : MonoBehaviour
{
    IEnumerator Start()
    {
        // Tunggu ARCore initialize
        yield return ARSession.CheckAvailability();

        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.LogError("DEVICE TIDAK SUPPORT AR!");
        }
        else
        {
            Debug.Log("DEVICE SUPPORT AR! Masalah mungkin pada setting kamera.");
        }
    }
}