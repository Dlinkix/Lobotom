using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Данные")]
    public GameDataMenu gameDataMenu;
    public CharactersSelected charactersSelected;
    public AbnormalityAdder abnormalityAdder;

    [Header("UI")]
    public TMPro.TMP_Text dayText;
    public Button newDayButton;
    public Button startWorkButton;
    public Button endDayButton;
    public Button restartDayButton;

    [Header("Панели")]
    public GameObject abnormalitySelectionPanel;

    public enum DayState
    {
        NewDay,
        CurrentDay,
        DayInProgress,
        DayEnd
    }

    public int currentDay = 1;
    public DayState currentState = DayState.NewDay;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        LoadSavedState();
        UpdateDayUI();

        if (newDayButton != null)
            newDayButton.onClick.AddListener(StartNewDay);
        if (startWorkButton != null)
            startWorkButton.onClick.AddListener(BeginWorkDay);
        if (endDayButton != null)
            endDayButton.onClick.AddListener(EndWorkDay);
        if (restartDayButton != null)
            restartDayButton.onClick.AddListener(RestartCurrentDay);

        if (currentState == DayState.NewDay)
        {
            OpenAbnormalitySelection();
        }
    }

    public void SaveCurrentState()
    {
        if (gameDataMenu == null) return;

        gameDataMenu.savedCurrentDay = currentDay;
        gameDataMenu.savedDayState = currentState.ToString();
    }

    private void LoadSavedState()
    {
        if (gameDataMenu == null) return;

        Debug.Log($"LoadSavedState ДО загрузки: savedCurrentDay = {gameDataMenu.savedCurrentDay}, savedDayState = {gameDataMenu.savedDayState}");

        currentDay = gameDataMenu.savedCurrentDay;

        switch (gameDataMenu.savedDayState)
        {
            case "NewDay":
                currentState = DayState.NewDay;
                break;
            case "CurrentDay":
                currentState = DayState.CurrentDay;
                break;
            case "DayInProgress":
                currentState = DayState.DayInProgress;
                break;
            case "DayEnd":
                currentState = DayState.DayEnd;
                break;
            default:
                currentState = DayState.NewDay;
                break;
        }

        Debug.Log($"Загружено состояние: Day {currentDay}, State {currentState}");
    }

    public void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Day {currentDay}";
    }

    public void StartNewDay()
    {
        if (currentState != DayState.DayEnd && currentState != DayState.NewDay)
        {
            Debug.Log("Нельзя начать новый день сейчас!");
            return;
        }

        currentState = DayState.NewDay;
        SaveCurrentState();
        OnNewDayStarted();
    }

    public void RestartCurrentDay()
    {
        if (currentState != DayState.DayInProgress && currentState != DayState.CurrentDay)
        {
            Debug.Log("Нельзя переиграть этот день сейчас!");
            return;
        }

        currentState = DayState.CurrentDay;
        ResetAgentsToStartPositions();
        ResetAllAbnormalitiesWork();
        CloseAbnormalitySelection();
        SaveCurrentState();
        Debug.Log($"День {currentDay} переигрывается!");
    }

    private void OnNewDayStarted()
    {
        Debug.Log($"Наступил День {currentDay}! (Новый день)");
        UpdateDayUI();
        OpenAbnormalitySelection();
        SaveCurrentState();
    }

    public void ContinueCurrentDay()
    {
        if (currentState != DayState.NewDay && currentState != DayState.CurrentDay)
        {
            Debug.Log("Нельзя продолжить текущий день!");
            return;
        }

        currentState = DayState.CurrentDay;
        CloseAbnormalitySelection();
        SaveCurrentState();
        Debug.Log($"Продолжаем День {currentDay}. Аномалия уже выбрана.");
    }

    public void OpenAbnormalitySelection()
    {
        if (abnormalitySelectionPanel != null)
        {
            abnormalitySelectionPanel.SetActive(true);
            Debug.Log("Панель выбора аномалии открыта");
        }
    }

    public void CloseAbnormalitySelection()
    {
        if (abnormalitySelectionPanel != null)
        {
            abnormalitySelectionPanel.SetActive(false);
            Debug.Log("Панель выбора аномалии закрыта");
        }
    }

    public void BeginWorkDay()
    {
        if (currentState != DayState.NewDay && currentState != DayState.CurrentDay)
        {
            Debug.Log("Нельзя начать рабочий день сейчас!");
            return;
        }

        CloseAbnormalitySelection();
        currentState = DayState.DayInProgress;
        SaveCurrentState();
        Debug.Log($"День {currentDay} начался! Агенты могут работать.");
    }

    public void EndWorkDay()
    {
        if (currentState != DayState.DayInProgress)
        {
            Debug.Log("Нельзя закончить день сейчас!");
            return;
        }

        currentState = DayState.DayEnd;
        SaveCurrentState();
        Debug.Log($"День {currentDay} закончен!");
    }

    public void NextDay()
    {
        if (currentState != DayState.DayEnd)
        {
            Debug.Log("Нельзя перейти к следующему дню сейчас!");
            return;
        }

        currentDay++;
        currentState = DayState.NewDay;

        if (gameDataMenu != null)
        {
            gameDataMenu.savedCurrentDay = currentDay;
            gameDataMenu.savedDayState = currentState.ToString();
        }

        UpdateDayUI();
        OnNewDayStarted();
    }

    private void ResetAgentsToStartPositions()
    {
        Agent[] agents = FindObjectsOfType<Agent>();
        foreach (var agent in agents)
        {
            if (agent.StayRoom != null)
            {
                agent.currentRoom = agent.StayRoom;
                RectTransform rectTransform = agent.GetComponent<RectTransform>();
                if (rectTransform != null && agent.StayRoom.waypoint != null)
                {
                    rectTransform.position = agent.StayRoom.waypoint.position;
                }
            }
            agent.currentState = Agent.State.Idle;
        }
        Debug.Log("Агенты возвращены на стартовые позиции");
    }

    private void ResetAllAbnormalitiesWork()
    {
        AbnormalityController[] abnormalities = FindObjectsOfType<AbnormalityController>();
        foreach (var abnormality in abnormalities)
        {
            if (abnormality.isWorking)
            {
                abnormality.isWorking = false;
                abnormality.workingAgent = null;
                abnormality.workProgress = 0f;
            }
        }
        Debug.Log("Прогресс работ сброшен");
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public DayState GetCurrentState()
    {
        return currentState;
    }

    public bool IsNewDay()
    {
        return currentState == DayState.NewDay;
    }
}