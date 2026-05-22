using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [Header("Перетащи сюда RoomsData")]
    public RoomsData roomsData;

    void Start()
    {
        if (roomsData == null)
        {
            Debug.LogError("RoomsData не назначен!");
            return;
        }

        LoadConnections();
    }

    void LoadConnections()
    {
        Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        Dictionary<string, Room> roomDict = new Dictionary<string, Room>();

        foreach (Room room in allRooms)
        {
            if (!string.IsNullOrEmpty(room.roomID))
            {
                roomDict[room.roomID] = room;
                Debug.Log($"Найдена комната: {room.roomID} - {room.roomName}");
            }
        }

        foreach (RoomData data in roomsData.rooms)
        {
            if (!roomDict.TryGetValue(data.id, out Room room))
            {
                Debug.LogError($"Комната {data.id} не найдена на сцене!");
                continue;
            }

            room.connectedRooms = new List<Room>();

            foreach (string connectedID in data.connections)
            {
                if (roomDict.TryGetValue(connectedID, out Room connectedRoom))
                {
                    room.connectedRooms.Add(connectedRoom);
                    Debug.Log($"Связь: {room.roomName}  {connectedRoom.roomName}");
                }
                else
                {
                    Debug.LogWarning($"Комната {data.id} ссылается на {connectedID}, но она не найдена");
                }
            }
        }

        Debug.Log("Связи загружены!");
    }
}