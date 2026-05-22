using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; }
    private List<Agent> allAgents = new List<Agent>();
    private List<Agent> movingAgents = new List<Agent>();
    private List<Agent> idleAgents = new List<Agent>();
    private Agent currentSelectedAgent;
    private float aiUpdateTimer = 0f;
    private float aiUpdateInterval = 0.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        for (int i = movingAgents.Count - 1; i >= 0; i--)
        {
            Agent agent = movingAgents[i];
            if (agent != null)
            {
                agent.MoveAlongPath();
            }
            else
            {
                movingAgents.RemoveAt(i);
            }
        }

        if (currentSelectedAgent != null)
        {
            currentSelectedAgent.UpdateVisual();
        }

        aiUpdateTimer += Time.deltaTime;
        if (aiUpdateTimer >= aiUpdateInterval)
        {
            aiUpdateTimer = 0f;
            UpdateIdleAgentsAI();
        }
    }

    private void UpdateIdleAgentsAI()
    {
        float deltaTime = aiUpdateInterval;

        for (int i = 0; i < idleAgents.Count; i++)
        {
            Agent agent = idleAgents[i];
            if (agent != null && agent.enableAI)
            {
                agent.UpdateAI(deltaTime);
            }
        }
    }

    public void SelectAgent(Agent agent)
    {
        if (currentSelectedAgent != null)
            currentSelectedAgent.Deselect();

        currentSelectedAgent = agent;
        currentSelectedAgent.Select();
        Debug.Log($"Выбран: {agent.agentName}");
    }

    public Agent GetSelectedAgent()
    {
        return currentSelectedAgent;
    }

    public void RegisterAgent(Agent agent)
    {
        if (!allAgents.Contains(agent))
        {
            allAgents.Add(agent);
            AddToStateList(agent);
            Debug.Log($"Агент {agent.agentName} зарегистрирован");
        }
    }

    public void UnregisterAgent(Agent agent)
    {
        allAgents.Remove(agent);
        RemoveFromStateList(agent);

        if (currentSelectedAgent == agent)
        {
            ClearSelected();
        }
    }

    private void AddToStateList(Agent agent, Agent.State? forcedState = null)
    {
        Agent.State state = forcedState ?? agent.currentState;

        switch (state)
        {
            case Agent.State.Moving:
                if (!movingAgents.Contains(agent)) movingAgents.Add(agent);
                break;
            case Agent.State.Idle:
                if (!idleAgents.Contains(agent)) idleAgents.Add(agent);
                break;
        }
    }

    private void RemoveFromStateList(Agent agent, Agent.State? forcedState = null)
    {
        Agent.State state = forcedState ?? agent.currentState;

        switch (state)
        {
            case Agent.State.Moving:
                movingAgents.Remove(agent);
                break;
            case Agent.State.Idle:
                idleAgents.Remove(agent);
                break;
        }
    }

    public void UpdateAgentState(Agent agent, Agent.State newState)
    {
        RemoveFromStateList(agent);
        AddToStateList(agent, newState);
    }

    public void ClearSelected()
    {
        if (currentSelectedAgent != null)
        {
            currentSelectedAgent.Deselect();
            currentSelectedAgent = null;
        }
    }

    public int GetMovingAgentsCount() => movingAgents.Count;
    public int GetIdleAgentsCount() => idleAgents.Count;
    public int GetTotalAgentsCount() => allAgents.Count;
}