
public class AIStateMachine
{
    private readonly AIController controller;
    private readonly GameKnowledge gameKnowledge;

    private AIState currentState;
    private NeutralState neutralState;

    public AIStateMachine(in AIController controller, in GameKnowledge gameKnowledge)
    {
        this.controller = controller;
        this.gameKnowledge = gameKnowledge;

        neutralState = new NeutralState();
        currentState = neutralState;
    }

    private void Think() => currentState.Think();

    private void ChangeState(in AIState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Enable(bool enabled) {}
}