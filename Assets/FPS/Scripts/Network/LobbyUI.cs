using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System;

public enum PvPGameMode
{
    CONTROL,
    MME,
    DEATHMATCH
}

public enum GlobalGameMode
{
    PVE,
    PVP
}

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private GameObject lobbyDisplayPrefab;
    [SerializeField] private GameObject playerDisplayPrefab;


    [Header("Panels")]
    [SerializeField] private GameObject NameSelectionPanel;
    [SerializeField] private GameObject LobbyListPanel;
    [SerializeField] private GameObject CreateLobbyPanel;
    [SerializeField] public GameObject JoinedLobbyPanel;




    [Header("Name Selection Panels")]
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button authButton;

    [Header("Lobby List Panels")]
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private Button createPanelButton;


    [Header("Create Lobby Panels")]
    [SerializeField] private TMP_InputField lobbyField;
    [SerializeField] private GameObject pvpGameModeObject;
    [SerializeField] private TMP_Dropdown pvpGameModeDropdown;
    [SerializeField] private TMP_Dropdown globalGameModeDropdown;
    [SerializeField] private Button createButton;

    [Header("Joined Lobby Panels")]
    public TMP_Text currentLobbyName;
    public TMP_Text currentLobbyPlayers;
    public TMP_Text currentLobbyGameMode;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject startButtonObj;
    [SerializeField] private Button startButton;




    private string playerName;
    private string lobbyName;
    private GameObject currentPanel;
    private PvPGameMode pvpGameMode;
    private GlobalGameMode globalGameMode;


    private void Awake()
	{
        currentPanel = NameSelectionPanel;

        authButton.onClick.AddListener(() =>
        {
            lobbyManager.Authenticate(playerName);
            SwitchPanel(LobbyListPanel);
        });

        createPanelButton.onClick.AddListener(() =>
        {
            SwitchPanel(CreateLobbyPanel);

            PopulateDropDownWithEnum(globalGameModeDropdown, globalGameMode);

            globalGameModeDropdown.onValueChanged.AddListener(delegate { GlobalDropdownValueChanged(globalGameModeDropdown); });

            PopulateDropDownWithEnum(pvpGameModeDropdown, pvpGameMode);

            pvpGameModeDropdown.onValueChanged.AddListener(delegate { PvPDropdownValueChanged(pvpGameModeDropdown); });

            pvpGameModeObject.SetActive(false);

            lobbyField.text = "";
        });

        createButton.onClick.AddListener(() =>
        {
            CreateLobby();
            SwitchPanel(JoinedLobbyPanel);
        });

        quitButton.onClick.AddListener(() =>
        {
            lobbyManager.LeaveLobby();
            SwitchPanel(LobbyListPanel);
        });
    }
    public void SetAsHost()
	{
        if (lobbyManager.IsLobbyHost())
        {
            startButtonObj.SetActive(true);
            startButton.onClick.AddListener(() =>
            {
                lobbyManager.StartGame();
            });
        }
    }

    public void OnNameChange()
	{
        playerName = nameField.text;

        if(nameField.text.Length > 0)
            authButton.interactable = true;
        else
            authButton.interactable = false;
    }

    public void OnLobbyChange()
    {
        lobbyName = lobbyField.text;

        if (lobbyField.text.Length > 0)
            createButton.interactable = true;
        else
            createButton.interactable = false;
    }

    private void SwitchPanel(GameObject newPanel)
	{
        currentPanel.SetActive(false);
        newPanel.SetActive(true);
        currentPanel = newPanel;
	}
    
    public void SwitchLobbyPanel()
	{
        LobbyListPanel.SetActive(false);
        JoinedLobbyPanel.SetActive(true);
        currentPanel = JoinedLobbyPanel;
	}

    public void ClearLobbyDisplay()
	{
		foreach (Transform child in lobbyListContainer)
		{
            Destroy(child.gameObject);
		}
	}
    
    public void ClearPlayerDisplay()
	{
		foreach (Transform child in playerListContainer)
		{
            Destroy(child.gameObject);
		}
	}

    public void InstanciateLobbyDisplay(Lobby lobby)
	{
        GameObject l = Instantiate(lobbyDisplayPrefab, lobbyListContainer);
        l.GetComponent<LobbyDisplay>().lobbyManager = lobbyManager;
        l.GetComponent<LobbyDisplay>().lobbyUI = this;
        l.GetComponent<LobbyDisplay>().SetDisplay(lobby.Name, lobby.MaxPlayers, lobby.MaxPlayers - lobby.AvailableSlots, lobby.Data["GlobalGameMode"].Value, lobby.Data["PvPGameMode"].Value, lobby);
    }

    public void InstanciatePlayerDisplay(Player player)
    {
        GameObject p = Instantiate(playerDisplayPrefab, playerListContainer);
        p.GetComponent<PlayerDisplay>().SetDisplay(player.Data["PlayerName"].Value);
    }

    public void CreateLobby()
	{
        string selectedGlobalGameMode = "";
        string selectedPvPGameMode = "";

		switch (globalGameMode)
		{
			case GlobalGameMode.PVE:
                selectedGlobalGameMode = "pve";
                break;
			case GlobalGameMode.PVP:
                selectedGlobalGameMode = "pvp";

                switch (pvpGameMode)
                {
                    case PvPGameMode.CONTROL:
                        selectedPvPGameMode = "control";
                        break;
                    case PvPGameMode.MME:
                        selectedPvPGameMode = "mme";
                        break;
                    case PvPGameMode.DEATHMATCH:
                        selectedPvPGameMode = "deathmatch";
                        break;
                    default:
                        selectedPvPGameMode = "control";
                        break;
                }

                break;
			default:
				break;
		}

		lobbyManager.CreateLobby(lobbyName, selectedGlobalGameMode, selectedPvPGameMode);
        currentLobbyName.text = lobbyName;
        currentLobbyGameMode.text = selectedPvPGameMode == "" ? selectedGlobalGameMode : selectedGlobalGameMode + " | " + selectedPvPGameMode;
	}

    void PopulateDropDownWithEnum(TMP_Dropdown dropdown, Enum targetEnum)//You can populate any dropdown with any enum with this method
    {
        Type enumType = targetEnum.GetType();//Type of enum(FormatPresetType in my example)
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < Enum.GetNames(enumType).Length; i++)//Populate new Options
        {
            newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, i)));
        }

        dropdown.ClearOptions();//Clear old options
        dropdown.AddOptions(newOptions);//Add new options
    }

    void PvPDropdownValueChanged(TMP_Dropdown change)
    {
        pvpGameMode = (PvPGameMode)change.value; //Convert dropwdown value to enum
    }    
    void GlobalDropdownValueChanged(TMP_Dropdown change)
    {
        globalGameMode = (GlobalGameMode)change.value; //Convert dropwdown value to enum

		switch (globalGameMode)
		{
			case GlobalGameMode.PVE:
                pvpGameModeObject.SetActive(false);
				break;
			case GlobalGameMode.PVP:
                pvpGameModeObject.SetActive(true);
                break;
			default:
				break;
		}
	}
}
