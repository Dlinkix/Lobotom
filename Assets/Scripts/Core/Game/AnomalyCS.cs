using UnityEngine;

public class AbnormalityController : MonoBehaviour
{
    [Header("Данные из GameDataMenu")]
    public GameDataMenu.Abnormality abnormalityData;

    [Header("Состояние")]
    public bool isWorking = false;
    public Agent workingAgent;
    public float workProgress = 0f;
    private int workProgressPeBox = 0;
    public Sprite AbnormalityIcon => abnormalityData?.icon;

    public float workDurationPeBox = 2f;
    private int workGetPeBox = 0;
    private float successChance;
    private GameDataMenu.WorkType selectedWork;
    public ClickHandler clickHandler;

    void Start()
    {
        isWorking = false;
        workingAgent = null;
        workProgress = 0f;
        workProgressPeBox = 0;
        workGetPeBox = 0;
        if (clickHandler == null)
        {
            clickHandler = FindFirstObjectByType<ClickHandler>();
        }
    }

    public Sprite GetIcon()
    {
        return abnormalityData?.icon;
    }

    public bool IsAgentWorking(Agent agent)
    {
        return isWorking && workingAgent == agent;
    }

    public void StartWork(Agent agent, string workType)
    {
        if (isWorking)
        {
            Debug.Log("Уже идет работа");
            return;
        }

        GameDataMenu.CharactersData agentData = agent.characterData;

        if (agentData == null)
        {
            Debug.LogError($"У агента {agent.agentName} нет данных!");
            return;
        }

        int agentLevel = 1;
        selectedWork = null;

        switch (workType)
        {
            case "Instinct":
                agentLevel = agentData.braveryLevel;
                selectedWork = abnormalityData.instinctWork;
                break;
            case "Insight":
                agentLevel = agentData.sapienceLevel;
                selectedWork = abnormalityData.insightWork;
                break;
            case "Attachment":
                agentLevel = agentData.excerptLevel;
                selectedWork = abnormalityData.attachmentWork;
                break;
            case "Repression":
                agentLevel = agentData.justiceLevel;
                selectedWork = abnormalityData.repressionWork;
                break;
        }

        if (selectedWork == null)
        {
            Debug.LogError($"Неизвестный тип работы: {workType}");
            return;
        }

        successChance = selectedWork.GetSuccessChance(agentLevel);

        if (successChance > 0.95f)
        {
            successChance = 0.95f;
        }

        Debug.Log($"{agent.agentName} начал работу {workType}");
        Debug.Log($"Уровень {workType}: {agentLevel}, Шанс успеха: {successChance * 100}%");

        workingAgent = agent;
        isWorking = true;
        workGetPeBox = 0;
        workProgress = 0f;
        agent.StartWorking();
    }

    void AddPeBox()
    {
        float roll = Random.Range(0f, 1f);

        if (roll > successChance)
        {
            Debug.Log($"Провал! еквафалин не получен!");
        }
        else
        {
            workGetPeBox++;
            Debug.Log($"Получен PE-Box! Всего: {workGetPeBox}/{abnormalityData.peBoxReward}");
        }
    }

    void Update()
    {
        if (isWorking && workingAgent != null)
        {
            workProgress += Time.deltaTime;

            if (workProgress >= workDurationPeBox)
            {
                workProgressPeBox++;
                if (workProgressPeBox > abnormalityData.peBoxReward)
                {
                    CompleteWork();
                    workProgress = 0f;
                }
                else
                {
                    AddPeBox();
                    workProgress = 0f;
                }
            }
        }
    }

    void CompleteWork()
    {
        int reward = workGetPeBox;
        if (clickHandler != null)
        {
            clickHandler.AddPeBox(reward);
        }

        Debug.Log($"Работа над {abnormalityData.abnormalityName} завершена! Получено {reward} PE-Box");

        if (workingAgent != null)
        {
            workingAgent.StopWorking();

            if (workingAgent.StayRoom != null)
            {
                Debug.Log($"{workingAgent.agentName} возвращается в {workingAgent.StayRoom.roomName}");
                workingAgent.MoveToRoom(workingAgent.StayRoom);
            }
            else
            {
                Debug.LogWarning($"{workingAgent.agentName}: нет сохранённой комнаты StayRoom!");
            }
        }

        isWorking = false;
        workingAgent = null;
        workProgress = 0f;
        workProgressPeBox = 0;
        workGetPeBox = 0;
    }
}