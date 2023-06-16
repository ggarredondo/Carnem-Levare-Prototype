using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Controller controller;
    protected CharacterStateMachine stateMachine;
    [SerializeField] protected CharacterStats characterStats;
    [SerializeField] protected CharacterMovement characterMovement;
    [SerializeField] protected CharacterAnimation characterAnimation;
    [SerializeField] protected CharacterAudio characterAudio;
    protected Transform target;

    protected virtual void Awake()
    {
        controller = GetComponent<Controller>();
        controller.Initialize();
        stateMachine = GetComponent<CharacterStateMachine>();
        stateMachine.Initialize();
        characterStats.Initialize(this, GetComponent<Rigidbody>());
        characterMovement.Initialize(transform, target, GetComponent<Rigidbody>());
        characterAnimation.Initialize(GetComponent<Animator>());
    }
    protected virtual void Start()
    {
        stateMachine.Reference(controller, characterStats, characterMovement);
        characterStats.Reference(stateMachine);
        characterAudio.Reference(stateMachine, characterStats);
        characterAnimation.Reference(stateMachine, characterStats, characterMovement);
        stateMachine.TransitionToWalking(); // Must be done last
    }
    protected void OnValidate()
    {
        characterAnimation.OnValidate();
        CharacterStats.OnValidate(this, GetComponent<Rigidbody>());
    }

    public ref readonly CharacterStateMachine StateMachine { get => ref stateMachine; }
    public ref readonly Controller Controller { get => ref controller; }
    public ref readonly CharacterMovement CharacterMovement { get => ref characterMovement; }
    public ref readonly CharacterStats CharacterStats { get => ref characterStats; }
    public ref readonly CharacterAnimation CharacterAnimation { get => ref characterAnimation; }
    public ref readonly CharacterAudio CharacterAudio { get => ref characterAudio; }
}
