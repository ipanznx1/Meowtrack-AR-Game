using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PengurusKadInteraktif : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private Button butangSahkanKedudukan;
    [SerializeField] private ScrollRect scrollRectKad;

    private bool kedudukanTerkunci;
    private RectTransform rectTransform;
    private Vector3 offsetPosisi;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (butangSahkanKedudukan == null)
            butangSahkanKedudukan = GetComponentInChildren<Button>();

        if (scrollRectKad == null)
            scrollRectKad = GetComponentInChildren<ScrollRect>();
    }

    private void Start()
    {
        if (scrollRectKad != null)
        {
            scrollRectKad.enabled = false;
        }

        if (butangSahkanKedudukan != null)
        {
            butangSahkanKedudukan.onClick.AddListener(KunciKedudukan);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (kedudukanTerkunci)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPos))
        {
            offsetPosisi = rectTransform.position - worldPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (kedudukanTerkunci)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPos))
        {
            rectTransform.position = worldPos + offsetPosisi;
        }
    }

    private void KunciKedudukan()
    {
        kedudukanTerkunci = true;

        if (scrollRectKad != null)
        {
            scrollRectKad.enabled = true;
        }

        if (butangSahkanKedudukan != null)
        {
            butangSahkanKedudukan.gameObject.SetActive(false);
        }
    }
}
