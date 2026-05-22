using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/Rooms Data")]
public class RoomsData : ScriptableObject
{
    [Header("Все комнаты")]
    public List<RoomData> rooms = new List<RoomData>();
}

[Serializable]
public class RoomData
{
    [Header("Идентификация")]
    public string id = "room_01";
    public string name = "Новая комната";
    public string department = "Information";
    public int floor = 0;


    [Header("Тип комнаты")]
    public RoomType type = RoomType.MainRoom;

    [Header("Связи (ID других комнат)")]
    public List<string> connections = new List<string>();

    [Header("Позиция (опционально)")]
    public Vector2 position = Vector2.zero;
}

public enum RoomType
{
    MainRoom,
    Corridor,
    Elevator,
    AbnormalityCell
}