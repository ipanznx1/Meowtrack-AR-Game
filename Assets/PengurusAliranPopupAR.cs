using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PengurusAliranPopupAR : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private GameObject popupSelamatDatang;
    [SerializeField] private GameObject popupSilaLetakKucing;
    [SerializeField] private GameObject popupSilaTekanBadan;
    [SerializeField] private GameObject popupSilaLetakKad;
    [SerializeField] private GameObject coverFlowUI;

    [Header("Opsyen")]
    [SerializeField] private bool mulaAutoPadaStart = true;
    [Header("Delay Options")]
    [Tooltip("Delay in seconds to keep the 'letak kucing' popup visible after placement before showing the 'tekan badan' popup.")]
    [SerializeField] private float delaySebelumTekanBadan = 1.0f;

    private Coroutine pendingDelayCoroutine;

    private enum TahapFlow
    {
        MenungguMula,
        TungguLetakKucing,
        TungguTekanBadan,
        TungguLetakKad,
        Selesai
    }

    private TahapFlow tahapSemasa;

    private void Awake()
    {
        PasangButangLanjut(popupSelamatDatang, () => TunjukArahanLetakKucing());
        PasangButangLanjut(popupSilaLetakKucing, () => TunjukArahanTekanBadan());
        PasangButangLanjut(popupSilaTekanBadan, () => TunjukArahanLetakKad());
        PasangButangLanjut(popupSilaLetakKad, () => MunculkanCoverFlow());

        // Diagnostic: log which popup references are assigned
        Debug.Log($"[PengurusAliranPopupAR] Awake assignments: popupSelamatDatang={(popupSelamatDatang!=null?popupSelamatDatang.name:"NULL")}, popupSilaLetakKucing={(popupSilaLetakKucing!=null?popupSilaLetakKucing.name:"NULL")}, popupSilaTekanBadan={(popupSilaTekanBadan!=null?popupSilaTekanBadan.name:"NULL")}, popupSilaLetakKad={(popupSilaLetakKad!=null?popupSilaLetakKad.name:"NULL")}, coverFlowUI={(coverFlowUI!=null?coverFlowUI.name:"NULL")}");

        // If coverFlowUI isn't assigned in Inspector, try to auto-find one in scene
        if (coverFlowUI == null)
        {
            var cf = FindAnyObjectByType<CoverFlowUI>();
            if (cf != null)
            {
                coverFlowUI = cf.gameObject;
                Debug.Log("[PengurusAliranPopupAR] Auto-linked coverFlowUI to: " + coverFlowUI.name);
            }
            else
            {
                Debug.LogWarning("[PengurusAliranPopupAR] coverFlowUI not assigned and none found in scene.");
            }
        }

        // Try auto-linking popup panels if designer didn't assign them
        AutoLinkPopups();

        // Ensure the final confirmation popup uses the simpler instruction label.
        UpdateFinalPopupInstruction();
    }

    // Attempt to find popup GameObjects by heuristics (names / PengurusPopupObjek instances)
    private void AutoLinkPopups()
    {
        bool changed = false;

        if (popupSelamatDatang == null || popupSilaLetakKucing == null || popupSilaTekanBadan == null || popupSilaLetakKad == null)
        {
            // Search for PengurusPopupObjek instances (includes inactive via Resources)
            var popupObjs = Resources.FindObjectsOfTypeAll<PengurusPopupObjek>();
            foreach (var p in popupObjs)
            {
                if (p == null || p.gameObject == null) continue;
                string nameLow = p.gameObject.name.ToLower();

                if (popupSelamatDatang == null && (nameLow.Contains("selamat") || nameLow.Contains("welcome") || nameLow.Contains("selamatdatang")))
                {
                    popupSelamatDatang = p.gameObject; changed = true; Debug.Log("[PengurusAliranPopupAR] Auto-linked popupSelamatDatang -> " + p.gameObject.name);
                }
                else if (popupSilaLetakKucing == null && (nameLow.Contains("letak") && nameLow.Contains("kucing") || nameLow.Contains("letakkucing") || nameLow.Contains("silaletakkucing")))
                {
                    popupSilaLetakKucing = p.gameObject; changed = true; Debug.Log("[PengurusAliranPopupAR] Auto-linked popupSilaLetakKucing -> " + p.gameObject.name);
                }
                else if (popupSilaTekanBadan == null && (nameLow.Contains("tekan") || nameLow.Contains("badan") || nameLow.Contains("silatekanbadan")))
                {
                    popupSilaTekanBadan = p.gameObject; changed = true; Debug.Log("[PengurusAliranPopupAR] Auto-linked popupSilaTekanBadan -> " + p.gameObject.name);
                }
                else if (popupSilaLetakKad == null && (nameLow.Contains("kad") || nameLow.Contains("letakkad") || nameLow.Contains("silaletakkad")))
                {
                    popupSilaLetakKad = p.gameObject; changed = true; Debug.Log("[PengurusAliranPopupAR] Auto-linked popupSilaLetakKad -> " + p.gameObject.name);
                }
            }

            // Fallback: search all GameObjects by name
            if (!changed)
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string nameLow = go.name.ToLower();

                    // Only accept fallback candidates that look like real popup objects: they should either have a PengurusPopupObjek
                    // component or contain a UI Button child. This prevents non-popup objects (like Canvas_Maklumat) from being linked.
                    bool looksLikePopup = go.GetComponent<PengurusPopupObjek>() != null || go.GetComponentInChildren<UnityEngine.UI.Button>() != null;
                    if (!looksLikePopup) continue;

                    if (popupSelamatDatang == null && (nameLow.Contains("selamat") || nameLow.Contains("welcome")))
                    {
                        popupSelamatDatang = go; changed = true; Debug.Log("[PengurusAliranPopupAR] Fallback-linked popupSelamatDatang -> " + go.name);
                    }
                    if (popupSilaLetakKucing == null && (nameLow.Contains("letak") && nameLow.Contains("kucing") || nameLow.Contains("kadet") ))
                    {
                        popupSilaLetakKucing = go; changed = true; Debug.Log("[PengurusAliranPopupAR] Fallback-linked popupSilaLetakKucing -> " + go.name);
                    }
                    if (popupSilaTekanBadan == null && (nameLow.Contains("tekan") || nameLow.Contains("badan")))
                    {
                        popupSilaTekanBadan = go; changed = true; Debug.Log("[PengurusAliranPopupAR] Fallback-linked popupSilaTekanBadan -> " + go.name);
                    }
                    if (popupSilaLetakKad == null && (nameLow.Contains("kad") || nameLow.Contains("letak")))
                    {
                        popupSilaLetakKad = go; changed = true; Debug.Log("[PengurusAliranPopupAR] Fallback-linked popupSilaLetakKad -> " + go.name);
                    }
                    if (changed) break;
                }
            }

            // Try direct exact-name matching for any still-missing popup reference.
            if (popupSelamatDatang == null)
                popupSelamatDatang = FindPopupByNames(new[] { "popup", "selamat" }, new[] { "greeting" });
            if (popupSilaLetakKucing == null)
                popupSilaLetakKucing = FindPopupByExactName("Popup_LetakKucing") ?? FindPopupByExactName("Popup_LetakKucing ") ?? FindPopupByNames(new[] { "popup", "letak", "kucing" });
            if (popupSilaTekanBadan == null)
                popupSilaTekanBadan = FindPopupByExactName("Popup_tekan badan") ?? FindPopupByExactName("Popup_tekan badan ") ?? FindPopupByNames(new[] { "popup", "tekan", "badan" });
            if (popupSilaLetakKad == null)
                popupSilaLetakKad = FindPopupByExactName("Popup_LetakKad") ?? FindPopupByExactName("Popup_LetakKad ") ?? FindPopupByNames(new[] { "popup", "letak", "kad" });

            // Ensure all assigned popup references are scene instances so SetActive() works safely.
            popupSelamatDatang = InstantiatePopupIfNeeded(popupSelamatDatang);
            popupSilaLetakKucing = InstantiatePopupIfNeeded(popupSilaLetakKucing);
            popupSilaTekanBadan = InstantiatePopupIfNeeded(popupSilaTekanBadan);
            popupSilaLetakKad = InstantiatePopupIfNeeded(popupSilaLetakKad);

            // Wire button listeners on the scene instances even if the popups were assigned as prefab assets.
            PasangButangLanjut(popupSelamatDatang, () => TunjukArahanLetakKucing());
            PasangButangLanjut(popupSilaLetakKucing, () => TunjukArahanTekanBadan());
            PasangButangLanjut(popupSilaTekanBadan, () => TunjukArahanLetakKad());
            PasangButangLanjut(popupSilaLetakKad, () => MunculkanCoverFlow());
        }
    }

    // Public helper to re-run auto-linking & ensure scene instances are wired.
    public void EnsurePopupsLinked()
    {
        AutoLinkPopups();

        // Defensive: instantiate any prefab assets into scene and wire buttons again
        popupSelamatDatang = InstantiatePopupIfNeeded(popupSelamatDatang);
        popupSilaLetakKucing = InstantiatePopupIfNeeded(popupSilaLetakKucing);
        popupSilaTekanBadan = InstantiatePopupIfNeeded(popupSilaTekanBadan);
        popupSilaLetakKad = InstantiatePopupIfNeeded(popupSilaLetakKad);

        PasangButangLanjut(popupSelamatDatang, () => TunjukArahanLetakKucing());
        PasangButangLanjut(popupSilaLetakKucing, () => TunjukArahanTekanBadan());
        PasangButangLanjut(popupSilaTekanBadan, () => TunjukArahanLetakKad());
        PasangButangLanjut(popupSilaLetakKad, () => MunculkanCoverFlow());
    }

    private GameObject FindPopupByNames(string[] allContains, string[] anyContains = null)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go == null) continue;
            string nameLow = go.name.ToLower();
            bool allMatch = true;
            foreach (var substr in allContains)
            {
                if (!nameLow.Contains(substr))
                {
                    allMatch = false;
                    break;
                }
            }
            if (!allMatch) continue;

            if (anyContains != null && anyContains.Length > 0)
            {
                bool anyMatch = false;
                foreach (var substr in anyContains)
                {
                    if (nameLow.Contains(substr))
                    {
                        anyMatch = true;
                        break;
                    }
                }
                if (!anyMatch) continue;
            }

            if (go.GetComponent<PengurusPopupObjek>() != null || go.GetComponentInChildren<Button>(true) != null)
            {
                Debug.Log("[PengurusAliranPopupAR] FindPopupByNames matched " + go.name);
                return go;
            }
        }
        return null;
    }

    private GameObject FindPopupByExactName(string expectedName)
    {
        if (string.IsNullOrEmpty(expectedName))
            return null;

        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go == null) continue;
            if (string.Equals(go.name.Trim(), expectedName.Trim(), System.StringComparison.OrdinalIgnoreCase)
                && (go.GetComponent<PengurusPopupObjek>() != null || go.GetComponentInChildren<Button>(true) != null))
            {
                Debug.Log("[PengurusAliranPopupAR] FindPopupByExactName matched " + go.name);
                return go;
            }
        }
        return null;
    }

    public string GetPopupLinkStatus()
    {
        return $"selamat={(popupSelamatDatang != null ? popupSelamatDatang.name : "NULL")}, " +
               $"letakkucing={(popupSilaLetakKucing != null ? popupSilaLetakKucing.name : "NULL")}, " +
               $"tekanbadan={(popupSilaTekanBadan != null ? popupSilaTekanBadan.name : "NULL")}, " +
               $"letakkad={(popupSilaLetakKad != null ? popupSilaLetakKad.name : "NULL")}";
    }

    private void Start()
    {
        if (mulaAutoPadaStart)
        {
            if (FindAnyObjectByType<GreetingWatcher>() != null)
            {
                Debug.Log("[PengurusAliranPopupAR] GreetingWatcher detected; skipping auto-start of popup flow.");
            }
            else
            {
                MulaFlow();
            }
        }
    }

    public void MulaFlow()
    {
        tahapSemasa = TahapFlow.MenungguMula;
        TunjukSatuPanel(popupSelamatDatang);
        TutupPanel(popupSilaLetakKucing);
        TutupPanel(popupSilaTekanBadan);
        TutupPanel(popupSilaLetakKad);

        if (coverFlowUI != null)
        {
            coverFlowUI.SetActive(false);
        }
    }

    public void KucingDahDiletak()
    {
        if (tahapSemasa == TahapFlow.MenungguMula || tahapSemasa == TahapFlow.TungguLetakKucing)
        {
            // Advance logical stage immediately but delay the visual switch so the "letak kucing" popup stays visible
            tahapSemasa = TahapFlow.TungguTekanBadan;

            if (pendingDelayCoroutine != null)
            {
                StopCoroutine(pendingDelayCoroutine);
                pendingDelayCoroutine = null;
            }

            if (delaySebelumTekanBadan > 0f)
            {
                pendingDelayCoroutine = StartCoroutine(DelayTunjukTekanBadan(delaySebelumTekanBadan));
            }
            else
            {
                TunjukSatuPanel(popupSilaTekanBadan);
            }
        }
    }

    private System.Collections.IEnumerator DelayTunjukTekanBadan(float delay)
    {
        // Keep current 'letak kucing' popup visible during delay
        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        pendingDelayCoroutine = null;
        TunjukSatuPanel(popupSilaTekanBadan);
    }

    public void BadanKucingDitekan()
    {
        if (tahapSemasa == TahapFlow.TungguTekanBadan || tahapSemasa == TahapFlow.TungguLetakKucing)
        {
            if (pendingDelayCoroutine != null)
            {
                StopCoroutine(pendingDelayCoroutine);
                pendingDelayCoroutine = null;
            }

            tahapSemasa = TahapFlow.TungguLetakKad;
            TunjukSatuPanel(popupSilaLetakKad);
        }
    }

    public void KadSudahDiletak()
    {
        if (tahapSemasa == TahapFlow.TungguLetakKad)
        {
            tahapSemasa = TahapFlow.Selesai;
            MunculkanCoverFlow();
        }
    }

    public void TunjukArahanLetakKucing()
    {
        Debug.Log("[PengurusAliranPopupAR] TunjukArahanLetakKucing() called.");
        tahapSemasa = TahapFlow.TungguLetakKucing;
        TunjukSatuPanel(popupSilaLetakKucing);
        string popupName = popupSilaLetakKucing != null ? popupSilaLetakKucing.name : "NULL";
        Debug.Log("[PengurusAliranPopupAR] Showing popup: " + popupName);
    }

    public void TunjukArahanTekanBadan()
    {
        tahapSemasa = TahapFlow.TungguTekanBadan;
        TunjukSatuPanel(popupSilaTekanBadan);
    }

    public void TunjukArahanLetakKad()
    {
        tahapSemasa = TahapFlow.TungguLetakKad;
        TunjukSatuPanel(popupSilaLetakKad);
    }

    public void MunculkanCoverFlow()
    {
        tahapSemasa = TahapFlow.Selesai;
        TunjukSatuPanel(null);

        if (coverFlowUI != null)
        {
            coverFlowUI.SetActive(true);
        }

        var coverFlow = coverFlowUI != null ? coverFlowUI.GetComponent<CoverFlowUI>() : null;
        if (coverFlow != null)
        {
            coverFlow.ResetDanMunculTutorial();
        }
    }


    private void TunjukSatuPanel(GameObject panelAktif)
    {
        string panelName = panelAktif != null ? panelAktif.name : "NULL";
        Debug.Log("[PengurusAliranPopupAR] TunjukSatuPanel() - showing " + panelName);
        Debug.Log($"[PengurusAliranPopupAR] Current refs: selamat={(popupSelamatDatang!=null?popupSelamatDatang.name:"NULL")}, letakkucing={(popupSilaLetakKucing!=null?popupSilaLetakKucing.name:"NULL")}, tekanbadan={(popupSilaTekanBadan!=null?popupSilaTekanBadan.name:"NULL")}, letakkad={(popupSilaLetakKad!=null?popupSilaLetakKad.name:"NULL")} ");
        TutupPanel(popupSelamatDatang);
        TutupPanel(popupSilaLetakKucing);
        TutupPanel(popupSilaTekanBadan);
        TutupPanel(popupSilaLetakKad);

        if (panelAktif != null)
        {
            Debug.Log("[PengurusAliranPopupAR] Activating panel: " + panelAktif.name);
            panelAktif.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[PengurusAliranPopupAR] panelAktif is NULL!");
        }
    }

    private void TutupPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void PasangButangLanjut(GameObject panel, UnityEngine.Events.UnityAction onClick)
    {
        if (panel == null)
        {
            return;
        }

        Button button = panel.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }
    }

    private GameObject InstantiatePopupIfNeeded(GameObject popup)
    {
        if (popup != null && !popup.scene.IsValid())
        {
            var instance = Instantiate(popup);
            Debug.Log("[PengurusAliranPopupAR] Instantiated popup prefab into scene: " + instance.name);
            return instance;
        }
        return popup;
    }

    private void UpdateFinalPopupInstruction()
    {
        if (popupSilaLetakKad == null) return;

        SetPopupButtonLabel(popupSilaLetakKad, "Faham");
        ReplacePopupTextContent(popupSilaLetakKad, "Sahkan", "Faham");
    }

    private void SetPopupButtonLabel(GameObject popup, string label)
    {
        if (popup == null) return;

        Button button = popup.GetComponentInChildren<Button>(true);
        if (button == null) return;

        TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpText != null)
        {
            tmpText.text = label;
            return;
        }

        UnityEngine.UI.Text legacyText = button.GetComponentInChildren<UnityEngine.UI.Text>(true);
        if (legacyText != null)
        {
            legacyText.text = label;
        }
    }

    private void ReplacePopupTextContent(GameObject popup, string searchText, string replaceText)
    {
        if (popup == null) return;

        foreach (var tmpText in popup.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmpText.text.Contains(searchText))
            {
                tmpText.text = tmpText.text.Replace(searchText, replaceText);
            }
        }

        foreach (var legacyText in popup.GetComponentsInChildren<UnityEngine.UI.Text>(true))
        {
            if (legacyText.text.Contains(searchText))
            {
                legacyText.text = legacyText.text.Replace(searchText, replaceText);
            }
        }
    }
}
