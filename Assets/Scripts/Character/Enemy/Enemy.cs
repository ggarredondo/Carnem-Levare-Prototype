using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Character
{
    [Header("Movement Parameters")]
    [SerializeField] private float enemyDirectionSpeed = 1f;

    public void Movement(InputAction.CallbackContext context) { directionTarget = -context.ReadValue<Vector2>().normalized; }
    public void Block(InputAction.CallbackContext context) { block_pressed = !block_pressed; }

    public void Left0(InputAction.CallbackContext context) { if (LeftMoveset.Count > 0) anim.SetBool("left0", context.performed); }
    public void Left1(InputAction.CallbackContext context) { if (LeftMoveset.Count > 1) anim.SetBool("left1", context.performed); }
    public void Left2(InputAction.CallbackContext context) { if (LeftMoveset.Count > 2) anim.SetBool("left2", context.performed); }

    public void Right0(InputAction.CallbackContext context) { if (RightMoveset.Count > 0) anim.SetBool("right0", context.performed); }
    public void Right1(InputAction.CallbackContext context) { if (RightMoveset.Count > 1) anim.SetBool("right1", context.performed); }
    public void Right2(InputAction.CallbackContext context) { if (RightMoveset.Count > 2) anim.SetBool("right2", context.performed); }


    protected override void Start()
    {
        block_pressed = true;
        base.Start();
    }

    protected override void Update()
    {
        directionSpeed = enemyDirectionSpeed;
        base.Update();
    }
}
