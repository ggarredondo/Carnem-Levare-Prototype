using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CharacterAnimation
{
    private HurtState hurtState;
    private BlockedState blockedState;
    private KOState koState;
    private List<Move> moveList;

    private Animator animator;
    private AnimatorOverrideController animatorOverride;
    private AnimationClip[] animatorDefaults;

    [SerializeField] private float animatorSpeed = 1f;
    [SerializeField] private bool updateMovesetAnimations = false;
    [SerializeField] private bool updateAnimationEvents = false;

    public void Initialize(in Animator animator)
    {
        this.animator = animator;
        animatorDefaults = animator.runtimeAnimatorController.animationClips;
        animatorOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
    }
    public void Reference(in CharacterStateMachine stateMachine, in CharacterStats stats, in CharacterMovement movement)
    {
        moveList = stats.MoveList;
        UpdateMovesetAnimations();
        hurtState = stateMachine.HurtState;
        blockedState = stateMachine.BlockedState;
        koState = stateMachine.KOState;

        movement.OnMoveCharacter += MovementAnimation;

        stateMachine.WalkingState.OnEnter += EnterWalkingState;
        stateMachine.WalkingState.OnExit += ExitWalkingState;

        stateMachine.BlockingState.OnEnter += EnterBlockingState;
        stateMachine.BlockingState.OnExit += ExitBlockingState;

        stateMachine.MoveState.OnEnterInteger += EnterMoveState;

        stateMachine.HurtState.OnEnter += EnterHurtState;
        stateMachine.HurtState.OnExit += ExitHurtState;

        stateMachine.BlockedState.OnEnter += EnterBlockedState;
        stateMachine.BlockedState.OnExit += ExitBlockedState;

        stateMachine.KOState.OnEnter += EnterKOState;
        stateMachine.KOState.OnExit += ExitKOState;
    }
    public void OnValidate()
    {
        if (animator != null) animator.speed = animatorSpeed;
        if (updateMovesetAnimations) { UpdateMovesetAnimations(); updateMovesetAnimations = false; }
        if (updateAnimationEvents) { moveList.ForEach(m => m.AssignEvents()); }
    }

    private void UpdateAnimation(string originalClip, AnimationClip newClip)
    {
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(animatorDefaults.Where(clip => clip.name == originalClip).SingleOrDefault(), newClip));
        animatorOverride.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = animatorOverride;
    }

    private void UpdateMovesetAnimations()
    {
        for (int i = 0; i < moveList.Count; ++i)
        {
            UpdateAnimation("MoveClip" + i, moveList[i].Animation);
            animator.SetFloat("move" + i + "_speed", moveList[i].AnimationSpeed);
        }
    }

    private void MovementAnimation(in Vector2 direction)
    {
        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
    }

    private void TriggerHurtAnimation(float target, float stagger)
    {
        animator.SetFloat("hurt_target", target);
        animator.SetFloat("hurt_stagger", stagger);
        animator.SetTrigger("hurt");
    }

    #region Enter/Exit States

    private void EnterWalkingState() { animator.SetBool("STATE_WALKING", true); }
    private void ExitWalkingState() { animator.SetBool("STATE_WALKING", false); }

    private void EnterBlockingState() { animator.SetBool("STATE_BLOCKING", true); }
    private void ExitBlockingState() { animator.SetBool("STATE_BLOCKING", false); }

    private void EnterMoveState(int moveIndex) { animator.SetTrigger("move" + moveIndex); }

    private void EnterHurtState() 
    {
        Hitbox hitbox = hurtState.Hitbox;
        animator.SetBool("STATE_HURT", true);
        TriggerHurtAnimation(hitbox.AnimationBodyTarget, hitbox.AnimationStagger);
    }
    private void ExitHurtState() { animator.SetBool("STATE_HURT", false); }

    private void EnterBlockedState()
    {
        Hitbox hitbox = blockedState.Hitbox;
        animator.SetBool("STATE_BLOCKED", true);
        TriggerHurtAnimation(hitbox.AnimationBodyTarget, hitbox.AnimationStagger);
    }
    private void ExitBlockedState() { animator.SetBool("STATE_BLOCKED", false); }

    private void EnterKOState()
    {
        Hitbox hitbox = koState.Hitbox;
        animator.SetFloat("hurt_target", hitbox.AnimationBodyTarget);
        animator.SetFloat("hurt_stagger", hitbox.AnimationStagger);
        animator.SetBool("STATE_KO", true);
    }
    private void ExitKOState() { animator.SetBool("STATE_KO", false); }

    #endregion
}
