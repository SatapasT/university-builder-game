using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicTutorial : MonoBehaviour
{
    public static BasicTutorial Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject tutorialCanvasRoot;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Settings")]
    [SerializeField] private float menuOpenCheckCooldown = 0.1f;

    [Header("Tutorial Highlights")]
    [SerializeField] private TutorialHighlighter tutorialTree;
    [SerializeField] private TutorialHighlighter tutorialRock;
    [SerializeField] private TutorialHighlighter tutorialWorkshop;

    private enum Step
    {
        Move,
        OpenMenu,
        Gather,
        UseWorkshop,
        Done
    }

    private Step step = Step.Move;

    private Dictionary<ResourceType, int> startingResources = new();
    private float menuCheckTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (tutorialCanvasRoot != null) tutorialCanvasRoot.SetActive(true);
        SetStep(Step.Move);
        CacheStartingResources();
    }

    private void Update()
    {
        if (step == Step.Done) return;

        switch (step)
        {
            case Step.Move:
                if (PressedMoveKeyWASD())
                {
                    SetStep(Step.OpenMenu);
                }
                break;

            case Step.OpenMenu:
                if (PressedTab() || IsWorkshopMenuOpen())
                {
                    SetStep(Step.Gather);
                    CacheStartingResources();
                }
                break;

            case Step.Gather:
                if (HasGained(ResourceType.Wood) || HasGained(ResourceType.Stone))
                {
                    SetStep(Step.UseWorkshop);
                }
                break;

            case Step.UseWorkshop:
                menuCheckTimer -= Time.deltaTime;

                if (PressedTab())
                    menuCheckTimer = menuOpenCheckCooldown;

                if (menuCheckTimer <= 0f && IsWorkshopMenuOpen())
                    SetStep(Step.Done);

                break;
        }
    }

    public void NotifyWorkshopOpened()
    {
        if (step == Step.UseWorkshop)
            SetStep(Step.Done);
    }

    private void SetStep(Step newStep)
    {
        step = newStep;

        tutorialCanvasRoot?.SetActive(step != Step.Done);

        tutorialTree?.DisableHighlight();
        tutorialRock?.DisableHighlight();
        tutorialWorkshop?.DisableHighlight();

        switch (step)
        {
            case Step.Move:
                tutorialText.text =
                    "<b>Tutorial</b>\nMove using <color=yellow>W A S D</color>.";
                break;

            case Step.OpenMenu:
                tutorialText.text =
                    "<b>Tutorial</b>\nPress <color=yellow>TAB</color> to open the menu.";
                break;

            case Step.Gather:
                tutorialText.text =
                    "<b>Tutorial</b>\nUse <color=yellow>Left Click</color> on a <color=yellow>Tree</color> or <color=yellow>Rock</color>.";

                tutorialTree?.EnableHighlight();
                tutorialRock?.EnableHighlight();
                break;

            case Step.UseWorkshop:
                tutorialText.text =
                    "<b>Tutorial</b>\nInteract with the <color=yellow>Workshop</color>.";

                tutorialWorkshop?.EnableHighlight();
                break;

            case Step.Done:
                tutorialText.text = "";
                break;
        }
    }


    private bool PressedMoveKeyWASD()
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        return kb.wKey.wasPressedThisFrame ||
               kb.aKey.wasPressedThisFrame ||
               kb.sKey.wasPressedThisFrame ||
               kb.dKey.wasPressedThisFrame;
    }

    private bool PressedTab()
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        return kb.tabKey.wasPressedThisFrame;
    }

    private bool IsWorkshopMenuOpen()
    {
        return WorkshopUI.Instance != null && WorkshopUI.Instance.IsOpen;
    }

    private void CacheStartingResources()
    {
        startingResources.Clear();

        if (ResourcesManager.Instance == null) return;

        var all = ResourcesManager.Instance.GetAllResources();
        foreach (var kvp in all)
            startingResources[kvp.Key] = kvp.Value;
    }

    private bool HasGained(ResourceType type)
    {
        if (ResourcesManager.Instance == null) return false;

        var all = ResourcesManager.Instance.GetAllResources();
        all.TryGetValue(type, out int now);

        startingResources.TryGetValue(type, out int start);
        return now > start;
    }
}
