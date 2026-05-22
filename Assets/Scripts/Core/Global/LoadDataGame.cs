using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataGame : MonoBehaviour
{
    public GameDataMenu gameDataMenu;
    public RoomsData roomsData;
    [SerializeField] public GameObject Character;
    [SerializeField] public List<Transform> departments;
    [SerializeField] public int characterID;

    void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        if (gameDataMenu == null || gameDataMenu.Department == null) return;

        ClearAllDepartments();

        Dictionary<string, Room> roomDict = BuildRoomDictionary();

        LoadAllCharacters(roomDict);
        LoadAllAbnormalities(roomDict);
    }

    private void LoadAllCharacters(Dictionary<string, Room> roomDict)
    {
        for (int i = 0; i < gameDataMenu.Department.Count && i < departments.Count; i++)
        {
            if (departments[i] == null) continue;

            foreach (var characterData in gameDataMenu.Department[i].Characters)
            {
                if (characterData == null) continue;

                GameObject characterPrefab = characterData.Character != null ? characterData.Character : Character;
                if (characterPrefab == null) continue;

                GameObject loadedCharacter = Instantiate(characterPrefab);
                loadedCharacter.transform.SetParent(departments[i]);
                loadedCharacter.transform.localScale = Vector3.one;
                loadedCharacter.transform.localPosition = Vector3.zero;

                loadedCharacter.name = string.Format("Character {0}", characterData.id);

                Agent agent = loadedCharacter.GetComponent<Agent>();
                if (agent == null)
                    agent = loadedCharacter.AddComponent<Agent>();

                agent.agentName = characterData.CharacterName;
                agent.speed = 2f;
                agent.characterData = characterData;
                agent.characterData.CalculateLevels();

                Room startRoom = FindStartRoom(characterData, roomDict, i);
                if (startRoom != null)
                {
                    agent.currentRoom = startRoom;

                    RectTransform rectTransform = loadedCharacter.GetComponent<RectTransform>();
                    if (rectTransform != null && startRoom.waypoint != null)
                    {
                        rectTransform.position = startRoom.waypoint.position;
                    }
                    Debug.Log(string.Format("{0} -> {1}", characterData.CharacterName, startRoom.roomName));
                }
                else
                {
                    Debug.LogWarning(string.Format("Не найдена комната для {0}", characterData.CharacterName));
                }

                AgentManager.Instance?.RegisterAgent(agent);
                characterID = Mathf.Max(characterID, characterData.id + 1);
            }
        }
    }

    private void LoadAllAbnormalities(Dictionary<string, Room> roomDict)
    {
        for (int i = 0; i < gameDataMenu.Department.Count; i++)
        {
            var department = gameDataMenu.Department[i];
            if (department == null) continue;

            foreach (var abnormalityData in department.Anomaly)
            {
                if (abnormalityData == null) continue;
                if (abnormalityData.prefab == null)
                {
                    Debug.LogWarning($"У аномалии {abnormalityData.abnormalityName} нет префаба!");
                    continue;
                }

                Room targetRoom = FindRoomForAbnormality(abnormalityData, roomDict, i);

                if (targetRoom == null)
                {
                    Debug.LogWarning($"Не найдена комната для аномалии {abnormalityData.abnormalityName}");
                    continue;
                }

                GameObject abnormalityObject = Instantiate(abnormalityData.prefab, targetRoom.transform);
                RectTransform roomRect = targetRoom.GetComponent<RectTransform>();

                if (abnormalityData.imageAbnormaly != null)
                {
                    Image targetImage = abnormalityObject.GetComponent<Image>();
                    if (targetImage == null)
                        targetImage = abnormalityObject.AddComponent<Image>();

                    targetImage.sprite = abnormalityData.imageAbnormaly;
                }

                float roomWidth = roomRect.rect.width;
                float roomHeight = roomRect.rect.height;
                Vector2 pivotOffset = roomRect.pivot;
                float leftX = (-pivotOffset.x * roomWidth) + (abnormalityData.width / 1.5f);
                float centerY = (-pivotOffset.y * roomHeight) + (abnormalityData.height / 1.75f);
                abnormalityObject.transform.localPosition = new Vector3(leftX, centerY, 0);
                abnormalityObject.transform.localScale = Vector3.one;
                ApplyObjectSize(abnormalityObject, abnormalityData.width, abnormalityData.height);

                abnormalityObject.name = $"Abnormality_{abnormalityData.abnormalityName}";

                AbnormalityController controller = abnormalityObject.GetComponent<AbnormalityController>();
                if (controller == null)
                    controller = abnormalityObject.AddComponent<AbnormalityController>();

                controller.abnormalityData = abnormalityData;

                targetRoom.roomType = RoomType.AbnormalityCell;
                targetRoom.abnormality = controller;

                if (targetRoom.clickHandler == null)
                {
                    targetRoom.clickHandler = FindFirstObjectByType<ClickHandler>();
                }

                Debug.Log($"Аномалия {abnormalityData.abnormalityName} загружена в комнату {targetRoom.roomName}");
            }
        }
    }

    private void ApplyObjectSize(GameObject obj, float width, float height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.Log($"Размеры не заданы (width={width}, height={height}), оставляем исходный размер");
            return;
        }

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
            Debug.Log($"Установлен размер UI объекта: {width} x {height}");
            return;
        }

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Vector2 originalSize = spriteRenderer.sprite.bounds.size;
            float scaleX = width / originalSize.x;
            float scaleY = height / originalSize.y;
            obj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            Debug.Log($"Установлен масштаб 3D объекта: {scaleX} x {scaleY}");
            return;
        }

        obj.transform.localScale = new Vector3(width, height, 1f);
        Debug.Log($"Установлен localScale: {width} x {height}");
    }

    private Dictionary<string, Room> BuildRoomDictionary()
    {
        Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        Dictionary<string, Room> dict = new Dictionary<string, Room>();
        foreach (Room room in allRooms)
        {
            if (!string.IsNullOrEmpty(room.roomID))
            {
                dict[room.roomID] = room;
            }
        }
        return dict;
    }

    private Room FindStartRoom(GameDataMenu.CharactersData characterData, Dictionary<string, Room> roomDict, int departmentIndex)
    {
        if (!string.IsNullOrEmpty(characterData.startRoomID))
        {
            if (roomDict.TryGetValue(characterData.startRoomID, out Room room))
            {
                return room;
            }
        }

        if (roomsData != null)
        {
            string targetDepartment = GetDepartmentName(departmentIndex);

            foreach (RoomData roomData in roomsData.rooms)
            {
                if (roomData.department == targetDepartment && roomData.type == RoomType.MainRoom)
                {
                    if (roomDict.TryGetValue(roomData.id, out Room mainRoom))
                    {
                        return mainRoom;
                    }
                }
            }
        }

        return null;
    }

    private Room FindRoomForAbnormality(GameDataMenu.Abnormality abnormalityData, Dictionary<string, Room> roomDict, int departmentIndex)
    {
        if (!string.IsNullOrEmpty(abnormalityData.startRoomID))
        {
            if (roomDict.TryGetValue(abnormalityData.startRoomID, out Room room))
            {
                return room;
            }
        }

        string targetDepartment = GetDepartmentName(departmentIndex);

        foreach (var roomData in roomsData.rooms)
        {
            if (roomData.department == targetDepartment && roomData.type == RoomType.AbnormalityCell)
            {
                if (roomDict.TryGetValue(roomData.id, out Room abnormalityRoom))
                {
                    if (abnormalityRoom.abnormality == null)
                    {
                        return abnormalityRoom;
                    }
                }
            }
        }

        return null;
    }

    private string GetDepartmentName(int index)
    {
        string[] deptNames = { "Information", "Safety", "Central" };
        if (index < deptNames.Length)
            return deptNames[index];
        return "Unknown";
    }

    private void ClearAllDepartments()
    {
        foreach (var department in departments)
        {
            if (department == null) continue;
            foreach (Transform child in department)
            {
                Destroy(child.gameObject);
            }
        }
    }
}