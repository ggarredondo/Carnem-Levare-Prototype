using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    private bool isAttacking, canAttack, canSkip;

    [Header("Input Parameters")]

    [Tooltip("How quickly player animations follows stick movement")]
    [SerializeField] private float stickSpeed;

    [Tooltip("Lower stickSpeed to smooth out transitions to idle (when stick is centered)")]
    [SerializeField] private float smoothStickSpeed;

    [Tooltip("How much off axis movement do you allow when attempting stick tapping actions?")]
    [SerializeField] private float stickTapTolerance = 0.1f;
    private bool canTapStick = true;

    [Header("Debug")]
    [SerializeField] private bool modifyTimeScale = false;
    [SerializeField] [Range(0f, 1f)] private float timeScale = 1f;

    override protected void Update()
    {
        // Bellow are values that must be updated frame by frame to allow certain animations to play out accordingly.
        isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attacking") && !anim.IsInTransition(0);

        // The player can only attack if they're not attacking already.
        canAttack = !isAttacking;
        anim.SetBool("can_attack", canAttack);

        // The player can only skip if they are blocking but not attacking.
        canSkip = canTapStick && !isAttacking;
        anim.SetBool("can_skip", canSkip);

        directionSpeed = directionTarget.magnitude == 0f && !isBlocking ? smoothStickSpeed : stickSpeed;
        base.Update();

        if (modifyTimeScale) Time.timeScale = timeScale; // DEBUG
    }

    #region Input
    // Meant for Unity Input System events

    public void Movement(InputAction.CallbackContext context) { 
        directionTarget = context.ReadValue<Vector2>().normalized;

        // Forward and backwards skips trigger by tapping the left stick twice (up or down respectively).
        // However, they may also trigger by drawing circles in the stick. We have to make sure there is
        // no horizontal movement before allowing the player to skip jump, otherwise they may trigger it
        // by mistake when dodging attacks.
        // We, however, allow some horizontal movement so that it doesn't have to be too precise.
        // In the range of [-stickTapTolerance, stickTapTolerance]
        if (directionTarget.x < -stickTapTolerance || directionTarget.x > stickTapTolerance) canTapStick = false;
        if (directionTarget.magnitude == 0f) canTapStick = true;
    }

    public void SkipFwd(InputAction.CallbackContext context) { anim.SetBool("skip_fwd", context.performed && isBlocking); }
    public void SkipBwd(InputAction.CallbackContext context) { anim.SetBool("skip_bwd", context.performed && isBlocking); }
    public void LeftNormal(InputAction.CallbackContext context) { anim.SetBool("left_normal", context.performed); }
    public void LeftSpecial(InputAction.CallbackContext context) { anim.SetBool("left_special", context.performed); }
    public void RightNormal(InputAction.CallbackContext context) { rightMoveset[0].pressed = context.performed; anim.SetBool("right_normal", context.performed); }
    public void RightSpecial(InputAction.CallbackContext context) { rightMoveset[1].pressed = context.performed; anim.SetBool("right_special", context.performed); }
    public void Block(InputAction.CallbackContext context) { isBlocking = context.performed; }
    #endregion

    #region PublicMethods
    /// <summary>
    /// Is the player in any State tagged as "Movement"?
    /// </summary>
    public bool getIsMovement { get { return anim.GetCurrentAnimatorStateInfo(0).IsTag("Movement"); } }

    /// <summary>
    /// Is the player moving? The player may move by pressing the stick or by attacking.
    /// </summary>
    public bool getIsMoving { get { return !(directionTarget.magnitude == 0f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Movement")); } }

    public float getDirectionX { get { return direction.x; } }

    public bool getIsBlocking { get { return isBlocking; } }
    #endregion

}