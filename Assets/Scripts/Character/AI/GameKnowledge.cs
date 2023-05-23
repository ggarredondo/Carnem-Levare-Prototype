using UnityEngine;

public class GameKnowledge
{
    private readonly CharacterStats agentStats, opponentStats;
    private readonly CharacterStateMachine agentStateMachine, opponentStateMachine;
    private readonly Transform agentTransform, opponentTransform;

    private float distance;

    public GameKnowledge(in CharacterStats agentStats, in CharacterStats opponentStats, 
        in CharacterStateMachine agentStateMachine, in CharacterStateMachine opponentStateMachine)
    {
        this.agentStats = agentStats;
        this.opponentStats = opponentStats;
        this.agentStateMachine = agentStateMachine;
        this.opponentStateMachine = opponentStateMachine;

        agentTransform = agentStateMachine.transform;
        opponentTransform = opponentStateMachine.transform;
        UpdateKnowledge(0f);
    }

    public void UpdateKnowledge(float distanceError)
    {
        distance = Vector3.Distance(agentTransform.position, opponentTransform.position) + distanceError;
    }

    public float Distance => distance;
}
