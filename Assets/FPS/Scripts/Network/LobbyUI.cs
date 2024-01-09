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
    [SerializeField] private TMP_Dropdown gameModeDropdown;
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
    private PvPGameMode gameMode;


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

        PopulateDropDownWithEnum(gameModeDropdown, gameMode);

        gameModeDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(gameModeDropdown); });
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
        l.GetComponent<LobbyDisplay>().SetDisplay(lobby.Name, lobby.MaxPlayers, lobby.MaxPlayers - lobby.AvailableSlots, lobby.Data["GameMode"].Value, lobby);
    }

    public void InstanciatePlayerDisplay(Player player)
    {
        GameObject p = Instantiate(playerDisplayPrefab, playerListContainer);
        p.GetComponent<PlayerDisplay>().SetDisplay(player.Data["PlayerName"].Value);
    }

    public void CreateLobby()
	{
        string selectedGameMode = "";

		switch (gameMode)
		{
			case PvPGameMode.CONTROL:
                selectedGameMode = "control";
				break;
			case PvPGameMode.MME:
                selectedGameMode = "mme";
                break;
			case PvPGameMode.DEATHMATCH:
                selectedGameMode = "deathmatch";
                break;
			default:
                selectedGameMode = "control";
                break;
		}

		lobbyManager.CreateLobby(lobbyName, selectedGameMode);
        currentLobbyName.text = lobbyName;
        currentLobbyGameMode.text = selectedGameMode;
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

    void DropdownValueChanged(TMP_Dropdown change)
    {
        gameMode = (PvPGameMode)change.value; //Convert dropwdown value to enum
    }
}
