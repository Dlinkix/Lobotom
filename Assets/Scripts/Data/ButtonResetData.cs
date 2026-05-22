using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResetGameData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Целевые данные")]
    public GameDataMenu gameDataMenu;

    [Header("Переход после очистки")]
    public int targetSceneNumber = 0;

    [Header("UI меню подтверждения")]
    public GameObject confirmationMenu;     
    public Button confirmButton;               
    public Button cancelButton;             

    [Header("Визуальные эффекты")]
    [Range(0f, 1f)][SerializeField] private float hoverAlpha = 0.7f;

    [Header("UI оповещение")]
    public TMPro.TMP_Text notificationText;
    public float notificationDuration = 2f;

    private Image buttonImage;
    private float normalAlpha;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OpenConfirmationMenu);

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            normalAlpha = buttonImage.color.a;

        // Скрываем меню подтверждения при старте
        if (confirmationMenu != null)
            confirmationMenu.SetActive(false);

        // Подписываемся на кнопки
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmReset);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelReset);
    }

    public void OpenConfirmationMenu()
    {
        if (confirmationMenu != null)
        {
            confirmationMenu.SetActive(true);
            ShowNotification("Вы уверены?", Color.yellow);
        }
        else
        {
 
            ShowNotification("Нажмите ещё раз для подтверждения!", Color.yellow);
        }
    }

    public void ConfirmReset()
    {
        Debug.Log("Подтверждено! Очищаем данные...");

        ResetAllData();

        if (confirmationMenu != null)
            confirmationMenu.SetActive(false);

        LoadTargetScene();
    }

    public void CancelReset()
    {
        Debug.Log("Отмена! Данные не изменены.");
        ShowNotification("Операция отменена", Color.white);

        // Закрываем меню
        if (confirmationMenu != null)
            confirmationMenu.SetActive(false);
    }

    private void ResetAllData()
    {
        if (gameDataMenu == null)
        {
            Debug.LogError("GameDataMenu не назначен в инспекторе!");
            ShowNotification("Ошибка: GameDataMenu не найден!", Color.red);
            return;
        }


        gameDataMenu.Characters.Clear();


        foreach (var department in gameDataMenu.Department)
        {
            if (department != null)
            {
                department.Characters.Clear();
                department.Anomaly.Clear();
            }
        }

        gameDataMenu.savedCurrentDay = 1;
        gameDataMenu.savedDayState = "NewDay";

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentDay = 1;
            GameManager.Instance.currentState = GameManager.DayState.NewDay; 
            GameManager.Instance.UpdateDayUI();
        }

        Debug.Log("Все данные сброшены: персонажи и аномалии удалены из всех департаментов!");
        ShowNotification("Данные сброшены! Переход...", Color.green);
    }

    private void LoadTargetScene()
    {
        if (targetSceneNumber < 0 || targetSceneNumber >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Сцена с номером {targetSceneNumber} не найдена в Build Settings!");
            ShowNotification($"Ошибка: сцена {targetSceneNumber} не найдена!", Color.red);
            return;
        }

        SceneManager.LoadScene(targetSceneNumber);
    }

    private void ShowNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = color;
            CancelInvoke(nameof(ClearNotification));
            Invoke(nameof(ClearNotification), notificationDuration);
        }
        else
        {
            Debug.Log(message);
        }
    }

    private void ClearNotification()
    {
        if (notificationText != null)
        {
            notificationText.text = "";
        }
    }

    // Эффекты наведения
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            SetAlpha(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            SetAlpha(normalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = buttonImage.color;
        color.a = alpha;
        buttonImage.color = color;
    }
}