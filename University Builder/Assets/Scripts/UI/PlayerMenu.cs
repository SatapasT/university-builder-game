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
            return $"<color=red><b>{type}</b>: PlayerStats missing</color>";

        ToolInfo info = PlayerStats.Instance.GetCurrentToolInfo(type);

        if (info == null)
            return $"<color=red><b>{type}</b>: None</color>";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // ----- NAME -----
        sb.AppendLine($"<b><color=yellow>{info.Name}</color></b>");
        sb.AppendLine();

        // ----- DESCRIPTION -----
        if (!string.IsNullOrWhiteSpace(info.Description))
        {
            sb.AppendLine($"<color=#DDDDDD>{info.Description}</color>");
            sb.AppendLine();
        }

        // ----- STATS HEADER -----
        sb.AppendLine("<b><color=orange>STATS</color></b>");

        bool hasStat = false;

        // ----- HARVEST STAT -----
        if (info.HarvestAmount > 0)
        {
            hasStat = true;
            sb.AppendLine($"<color=#90EE90>- Harvest:</color> <b>+{info.HarvestAmount}</b> per action");
        }

        // ----- SPEED STAT -----
        if (info.MovementSpeedBonus > 0f && info.MovementSpeedBonus != 1f)
        {
            hasStat = true;
            sb.AppendLine($"<color=#ADD8E6>- Move Speed:</color> <b>x{info.MovementSpeedBonus:0.00}</b>");
        }

        // ----- NO BONUS CASE -----
        if (!hasStat)
        {
            sb.AppendLine("<color=grey>- No bonuses</color>");
        }

        return sb.ToString().TrimEnd();
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
