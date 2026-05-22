using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickHandler : MonoBehaviour
{
    [Header("UI панель с кнопками")]
    public GameObject interactionPanel;
    public Button instinctButton;
    public Button insightButton;
    public Button attachmentButton;
    public Button repressionButton;
    public Button StopButton;
    public Button DefaultButton;
    public Button UpSpeedButton;
    public Button UpUpSpeedButton;

    public Image instinctIcon;
    public Image insightIcon;
    public Image attachmentIcon;
    public Image repressionIcon;
    public TextMeshProUGUI textMeshPro;
    public int PeBoxHave;
    public int PeBoxNeed;
    public Image abnormalityDisplayImage;
    public GameDataMenu gameDataMenu;
    public Button endDayButton;
    public int loadedScene;

    [Header("Настройки скорости")]
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float fastSpeed = 1.5f;
    [SerializeField] private float veryFastSpeed = 2f;

    [Header("Цвета кнопок")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private Button[] speedButtons;
    private int currentSpeedIndex = 1;
    private float[] speedValues;
    private float previousTimeScale = 1f;

    public static ClickHandler currentOpenPanel { get; private set; }

    [Header("Меню завершения дня")]
    public GameObject endDayMenuPanel;
    public Button restartDayButton;
    public Button nextDayButton;

    private Room targetRoom;
    private AbnormalityController targetAbnormality;
    private Agent targetAgent;

    void Start()
    {
        Time.timeScale = 1f;
        PeBoxNeed = (gameDataMenu.savedCurrentDay * 15);
        UpdateText();

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (endDayButton != null)
            endDayButton.gameObject.SetActive(false);
        if (endDayMenuPanel != null)
            endDayMenuPanel.SetActive(false);

        HideAllButtons();

        if (instinctButton != null)
            instinctButton.onClick.AddListener(() => StartWork("Instinct"));
        if (insightButton != null)
            insightButton.onClick.AddListener(() => StartWork("Insight"));
        if (attachmentButton != null)
            attachmentButton.onClick.AddListener(() => StartWork("Attachment"));
        if (repressionButton != null)
            repressionButton.onClick.AddListener(() => StartWork("Repression"));

        if (endDayButton != null)
            endDayButton.onClick.AddListener(ShowEndMenu);
        if (restartDayButton != null)
            restartDayButton.onClick.AddListener(RestartCurrentDay);
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(GoToNextDay);

        InitializeSpeedButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NavigateSpeedButtons(-1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            NavigateSpeedButtons(1);
        }
    }

    void InitializeSpeedButtons()
    {
        speedButtons = new Button[] { StopButton, DefaultButton, UpSpeedButton, UpUpSpeedButton };
        speedValues = new float[] { 0f, normalSpeed, fastSpeed, veryFastSpeed };

        for (int i = 0; i < speedButtons.Length; i++)
        {
            if (speedButtons[i] != null)
            {
                int index = i;
                speedButtons[i].onClick.AddListener(() => OnSpeedButtonClick(index));
            }
        }

        UpdateButtonColors();
    }

    void NavigateSpeedButtons(int direction)
    {
        if (speedButtons == null || speedButtons.Length == 0) return;

        int newIndex = currentSpeedIndex + direction;

        if (newIndex < 0)
            newIndex = speedButtons.Length - 1;
        if (newIndex >= speedButtons.Length)
            newIndex = 0;

        OnSpeedButtonClick(newIndex);
    }

    void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;

            if (currentSpeedIndex != 0)
            {
                currentSpeedIndex = 0;
                UpdateButtonColors();
            }
            Debug.Log("Пауза (пробел)");
        }
        else
        {
            float resumeSpeed = previousTimeScale > 0 ? previousTimeScale : normalSpeed;
            Time.timeScale = resumeSpeed;

            int speedIndex = 1;
            for (int i = 0; i < speedValues.Length; i++)
            {
                if (Mathf.Approximately(speedValues[i], resumeSpeed))
                {
                    speedIndex = i;
                    break;
                }
            }

            currentSpeedIndex = speedIndex;
            UpdateButtonColors();
            Debug.Log($"Возобновление со скоростью {resumeSpeed}x");
        }
    }

    void OnSpeedButtonClick(int index)
    {
        if (index < 0 || index >= speedValues.Length) return;

        currentSpeedIndex = index;
        float targetSpeed = speedValues[index];

        if (targetSpeed == 0)
        {
            previousTimeScale = Time.timeScale > 0 ? Time.timeScale : previousTimeScale;
            Time.timeScale = 0;
            Debug.Log("Игра на паузе");
        }
        else
        {
            Time.timeScale = targetSpeed;
            previousTimeScale = targetSpeed;
            Debug.Log($"Скорость игры изменена на: {targetSpeed}x");
        }

        UpdateButtonColors();
    }

    public float GetCurrentSpeed()
    {
        return currentSpeedIndex >= 0 && currentSpeedIndex < speedValues.Length ? speedValues[currentSpeedIndex] : 1f;
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < speedButtons.Length; i++)
        {
            if (speedButtons[i] != null)
            {
                ColorBlock colors = speedButtons[i].colors;

                if (i == currentSpeedIndex)
                {
                    colors.normalColor = selectedColor;
                    colors.selectedColor = selectedColor;
                }
                else
                {
                    colors.normalColor = normalColor;
                    colors.selectedColor = normalColor;
                }

                speedButtons[i].colors = colors;
            }
        }
    }

    public void ShowEndButton()
    {
        if (endDayButton != null)
        {
            endDayButton.gameObject.SetActive(true);
            Debug.Log("Кнопка завершения дня показана");
        }
    }

    private void ShowEndMenu()
    {
        Time.timeScale = 0f;
        if (endDayMenuPanel != null)
        {
            endDayMenuPanel.SetActive(true);
            Debug.Log("Меню завершения дня открыто");
        }

        if (interactionPanel != null && interactionPanel.activeSelf)
        {
            interactionPanel.SetActive(false);
        }
    }

    private void RestartCurrentDay()
    {
        Debug.Log("Переигрываем текущий день...");

        PeBoxHave = 0;
        UpdateText();

        if (endDayButton != null)
            endDayButton.gameObject.SetActive(false);
        if (endDayMenuPanel != null)
            endDayMenuPanel.SetActive(false);

        if (gameDataMenu != null)
        {
            gameDataMenu.savedDayState = "CurrentDay";
            Debug.Log($"Сохранено: Day {gameDataMenu.savedCurrentDay}, State {gameDataMenu.savedDayState}");
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(loadedScene);
    }

    private void GoToNextDay()
    {
        Debug.Log("Переход к следующему дню...");

        if (endDayMenuPanel != null)
            endDayMenuPanel.SetActive(false);

        if (gameDataMenu != null)
        {
            gameDataMenu.savedCurrentDay++;
            gameDataMenu.savedDayState = "NewDay";
            gameDataMenu.ForceSave();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameDataMenu);
#endif

            Debug.Log($"Сохранено в GameDataMenu: Day {gameDataMenu.savedCurrentDay}, State {gameDataMenu.savedDayState}");
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(loadedScene);
    }

    void HideAllButtons()
    {
        if (instinctButton != null) instinctButton.gameObject.SetActive(false);
        if (insightButton != null) insightButton.gameObject.SetActive(false);
        if (attachmentButton != null) attachmentButton.gameObject.SetActive(false);
        if (repressionButton != null) repressionButton.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        if (currentOpenPanel != null && currentOpenPanel != this)
        {
            currentOpenPanel.interactionPanel.SetActive(false);
        }
        currentOpenPanel = this;
        interactionPanel.SetActive(true);
    }

    public void SetWorkData(Room room, AbnormalityController abnormality, Agent agent)
    {
        targetRoom = room;
        targetAbnormality = abnormality;
        targetAgent = agent;
        UpdateAbnormalityIcon();
    }

    private void UpdateAbnormalityIcon()
    {
        if (abnormalityDisplayImage != null && targetAbnormality != null)
        {
            Sprite icon = targetAbnormality.GetIcon();
            if (icon != null)
            {
                abnormalityDisplayImage.sprite = icon;
                abnormalityDisplayImage.enabled = true;
                abnormalityDisplayImage.preserveAspect = true;
            }
            else
            {
                abnormalityDisplayImage.enabled = false;
                Debug.LogWarning($"У аномалии {targetAbnormality.name} нет иконки!");
            }
        }
    }

    public void AddPeBox(int amount)
    {
        PeBoxHave += amount;
        if (PeBoxHave > PeBoxNeed)
        {
            PeBoxHave = PeBoxNeed;
        }
        UpdateText();

        if (PeBoxHave >= PeBoxNeed)
        {
            ShowEndButton();
        }
    }

    public void UpdateText()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = $"{PeBoxHave} / {PeBoxNeed}";
        }
    }

    void StartWork(string workType)
    {
        Debug.Log($"StartWork: agent={targetAgent?.agentName}, room={targetRoom?.roomName}, workType={workType}");

        if (targetAgent == null || targetRoom == null || targetAbnormality == null)
        {
            Debug.LogError("Не все данные установлены!");
            ClosePanel();
            return;
        }

        WorkManager.Instance?.AssignWork(targetAgent, targetRoom, targetAbnormality, workType);
        ClosePanel();
    }

    public void ClosePanel()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);
        HideAllButtons();

        if (abnormalityDisplayImage != null)
        {
            abnormalityDisplayImage.sprite = null;
            abnormalityDisplayImage.enabled = false;
        }
        if (currentOpenPanel == this)
            currentOpenPanel = null;
    }
}