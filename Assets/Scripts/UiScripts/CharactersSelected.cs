using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CharactersSelected : MonoBehaviour
{
    [SerializeField] public Button hire;
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject CharacterStanding;
    [SerializeField] public GameObject Character;
    [SerializeField] public GameObject AnomalyPrefab;
    public GameDataMenu gameDataMenu;
    [SerializeField] public int characterID;
    [Header("Департаменты")]
    [SerializeField] public List<Transform> departments;         
    [SerializeField] public List<Transform> anomalyDepartments;
    [SerializeField] public GameObject plaseholder;
    public RoomsData roomsData;
    private GameObject draggedCharacter;
    private Mouse mouse;
    private List<RectTransform> departmentRects;
    private bool isDragging = false;

    void Start()
    {
        mouse = Mouse.current;
        departmentRects = new List<RectTransform>();

        foreach (var dept in departments)
        {
            if (dept != null)
                departmentRects.Add(dept.GetComponent<RectTransform>());
        }

        if (hire != null)
            hire.onClick.AddListener(ButtonHire);

        LoadData();
    }

    void Update()
    {
        if (!isDragging || draggedCharacter == null || mouse == null) return;

        if (mouse.leftButton.isPressed)
        {
            Vector3 mousePos = mouse.position.ReadValue();
            mousePos.z = 0;
            draggedCharacter.transform.position = mousePos;
        }
        else
        {
            Vector3 finalMousePos = mouse.position.ReadValue();
            finalMousePos.z = 0;
            TryDropCharacter(finalMousePos);
            draggedCharacter = null;
            isDragging = false;
        }
    }
    private void LoadAnomalies()
    {
        if (gameDataMenu == null || gameDataMenu.Department == null) return;

        for (int i = 0; i < gameDataMenu.Department.Count && i < anomalyDepartments.Count; i++)
        {
            if (anomalyDepartments[i] == null) continue;

            // Очищаем старые аномалии в этом департаменте
            foreach (Transform child in anomalyDepartments[i])
                Destroy(child.gameObject);

            foreach (var anomalyData in gameDataMenu.Department[i].Anomaly)
            {
                if (anomalyData == null) continue;

                // Создаём объект аномалии для отображения
                GameObject anomalyObj = CreateAnomalyVisual(anomalyData);
                anomalyObj.transform.SetParent(anomalyDepartments[i]);
                anomalyObj.transform.localScale = Vector3.one;
                anomalyObj.transform.localPosition = Vector3.zero;
            }
        }
    }
    private GameObject CreateAnomalyVisual(GameDataMenu.Abnormality anomalyData)
    {
        GameObject anomalyObj;

        if (AnomalyPrefab != null)
        {
            anomalyObj = Instantiate(AnomalyPrefab);
        }
        else
        {
            anomalyObj = new GameObject(anomalyData.abnormalityName);
            anomalyObj.AddComponent<RectTransform>();
        }

        // Добавляем Image для отображения иконки
        Image image = anomalyObj.GetComponent<Image>();
        if (image == null)
            image = anomalyObj.AddComponent<Image>();

        if (anomalyData.icon != null)
        {
            image.sprite = anomalyData.icon;
        }
        else if (anomalyData.imageAbnormaly != null)
        {
            image.sprite = anomalyData.imageAbnormaly;
        }

        anomalyObj.name = anomalyData.abnormalityName;

        return anomalyObj;
    }

    public void ButtonHire()
    {
        if (Character != null && CharacterStanding != null && gameDataMenu != null)
        {
            GameObject spawnedCharacter = Instantiate(Character);
            spawnedCharacter.transform.SetParent(CharacterStanding.transform);
            spawnedCharacter.transform.localScale = Vector3.one;
            spawnedCharacter.transform.localPosition = Vector3.zero;

            int newId = characterID;
            AddDragHandler(spawnedCharacter, newId);

            GameDataMenu.CharactersData newCharacterData = new GameDataMenu.CharactersData();
            newCharacterData.id = newId;
            newCharacterData.CharacterName = "Персонаж " + newId;

            gameDataMenu.Characters.Add(newCharacterData);
            characterID++;
        }
    }

    private void AddDragHandler(GameObject character, int id)
    {
        CharacterDragHandler dragHandler = character.AddComponent<CharacterDragHandler>();
        dragHandler.Init(this, id);

        Button btn = character.GetComponent<Button>();
        if (btn != null)
            Destroy(btn);
    }

    public void LoadData()
    {
        if (gameDataMenu == null || CharacterStanding == null) return;

        foreach (Transform child in CharacterStanding.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < departments.Count; i++)
        {
            if (departments[i] == null) continue;

            foreach (Transform child in departments[i])
                Destroy(child.gameObject);
        }

        for (int i = 0; i < anomalyDepartments.Count; i++)
        {
            if (anomalyDepartments[i] == null) continue;
            foreach (Transform child in anomalyDepartments[i])
                Destroy(child.gameObject);
        }

        LoadAnomalies();

        foreach (var characterData in gameDataMenu.Characters)
        {
            if (IsCharacterInAnyDepartment(characterData.id)) continue;

            GameObject characterPrefab = characterData.Character != null ? characterData.Character : Character;
            if (characterPrefab == null) continue;

            GameObject loadedCharacter = Instantiate(characterPrefab);
            loadedCharacter.transform.SetParent(CharacterStanding.transform);
            loadedCharacter.transform.localScale = Vector3.one;
            loadedCharacter.transform.localPosition = Vector3.zero;

            AddDragHandler(loadedCharacter, characterData.id);
            characterID = Mathf.Max(characterID, characterData.id + 1);
        }

        if (gameDataMenu.Department == null) return;

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

                AddDragHandler(loadedCharacter, characterData.id);
                characterID = Mathf.Max(characterID, characterData.id + 1);
            }

       
        }

    }

    private bool IsCharacterInAnyDepartment(int id)
    {
        if (gameDataMenu.Department == null) return false;

        foreach (var dept in gameDataMenu.Department)
        {
            if (dept.Characters == null) continue;
            foreach (var character in dept.Characters)
            {
                if (character.id == id) return true;
            }
        }
        return false;
    }

    public void StartDragging(GameObject character)
    {
        draggedCharacter = character;
        draggedCharacter.transform.SetParent(canvas.transform);
        isDragging = true;
    }

    private void TryDropCharacter(Vector3 mousePositionAtDrop)
    {
        int draggedId = GetDraggedCharacterId();

        for (int i = 0; i < departments.Count; i++)
        {
            if (departments[i] == null || departmentRects[i] == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(departmentRects[i], mousePositionAtDrop))
            {
                int currentCharacterCount = GetDepartmentCharacterCount(i);

                if (currentCharacterCount >= 5)
                {
                    Debug.Log($"в департаменте {i} уже 5 персонажей, нельзя добавить больше");
                    ReturnToStanding();
                    return;
                }

                RemoveCharacterFromAllDepartments(draggedId);
                RemoveCharacterFromGeneralList(draggedId);

                draggedCharacter.transform.SetParent(departments[i]);
                draggedCharacter.transform.localPosition = Vector3.zero;
                draggedCharacter.transform.localScale = Vector3.one;

                if (gameDataMenu != null)
                {
                    GameDataMenu.CharactersData newCharacterData = GetCharacterDataById(draggedId);
                    if (newCharacterData == null)
                    {
                        newCharacterData = new GameDataMenu.CharactersData();
                        newCharacterData.id = draggedId;
                        newCharacterData.CharacterName = "Персонаж " + draggedId;
                        newCharacterData.Character = Character;
                    }

                    string roomId = GetRoomIdByDepartmentIndex(i);
                    newCharacterData.startRoomID = roomId;
                    Debug.Log($"Персонажу {newCharacterData.CharacterName} назначена комната {roomId}");

                    if (gameDataMenu.Department == null)
                        gameDataMenu.Department = new List<GameDataMenu.DepartmentData>();

                    while (gameDataMenu.Department.Count <= i)
                        gameDataMenu.Department.Add(new GameDataMenu.DepartmentData());

                    gameDataMenu.Department[i].Characters.Add(newCharacterData);
                }

                return;
            }
        }

        ReturnToStanding();
    }

    private string GetRoomIdByDepartmentIndex(int departmentIndex)
    {
        string departmentName = GetDepartmentName(departmentIndex);

        if (roomsData != null)
        {
            foreach (RoomData roomData in roomsData.rooms)
            {
                if (roomData.department == departmentName && roomData.type == RoomType.MainRoom)
                {
                    return roomData.id;
                }
            }
        }

        if (departmentIndex == 0) return "Main";
        if (departmentIndex == 1) return "Safe";
        if (departmentIndex == 2) return "Training";

        return "Main";
    }

    private string GetDepartmentName(int index)
    {
        string[] deptNames = { "Main", "Training", "Safe" };
        if (index < deptNames.Length)
            return deptNames[index];
        return "Unknown";
    }

    private void ReturnToStanding()
    {
        int draggedId = GetDraggedCharacterId();

        RemoveCharacterFromAllDepartments(draggedId);

        if (!IsCharacterInGeneralList(draggedId))
        {
            GameDataMenu.CharactersData newCharacterData = new GameDataMenu.CharactersData();
            newCharacterData.id = draggedId;
            newCharacterData.CharacterName = "Персонаж " + draggedId;
            newCharacterData.Character = Character;
            gameDataMenu.Characters.Add(newCharacterData);
        }

        draggedCharacter.transform.SetParent(CharacterStanding.transform);
        draggedCharacter.transform.localPosition = Vector3.zero;
        draggedCharacter.transform.localScale = Vector3.one;
    }
    private int GetDepartmentCharacterCount(int departmentIndex)
    {
        if (gameDataMenu?.Department == null || departmentIndex >= gameDataMenu.Department.Count)
            return 0;

        return gameDataMenu.Department[departmentIndex].Characters?.Count ?? 0;
    }
    public void RefreshAnomaliesDisplay()
    {
        LoadAnomalies();  // Просто перезагружаем отображение аномалий
    }
    private int GetDraggedCharacterId()
    {
        CharacterDragHandler handler = draggedCharacter.GetComponent<CharacterDragHandler>();
        if (handler != null)
            return handler.GetId();
        return -1;
    }

    private void RemoveCharacterFromAllDepartments(int characterId)
    {
        if (gameDataMenu?.Department == null) return;

        foreach (var department in gameDataMenu.Department)
        {
            if (department?.Characters == null) continue;

            for (int i = department.Characters.Count - 1; i >= 0; i--)
            {
                if (department.Characters[i].id == characterId)
                {
                    department.Characters.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void RemoveCharacterFromGeneralList(int characterId)
    {
        if (gameDataMenu?.Characters == null) return;

        for (int i = gameDataMenu.Characters.Count - 1; i >= 0; i--)
        {
            if (gameDataMenu.Characters[i].id == characterId)
            {
                gameDataMenu.Characters.RemoveAt(i);
                break;
            }
        }
    }

    private bool IsCharacterInGeneralList(int characterId)
    {
        if (gameDataMenu?.Characters == null) return false;

        foreach (var character in gameDataMenu.Characters)
        {
            if (character.id == characterId) return true;
        }
        return false;
    }

    private GameDataMenu.CharactersData GetCharacterDataById(int id)
    {
        foreach (var dept in gameDataMenu.Department)
        {
            if (dept.Characters == null) continue;
            foreach (var character in dept.Characters)
            {
                if (character.id == id) return character;
            }
        }
        return null;
    }
}

public class CharacterDragHandler : MonoBehaviour, IPointerDownHandler
{
    private CharactersSelected parent;
    private int id;

    public void Init(CharactersSelected parent, int id)
    {
        this.parent = parent;
        this.id = id;
    }

    public int GetId()
    {
        return id;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        parent?.StartDragging(gameObject);
    }
}