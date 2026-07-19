using UnityEngine;
using UnityEngine.UI;

public class GreetingWatcher : MonoBehaviour
{
    [SerializeField] private GameObject greetingObject;
    [SerializeField] private PengurusAliranPopupAR flowManager;
    [SerializeField] private GameObject coachingUI;
    [SerializeField] private GameObject disclaimerPopup;
    [SerializeField] private GameObject tapToPlacePrompt;
    [SerializeField] private GameObject scanSurfacePrompt;
    [SerializeField] private bool showCoachingUIOnStart = false;
    [SerializeField] private bool showTapToPlacePromptOnStart = false;
    [SerializeField] private float checkInterval = 0.2f;

    private bool started = false;
    private float timer = 0f;
    private bool lastGreetingActive = false;

    void Start()
    {
        Debug.Log("[GreetingWatcher] Start() called.");
        if (flowManager == null)
        {
            flowManager = FindAnyObjectByType<PengurusAliranPopupAR>();
            string managerName = flowManager != null ? flowManager.name : "NULL";
            Debug.Log("[GreetingWatcher] Flow manager found: " + managerName);
        }

        AutoLinkFlowObjects();

        if (greetingObject == null)
        {
            Debug.LogWarning("[GreetingWatcher] 'greetingObject' not assigned in Inspector or scene. Starting popup flow as fallback.");
            StartPopupFlow();
            return;
        }
        else
        {
            lastGreetingActive = greetingObject.activeInHierarchy;
            Debug.Log("[GreetingWatcher] Greeting object assigned: " + greetingObject.name + ", Active: " + lastGreetingActive);
            if (!lastGreetingActive)
            {
                Debug.Log("[GreetingWatcher] Greeting object already inactive at Start. Starting popup flow immediately.");
                StartPopupFlow();
                return;
            }
        }
    }

    void Update()
    {
        if (started) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = checkInterval;

        if (greetingObject != null)
        {
            bool isActive = greetingObject.activeInHierarchy;
            if (lastGreetingActive && !isActive)
            {
                Debug.Log("[GreetingWatcher] Greeting transitioned from active to inactive. Starting popup flow...");
                StartPopupFlow();
            }
            lastGreetingActive = isActive;
        }
        else
        {
            Debug.LogWarning("[GreetingWatcher] greetingObject is NULL in Update()!");
        }
    }

    private void AutoLinkFlowObjects()
    {
        if (flowManager == null)
        {
            flowManager = FindAnyObjectByType<PengurusAliranPopupAR>();
            if (flowManager != null)
            {
                Debug.Log("[GreetingWatcher] Auto-linked flowManager: " + flowManager.name);
            }
            else
            {
                Debug.LogWarning("[GreetingWatcher] Could not auto-link PengurusAliranPopupAR. Creating runtime instance.");
                // Create a runtime GameObject with the flow manager so popups in scene can be controlled
                GameObject runtimeFlow = new GameObject("Runtime_PengurusAliranPopupAR");
                flowManager = runtimeFlow.AddComponent<PengurusAliranPopupAR>();
                Debug.Log("[GreetingWatcher] Runtime PengurusAliranPopupAR created: " + runtimeFlow.name);
            }
        }

        if (greetingObject == null)
        {
            var foundGreeting = GameObject.Find("Greeting") ?? GameObject.Find("GreetingObject") ?? GameObject.Find("Greeting Prompt") ?? GameObject.Find("GreetingPrompt") ?? GameObject.Find("GreetingCTA");
            if (foundGreeting != null)
            {
                greetingObject = foundGreeting;
                Debug.Log("[GreetingWatcher] Auto-linked greetingObject: " + greetingObject.name);
            }
            else
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if (n.Contains("greeting") || n.Contains("greeting prompt") || n.Contains("greetingcta"))
                    {
                        greetingObject = go;
                        Debug.Log("[GreetingWatcher] Fuzzy-linked greetingObject: " + greetingObject.name);
                        break;
                    }
                }
            }
        }

        if (coachingUI == null)
        {
            // Try exact name first
            coachingUI = GameObject.Find("CoachingUI");
            if (coachingUI == null)
            {
                // Fuzzy search: look for any GameObject with 'coaching' in its name
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if (n.Contains("coaching") || n.Contains("coaching ui") || n.Contains("coaching_ui"))
                    {
                        coachingUI = go;
                        Debug.Log("[GreetingWatcher] Fuzzy-linked coachingUI: " + coachingUI.name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("[GreetingWatcher] Auto-linked coachingUI: " + coachingUI.name);
            }
        }

        if (tapToPlacePrompt == null)
        {
            // Try exact names first
            tapToPlacePrompt = GameObject.Find("TapToPlacePrompt") ?? GameObject.Find("Prompt_TapToPlace") ?? GameObject.Find("Prompt TapToPlace");
            if (tapToPlacePrompt == null)
            {
                // Fuzzy search: look for names containing both 'tap' and 'place' or 'prompt'
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if ((n.Contains("tap") && n.Contains("place")) || n.Contains("prompt_tap") || n.Contains("prompt tap") || n.Contains("taptoplace") || n.Contains("prompt_taptoplace"))
                    {
                        tapToPlacePrompt = go;
                        Debug.Log("[GreetingWatcher] Fuzzy-linked tapToPlacePrompt: " + tapToPlacePrompt.name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("[GreetingWatcher] Auto-linked tapToPlacePrompt: " + tapToPlacePrompt.name);
            }
        }

        if (scanSurfacePrompt == null)
        {
            // Common names used in AR template
            scanSurfacePrompt = GameObject.Find("Prompt Scan Surfaces") ?? GameObject.Find("Prompt_ScanSurfaces") ?? GameObject.Find("PromptScanSurfaces") ?? GameObject.Find("ScanSurfacesPrompt");
            if (scanSurfacePrompt == null)
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if ((n.Contains("scan") && n.Contains("surface")) || n.Contains("scan surfaces") || n.Contains("scan_surfaces"))
                    {
                        scanSurfacePrompt = go;
                        Debug.Log("[GreetingWatcher] Fuzzy-linked scanSurfacePrompt: " + scanSurfacePrompt.name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("[GreetingWatcher] Auto-linked scanSurfacePrompt: " + scanSurfacePrompt.name);
            }
        }

        if (disclaimerPopup == null)
        {
            disclaimerPopup = GameObject.Find("Prompt Disclaimer") ?? GameObject.Find("Disclaimer") ?? GameObject.Find("PromptDisclaimer");
            if (disclaimerPopup == null)
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in all)
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if (n.Contains("disclaimer"))
                    {
                        disclaimerPopup = go;
                        Debug.Log("[GreetingWatcher] Fuzzy-linked disclaimerPopup: " + disclaimerPopup.name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("[GreetingWatcher] Auto-linked disclaimerPopup: " + disclaimerPopup.name);
            }
        }
    }

    public void StartPopupFlow()
    {
        if (started)
        {
            Debug.LogWarning("[GreetingWatcher] Flow already started.");
            return;
        }
        started = true;
        Debug.Log("[GreetingWatcher] StartPopupFlow() called.");

        if (flowManager != null)
        {
            if (coachingUI != null && !showCoachingUIOnStart)
            {
                HideCoachingElements();
                Debug.Log("[GreetingWatcher] Coaching UI elements hidden until disclaimer is accepted (root left intact).");
            }

            if (tapToPlacePrompt != null && !showTapToPlacePromptOnStart)
            {
                tapToPlacePrompt.SetActive(false);
                Debug.Log("[GreetingWatcher] Tap to place prompt hidden until disclaimer is accepted.");
            }

            if (disclaimerPopup != null)
            {
                if (!disclaimerPopup.scene.IsValid())
                {
                    GameObject inst = Instantiate(disclaimerPopup);
                    disclaimerPopup = inst;
                    Debug.Log("[GreetingWatcher] Instantiated disclaimerPopup prefab into scene: " + disclaimerPopup.name);
                }
                disclaimerPopup.SetActive(true);
                Debug.Log("[GreetingWatcher] Disclaimer popup shown.");
                SetupDisclaimerButton();
            }
            else
            {
                Debug.Log("[GreetingWatcher] No disclaimer popup assigned; starting the placement flow directly.");
                BeginPlacementFlow();
            }
        }
        else
        {
            Debug.LogError("[GreetingWatcher] PengurusAliranPopupAR NOT FOUND! Assign it manually or ensure it exists in scene.");
        }
    }

    private void SetupDisclaimerButton()
    {
        if (disclaimerPopup == null) return;

        Button button = disclaimerPopup.GetComponentInChildren<Button>(true);
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(AcceptDisclaimer);
            Debug.Log("[GreetingWatcher] Disclaimer button listener attached.");
        }
        else
        {
            Debug.LogWarning("[GreetingWatcher] Disclaimer popup has no Button child to attach AcceptDisclaimer().");
        }
    }

    private void HideCoachingElements()
    {
        if (coachingUI == null) return;

        bool hiddenSomething = false;
        foreach (Transform child in coachingUI.transform)
        {
            if (child == null) continue;
            string n = child.name.ToLower();
            if (n.Contains("coaching") || n.Contains("tutorial") || n.Contains("coach"))
            {
                child.gameObject.SetActive(false);
                hiddenSomething = true;
            }
        }

        if (!hiddenSomething)
        {
            var cg = coachingUI.GetComponentInChildren<CanvasGroup>(true);
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
                hiddenSomething = true;
            }
        }

        if (!hiddenSomething)
        {
            coachingUI.SetActive(false);
        }
    }

    public void AcceptDisclaimer()
    {
        if (disclaimerPopup != null)
        {
            disclaimerPopup.SetActive(false);
            Debug.Log("[GreetingWatcher] Disclaimer popup accepted and hidden.");
        }

        // Do NOT re-show scan/tap prompts; keep them hidden per user preference.
        BeginPlacementFlow();
    }

    private void BeginPlacementFlow()
    {
        // Ensure popup flow manager has linked its popup references (in case they were inactive at Awake)
        if (flowManager != null)
        {
            flowManager.EnsurePopupsLinked();
            Debug.Log("[GreetingWatcher] EnsurePopupsLinked() complete.");
            Debug.Log("[GreetingWatcher] flowManager refs after link: " + flowManager.GetPopupLinkStatus());
        }

        if (scanSurfacePrompt != null)
        {
            if (!scanSurfacePrompt.scene.IsValid())
            {
                GameObject instScan = Instantiate(scanSurfacePrompt);
                scanSurfacePrompt = instScan;
                Debug.Log("[GreetingWatcher] Instantiated scanSurfacePrompt prefab into scene: " + scanSurfacePrompt.name);
            }
            scanSurfacePrompt.SetActive(false);
            Debug.Log("[GreetingWatcher] Scan surfaces prompt hidden until after disclaimer.");
        }

        if (flowManager != null)
        {
            Debug.Log("[GreetingWatcher] Calling TunjukArahanLetakKucing() with flowManager.");
            flowManager.TunjukArahanLetakKucing();
            Debug.Log("[GreetingWatcher] Placement flow started successfully.");
        }
        else
        {
            Debug.LogError("[GreetingWatcher] flowManager is null in BeginPlacementFlow().");
        }
    }

    // Called by AR placement code when the cat has been placed in the scene.
    public void NotifyKucingPlaced()
    {
        Debug.Log("[GreetingWatcher] NotifyKucingPlaced() called.");
        if (tapToPlacePrompt != null)
        {
            tapToPlacePrompt.SetActive(false);
            Debug.Log("[GreetingWatcher] Tap to place prompt hidden after placement.");
        }

        if (coachingUI != null)
        {
            coachingUI.SetActive(false);
            Debug.Log("[GreetingWatcher] Coaching UI hidden after placement.");
        }

        if (flowManager != null)
        {
            Debug.Log("[GreetingWatcher] Informing flowManager that cat was placed.");
            flowManager.KucingDahDiletak();
        }
    }
}
