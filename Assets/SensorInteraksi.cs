using UnityEngine;
using UnityEngine.EventSystems;

public class SensorInteraksi : MonoBehaviour
{
    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        GameObject tutorialObj = GameObject.Find("Canvas_Maklumat");
        if (tutorialObj != null)
            tutorialObj.SendMessage("TutorialSelesai", SendMessageOptions.DontRequireReceiver);
    }
}