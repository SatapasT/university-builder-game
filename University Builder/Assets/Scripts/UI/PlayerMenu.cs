using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerMenu : MonoBehaviour
{
    public static PlayerMenu Instance { get; private set; }

    [SerializeField] private List<GameObject> AllMenuItem = new();
    [SerializeField] private List<GameObject> MainMenuItem = new();

    [SerializeField] private GameObject MenuBackdrop;
    [SerializeField] private GameObject MainButtonContainer;
    [SerializeField] private GameObject PlayerStatsContainer;
    [SerializeField] private GameObject PlayerModel;
    [SerializeField] private GameObject TeleportLocation;

    [Header("Player Stats UI")]
    [SerializeField] private TextMeshProUGUI playerAxeInfo;
    [SerializeField] private TextMeshProUGUI playerPickaxeInfo;
    [SerializeField] private TextMeshProUGUI playerBootsInfo;

    public bool IsOpen { get; private set; }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (GameObject panel in AllMenuItem)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    public void ToggleMenu()
    {
        if (IsOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu()
    {
        UIManager.Instance.SetMenuState(true);
        foreach (GameObject panel in MainMenuItem)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        IsOpen = true;
    }

    public void CloseMenu()
    {
        UIManager.Instance.SetMenuState(false);
        foreach (GameObject panel in AllMenuItem)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    public void TeleportToWorkbench()
    {
        var controller = PlayerModel.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        PlayerModel.transform.position = TeleportLocation.transform.position;

        if (controller != null)
            controller.enabled = true;

        CloseMenu();
    }

    public void openMainMenu()
    {
        MainButtonContainer.SetActive(true);
    }
    public void closeMainMenu()
    {
        MainButtonContainer.SetActive(false);
    }

    private string BuildToolLine(ToolType type)
    {
        if (PlayerStats.Instance == null)
            return $"{type}: (no stats – PlayerStats missing)";

        ToolInfo info = PlayerStats.Instance.GetCurrentToolInfo(type);

        if (info == null)
            return $"{type}: None";

        // Name
        string line = $"<b>{info.Name}</b>\n";

        // Description
        if (!string.IsNullOrWhiteSpace(info.Description))
            line += info.Description + "\n";

        // Stats
        string stats = "";

        if (info.HarvestAmount > 0)
            stats += $"- Harvest: +{info.HarvestAmount} per action\n";

        if (info.MovementSpeedBonus > 0f && info.MovementSpeedBonus != 1f)
            stats += $"- Move Speed: x{info.MovementSpeedBonus:0.00}\n";

        if (string.IsNullOrWhiteSpace(stats))
            stats = "- No bonuses\n";

        line += stats;

        return line.TrimEnd('\n');
    }

    public void openPlayerStats()
    {
        if (PlayerStatsContainer != null)
            PlayerStatsContainer.SetActive(true);

        if (PlayerStats.Instance == null)
        {
            if (playerAxeInfo != null) playerAxeInfo.text = "No stats";
            if (playerPickaxeInfo != null) playerPickaxeInfo.text = "No stats";
            if (playerBootsInfo != null) playerBootsInfo.text = "No stats";
            return;
        }

        // Axe
        if (playerAxeInfo != null)
            playerAxeInfo.text = BuildToolLine(ToolType.Axe);

        // Pickaxe
        if (playerPickaxeInfo != null)
            playerPickaxeInfo.text = BuildToolLine(ToolType.Pickaxe);

        // Boots
        if (playerBootsInfo != null)
            playerBootsInfo.text = BuildToolLine(ToolType.Boots);
    }


    public void closePlayerStats()
    {
        PlayerStatsContainer.SetActive(false);
    }
}
