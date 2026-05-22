using System.Collections.Generic;
using UnityEngine;

public class WorkManager : MonoBehaviour
{
    public static WorkManager Instance;

    private Dictionary<Agent, WorkTask> activeTasks = new Dictionary<Agent, WorkTask>();
    private Stack<WorkTask> taskPool = new Stack<WorkTask>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AssignWork(Agent agent, Room room, AbnormalityController abnormality, string workType)
    {
        if (agent == null || room == null || abnormality == null)
        {
            Debug.LogError("WorkManager: Не все данные для работы!");
            return;
        }

        if (activeTasks.ContainsKey(agent))
        {
            CancelWork(agent);
        }

        WorkTask task = GetTaskFromPool();
        task.targetRoom = room;
        task.targetAbnormality = abnormality;
        task.workType = workType;
        task.agent = agent;

        activeTasks[agent] = task;

        agent.OnReachedRoom -= OnAgentReachedRoom;
        agent.OnReachedRoom += OnAgentReachedRoom;

        agent.MoveToRoom(room);

        Debug.Log($"WorkManager: {agent.agentName} назначена работа в {room.roomName} ({workType})");
    }

    private void OnAgentReachedRoom(Agent agent, Room reachedRoom)
    {
        if (!activeTasks.TryGetValue(agent, out WorkTask task)) return;

        if (reachedRoom == task.targetRoom)
        {
            Debug.Log($"WorkManager: {agent.agentName} прибыл в {reachedRoom.roomName}, начинаем работу {task.workType}");

            task.targetAbnormality.StartWork(agent, task.workType);

            activeTasks.Remove(agent);
            ReturnTaskToPool(task);

            agent.OnReachedRoom -= OnAgentReachedRoom;
        }
    }

    public void CancelWork(Agent agent)
    {
        if (activeTasks.TryGetValue(agent, out WorkTask task))
        {
            ReturnTaskToPool(task);
            activeTasks.Remove(agent);
            agent.OnReachedRoom -= OnAgentReachedRoom;
            Debug.Log($"WorkManager: Отменена работа для {agent.agentName}");
        }
    }

    private WorkTask GetTaskFromPool()
    {
        if (taskPool.Count > 0)
            return taskPool.Pop();
        return new WorkTask();
    }

    private void ReturnTaskToPool(WorkTask task)
    {
        task.Reset();
        taskPool.Push(task);
    }

    private class WorkTask
    {
        public Room targetRoom;
        public AbnormalityController targetAbnormality;
        public string workType;
        public Agent agent;

        public void Reset()
        {
            targetRoom = null;
            targetAbnormality = null;
            workType = null;
            agent = null;
        }
    }
}