using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class CoverFlowUI : MonoBehaviour
{
    [Header("Tetapan Fizik")]
    public float sensitivity = 0.5f;
    public float friction = 0.96f;
    public float lerpSpeed = 8f;

    [Header("Layout")]
    public float jejariX = 550f;
    public float scaleBelakang = 0.6f;

    [Header("Tutorial UI")]
    public GameObject tutorialUI;

    private List<RectTransform> senaraiKad = new List<RectTransform>();
    private float sudutSemasa = 0f;
    private float velocityX = 0f;
    private bool sedangTouch = false;

    void Start()
    {
        RefreshList();
        ResetDanMunculTutorial(); // Panggil masa mula game
    }

    void OnEnable()
    {
        ResetDanMunculTutorial(); // Panggil setiap kali menu muncul balik
    }

    // Fungsi khas untuk setelkan isu tutorial tak muncul/tak reset
    public void ResetDanMunculTutorial()
    {
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(true);

            // Paksa Animator patah balik ke frame 0 (saat pertama)
            Animator anim = tutorialUI.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }

            // Pastikan tutorial duduk depan kad kucing
            tutorialUI.transform.SetAsLastSibling();

            // Matikan RaycastTarget supaya jari boleh tembus ke lantai AR
            var imgs = tutorialUI.GetComponentsInChildren<Image>();
            foreach (var img in imgs)
            {
                img.raycastTarget = false;
            }
        }
    }

    public void UpdateListScripts() { RefreshList(); }

    public void RefreshList()
    {
        senaraiKad.Clear();
        foreach (Transform child in transform)
        {
            if (tutorialUI != null && child.gameObject == tutorialUI) continue;
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null) senaraiKad.Add(rt);
        }
    }

    void Update()
    {
        if (senaraiKad.Count == 0) return;
        HandleInput();

        if (!sedangTouch)
        {
            sudutSemasa += velocityX * Time.deltaTime * 60f;
            velocityX *= friction;
            if (Mathf.Abs(velocityX) < 0.2f)
            {
                float step = 360f / senaraiKad.Count;
                float targetSudut = Mathf.Round(sudutSemasa / step) * step;
                sudutSemasa = Mathf.Lerp(sudutSemasa, targetSudut, Time.deltaTime * lerpSpeed);
            }
        }
        UpdateLayout();
    }

    void HandleInput()
    {
        Vector2 delta = Vector2.zero;
        bool inputAktif = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            delta = Touchscreen.current.primaryTouch.delta.ReadValue();
            inputAktif = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            delta = Mouse.current.delta.ReadValue();
            inputAktif = true;
        }

        if (inputAktif)
        {
            sedangTouch = true;
            velocityX = delta.x * sensitivity;
            sudutSemasa += velocityX;

            // Kalau user dah mula swipe, kita sorok tutorial terus
            if (Mathf.Abs(velocityX) > 0.5f && tutorialUI != null)
            {
                tutorialUI.SetActive(false);
            }
        }
        else { sedangTouch = false; }
    }

    void UpdateLayout()
    {
        senaraiKad.RemoveAll(k => k == null);
        if (senaraiKad.Count == 0) return;

        float step = 360f / senaraiKad.Count;

        for (int i = 0; i < senaraiKad.Count; i++)
        {
            if (senaraiKad[i] == null) continue;

            float angle = sudutSemasa + (i * step);
            float rad = angle * Mathf.Deg2Rad;

            float posX = Mathf.Sin(rad) * jejariX;
            float cosVal = Mathf.Cos(rad);

            senaraiKad[i].localPosition = new Vector3(posX, 0, 0);

            float normalZ = (cosVal + 1f) / 2f;
            float scale = Mathf.Lerp(scaleBelakang, 1f, normalZ);
            senaraiKad[i].localScale = new Vector3(scale, scale, 1f);
        }

        var sorted = senaraiKad
            .Where(k => k != null)
            .OrderBy(k => k.localScale.x)
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i] != null) sorted[i].SetSiblingIndex(i);
        }

        // Sentiasa paksa tutorial duduk paling atas (paling depan visual)
        if (tutorialUI != null && tutorialUI.activeSelf)
        {
            tutorialUI.transform.SetAsLastSibling();
        }
    }
}