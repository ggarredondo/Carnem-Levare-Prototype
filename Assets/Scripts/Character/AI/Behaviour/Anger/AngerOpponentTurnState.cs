
public class AngerOpponentTurnState : AIState
{
    private AngerAIStateMachine aiFSM;
    private AIController controller;
    private GameKnowledge gameKnowledge;
    public AngerOpponentTurnState(in AngerAIStateMachine aiFSM, in AIController controller, in GameKnowledge gameKnowledge)
    {
        this.aiFSM = aiFSM;
        this.controller = controller;
        this.gameKnowledge = gameKnowledge;
    }

    public void Enter() {}
    public void Update() {}
    public void Exit() {}
}
