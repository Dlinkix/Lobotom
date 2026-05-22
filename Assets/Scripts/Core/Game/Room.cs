using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour, IPointerClickHandler
{
    public string roomID;
    public string roomName;
    public Transform waypoint;
    public List<Room> connectedRooms = new List<Room>();

    [Header("Для клеток с аномалией")]
    public RoomType roomType;
    public AbnormalityController abnormality;
    public ClickHandler clickHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        Agent selectedAgent = AgentManager.Instance?.GetSelectedAgent();

        if (selectedAgent == null)
        {
            Debug.Log("Сначала выбери агента (кликни по нему)");
            return;
        }

        if (roomType == RoomType.AbnormalityCell && abnormality != null)
        {
            if (clickHandler != null && clickHandler.interactionPanel != null)
            {
                clickHandler.SetWorkData(this, abnormality, selectedAgent);
                clickHandler.OpenPanel();
                Debug.Log($"Открыта панель для комнаты {roomName} с аномалией {abnormality.abnormalityData.abnormalityName}");
            }
        }
        else if (roomType == RoomType.AbnormalityCell && abnormality == null)
        {
            Destroy(this);
        }
        else
        {
            CloseInteractionPanel();
            selectedAgent.MoveToRoom(this);
        }
    }

    private void CloseInteractionPanel()
    {
        if (ClickHandler.currentOpenPanel != null)
        {
            ClickHandler.currentOpenPanel.ClosePanel();
        }
    }
}