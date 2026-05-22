using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour, IPointerClickHandler
{
    public enum State
    {
        Idle,
        Moving,
        Working,
        Fighting,
        Die
    }

    [Header("Характеристики")]
    public string agentName = "Агент";
    public float speed = 2f;
    public Room currentRoom;
    public Room StayRoom;
    public State currentState = State.Idle;

    [Header("AI Settings")]
    public bool enableAI = true;
    public float idleMoveDelay = 3f;
    private float idleMoveTimer = 0f;
    private Vector3 targetMovePosition;
    private bool isMovingInsideRoom = false;

    [Header("Визуал")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public Color workingColor = Color.gray;
    public Color fightColor = Color.red;
    public Color dieColor = Color.black;

    public event Action<Agent, Room> OnReachedRoom;

    [Header("Данные из GameDataMenu")]
    public GameDataMenu.CharactersData characterData;

    private RectTransform rectTransform;
    public List<Room> currentPath;
    private int currentPathIndex;
    private bool isSelected = false;
    private Image agentImage;
    private Color originalColor;
    private AbnormalityController currentWorkAbnormality;
    private State previousState;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        agentImage = GetComponent<Image>();

        if (agentImage != null)
        {
            originalColor = agentImage.color;
        }

        AgentManager.Instance?.RegisterAgent(this);

        previousState = currentState;

        if (currentRoom != null)
        {
            Vector3 targetPos = GetRoomBottomCenter(currentRoom);
            rectTransform.position = targetPos;
            StayRoom = currentRoom;
        }
        GetSpeed();
    }

    void GetSpeed()
    {
        speed += characterData.justice / 100f;
    }

    private void SetState(State newState)
    {
        if (currentState == newState) return;

        State oldState = currentState;
        currentState = newState;

        AgentManager.Instance?.UpdateAgentState(this, newState);

        UpdateVisual();

        if (newState != State.Moving)
        {
            isMovingInsideRoom = false;
        }
    }

    public void UpdateVisual()
    {
        if (agentImage == null) return;

        if (currentState == State.Working)
            agentImage.color = workingColor;
        else if (currentState == State.Fighting)
            agentImage.color = fightColor;
        else if (currentState == State.Die)
            agentImage.color = dieColor;
        else if (isSelected)
            agentImage.color = selectedColor;
        else
            agentImage.color = originalColor;
    }

    public void StartWorking()
    {
        if (currentState == State.Idle)
        {
            SetState(State.Working);
            Debug.Log($"{agentName} начал работать");
        }
    }

    public void StopWorking()
    {
        if (currentState == State.Working)
        {
            SetState(State.Idle);
            Debug.Log($"{agentName} закончил работать");
        }
    }

    public void UpdateAI(float deltaTime)
    {
        if (!enableAI) return;
        if (currentState != State.Idle) return;
        if (isMovingInsideRoom) return;

        idleMoveTimer += deltaTime;

        if (idleMoveTimer >= idleMoveDelay)
        {
            if (Random.Range(0f, 1f) < 0.7f)
            {
                MoveToRandomPointInCurrentRoom();
            }
            idleMoveTimer = 0f;
        }
    }

    public void MoveToRandomPointInCurrentRoom()
    {
        if (currentRoom == null) return;

        targetMovePosition = GetRandomPointInRoom(currentRoom);
        isMovingInsideRoom = true;
        SetState(State.Moving);

        Debug.Log($"{agentName} бродит по комнате {currentRoom.roomName}");
    }

    private Vector3 GetRandomPointInRoom(Room room)
    {
        RectTransform roomRect = room.GetComponent<RectTransform>();

        float minX = roomRect.rect.xMin;
        float maxX = roomRect.rect.xMax;
        float minY = roomRect.rect.yMin;
        float maxY = roomRect.rect.yMax;

        float xOffset = roomRect.rect.width * 0.1f;
        float yOffset = roomRect.rect.height * 0.1f;

        float randomX = Random.Range(minX + xOffset, maxX - xOffset);
        float randomY = minY + 1;

        Vector2 localPoint = new Vector2(randomX, randomY);
        return roomRect.TransformPoint(localPoint);
    }

    public void SetCurrentWork(AbnormalityController abnormality)
    {
        currentWorkAbnormality = abnormality;
    }

    public void ClearCurrentWork()
    {
        currentWorkAbnormality = null;
    }

    public void MoveAlongPath()
    {
        if (isMovingInsideRoom && currentState == State.Moving)
        {
            rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetMovePosition, speed * Time.deltaTime);

            if (Vector3.Distance(rectTransform.position, targetMovePosition) < 0.05f)
            {
                isMovingInsideRoom = false;
                SetState(State.Idle);
                Debug.Log($"{agentName} закончил бродить по комнате");
            }
            return;
        }

        if (currentState != State.Moving) return;

        if (currentPath == null || currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            SetState(State.Idle);
            return;
        }

        Room targetRoom = currentPath[currentPathIndex];
        if (targetRoom == null)
        {
            SetState(State.Idle);
            return;
        }

        Vector3 targetPos = GetRoomBottomCenter(targetRoom);
        rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(rectTransform.position, targetPos) < 0.05f)
        {
            currentRoom = targetRoom;
            currentPathIndex++;
            Debug.Log($"{agentName} прибыл в {currentRoom.roomName}");

            if (currentPathIndex >= currentPath.Count)
            {
                SetState(State.Idle);
                Debug.Log($"{agentName} достиг цели!");
                OnReachedRoom?.Invoke(this, currentRoom);

                if (currentRoom.roomType != RoomType.AbnormalityCell)
                {
                    StayRoom = currentRoom;
                }
            }
        }
    }

    private Vector3 GetRoomBottomCenter(Room room)
    {
        RectTransform roomRect = room.GetComponent<RectTransform>();

        float width = roomRect.rect.width;
        float height = roomRect.rect.height;

        Vector2 pivotOffset = roomRect.pivot;

        float offsetX = (0.5f - pivotOffset.x) * width;
        float offsetY = (0f - pivotOffset.y) * height;

        Vector2 localPoint = new Vector2(offsetX, offsetY + 1);

        return roomRect.TransformPoint(localPoint);
    }

    public void MoveToRoom(Room destination)
    {
        if (currentState != State.Idle && currentState != State.Moving)
        {
            Debug.Log($"{agentName} не может двигаться (статус: {currentState})");
            return;
        }

        if (currentRoom == null)
        {
            Debug.LogWarning($"{agentName}: currentRoom не назначен!");
            return;
        }

        if (isMovingInsideRoom)
        {
            isMovingInsideRoom = false;
        }

        List<Room> path = FindPath(currentRoom, destination);

        if (path != null && path.Count > 0)
        {
            currentPath = path;
            currentPathIndex = 0;
            SetState(State.Moving);
            string pathStr = string.Join(" -> ", path.ConvertAll(r => r.roomName));
            Debug.Log($"{agentName} идёт: {pathStr}");
            AgentManager.Instance?.ClearSelected();
        }
        else
        {
            Debug.LogWarning($"{agentName}: нет пути от {currentRoom.roomName} до {destination.roomName}");
        }
    }

    private List<Room> FindPath(Room start, Room end)
    {
        if (start == end) return new List<Room> { start };

        Queue<List<Room>> paths = new Queue<List<Room>>();
        paths.Enqueue(new List<Room> { start });

        HashSet<Room> visited = new HashSet<Room>();
        visited.Add(start);

        while (paths.Count > 0)
        {
            List<Room> currentPath = paths.Dequeue();
            Room lastRoom = currentPath[currentPath.Count - 1];

            foreach (Room nextRoom in lastRoom.connectedRooms)
            {
                if (nextRoom == end)
                {
                    List<Room> completePath = new List<Room>(currentPath);
                    completePath.Add(nextRoom);
                    return completePath;
                }

                if (!visited.Contains(nextRoom))
                {
                    visited.Add(nextRoom);
                    List<Room> newPath = new List<Room>(currentPath);
                    newPath.Add(nextRoom);
                    paths.Enqueue(newPath);
                }
            }
        }

        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentState == State.Working || currentState == State.Die)
        {
            Debug.Log($"{agentName} сейчас {currentState}, его нельзя выбрать");
            return;
        }

        AgentManager.Instance?.SelectAgent(this);
    }

    public void Select()
    {
        isSelected = true;
        UpdateVisual();
        Debug.Log($"{agentName} выбран");
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateVisual();
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}