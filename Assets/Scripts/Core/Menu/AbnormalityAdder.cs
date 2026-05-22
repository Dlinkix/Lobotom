using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbnormalityAdder : MonoBehaviour
{
    [Header("Источник аномалий")]
    public AbnormalityDatabase abnormalityDatabase;

    [Header("Куда добавлять")]
    public GameDataMenu gameDataMenu;

    public CharactersSelected charactersSelected;

    [Header("Настройки")]
    public int maxAbnormalitiesPerDepartment = 4;

    [Header("UI")]
    public TMP_Text resultText;
    public Button addButton;
    public TextMeshProUGUI text;
    public GameObject canvas;

    void Start()
    {
        if (addButton != null)
            addButton.onClick.AddListener(AddRandomAbnormalityToDepartment);

        if (charactersSelected == null)
        {
            charactersSelected = FindAnyObjectByType<CharactersSelected>();
        }
    }

    public void AddRandomAbnormalityToDepartment()
    {
        if (abnormalityDatabase == null)
        {
            Debug.LogError("AbnormalityDatabase не назначен!");
            return;
        }

        if (gameDataMenu == null)
        {
            Debug.LogError("GameDataMenu не назначен!");
            return;
        }

        if (gameDataMenu.Department.Count == 0)
        {
            Debug.LogError("Нет департаментов в GameDataMenu!");
            return;
        }

        GameDataMenu.DepartmentData targetDepartment = null;
        int departmentIndex = -1;

        for (int i = 0; i < gameDataMenu.Department.Count; i++)
        {
            var department = gameDataMenu.Department[i];
            if (department.Anomaly.Count < maxAbnormalitiesPerDepartment)
            {
                targetDepartment = department;
                departmentIndex = i;
                break;
            }
        }

        if (targetDepartment == null)
        {
            string message = $"Все департаменты заполнены! (максимум {maxAbnormalitiesPerDepartment} аномалий)";
            Debug.LogWarning(message);
            if (resultText != null)
                resultText.text = message;
            return;
        }

        AbnormalityData randomData = GetRandomNewAbnormalityGlobal();
        if (randomData == null)
        {
            string message = "В базе нет новых аномалий! Все возможные аномалии уже добавлены в департаменты.";
            Debug.LogWarning(message);
            if (resultText != null)
                resultText.text = message;
            return;
        }

        GameDataMenu.Abnormality newAbnormality = ConvertToGameDataAbnormality(randomData);

        int anomalyNumber = targetDepartment.Anomaly.Count + 1;
        string generatedRoomID = GenerateRoomID(targetDepartment.depatmentName, anomalyNumber);
        newAbnormality.startRoomID = generatedRoomID;

        Debug.Log($"Сгенерирован startRoomID для {newAbnormality.abnormalityName}: {generatedRoomID}");

        targetDepartment.Anomaly.Add(newAbnormality);

        if (charactersSelected != null)
        {
            charactersSelected.RefreshAnomaliesDisplay();
        }

        string successMessage = $"Добавлена аномалия: {randomData.abnormalityName} в департамент {targetDepartment.depatmentName} (теперь аномалий: {targetDepartment.Anomaly.Count}/{maxAbnormalitiesPerDepartment}) | RoomID: {generatedRoomID}";
        Debug.Log(successMessage);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentState = GameManager.DayState.CurrentDay;
            GameManager.Instance.SaveCurrentState();
        }

        if (resultText != null)
        {
            resultText.text = successMessage;
        }
        canvas.SetActive(false);
    }

    private string GenerateRoomID(string departmentName, int anomalyNumber)
    {
        string cleanDepartmentName = departmentName.Replace(" ", "");
        return $"{cleanDepartmentName}_AnomalyCell_{anomalyNumber}";
    }

    private AbnormalityData GetRandomNewAbnormalityGlobal()
    {
        HashSet<int> allExistingIds = new HashSet<int>();

        foreach (var department in gameDataMenu.Department)
        {
            foreach (var anomaly in department.Anomaly)
            {
                allExistingIds.Add(anomaly.id);
            }
        }

        List<AbnormalityData> availableAbnormalities = new List<AbnormalityData>();
        foreach (var abnormality in abnormalityDatabase.allAbnormalities)
        {
            if (!allExistingIds.Contains(abnormality.id))
            {
                availableAbnormalities.Add(abnormality);
            }
        }

        if (availableAbnormalities.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, availableAbnormalities.Count);
        return availableAbnormalities[randomIndex];
    }

    private GameDataMenu.Abnormality ConvertToGameDataAbnormality(AbnormalityData data)
    {
        return new GameDataMenu.Abnormality
        {
            abnormalityName = data.abnormalityName,
            id = data.id,
            startRoomID = data.startRoomID,
            prefab = data.prefab,
            width = data.width,
            height = data.height,
            icon = data.icon,
            imageAbnormaly = data.imageAbnormaly,
            lvl = data.lvl,
            peBoxReward = data.peBoxReward,
            danger = data.danger,
            instinctWork = ConvertToWorkType(data.instinctWork),
            insightWork = ConvertToWorkType(data.insightWork),
            attachmentWork = ConvertToWorkType(data.attachmentWork),
            repressionWork = ConvertToWorkType(data.repressionWork),
            canEscape = data.canEscape,
            qliphothCounter = data.qliphothCounter,
            egoWeapon = data.egoWeapon,
            egoSuit = data.egoSuit,
            egoGift = data.egoGift,
            Discription = data.Discription
        };
    }

    private GameDataMenu.WorkType ConvertToWorkType(WorkTypeData data)
    {
        return new GameDataMenu.WorkType
        {
            name = data.name,
            level1SuccessChance = data.level1SuccessChance,
            level2SuccessChance = data.level2SuccessChance,
            level3SuccessChance = data.level3SuccessChance,
            level4SuccessChance = data.level4SuccessChance,
            level5SuccessChance = data.level5SuccessChance
        };
    }
}