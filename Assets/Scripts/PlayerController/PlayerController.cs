using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Vector2 movement_value, direction;
    private float left_jab_value, right_jab_value;
    private bool is_attacking;

    [Header("Animation Parameters")]
    public float speed = 1f;
    [Range(0f, 1f)] public float load = 0f;

    [Header("Movement Parameters")]
    public float movementSpeed = 1f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        direction = Vector2.zero;
    }

    void Update()
    {
        SetAnimationParameters();
    }

    //***INPUT***


    public void Movement(InputAction.CallbackContext context){ movement_value = context.ReadValue<Vector2>(); }
    public void LeftJab(InputAction.CallbackContext context) { left_jab_value = context.ReadValue<float>(); }
    public void RightJab(InputAction.CallbackContext context) { right_jab_value = context.ReadValue<float>();  }

    //***ANIMATION***

    /// <summary>
    /// Sets animation parameters for the animator.
    /// </summary>
    private void SetAnimationParameters()
    {
        is_attacking = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attacking");
        anim.SetBool("is_attacking", is_attacking);
        anim.SetFloat("load", load);
        anim.SetFloat("speed", speed);

        // MOVEMENT
        // Softens the movement by establishing the direction as a point that approaches the stick/mouse target.
        direction = Vector2.MoveTowards(direction, movement_value, movementSpeed * Time.deltaTime);
        anim.SetFloat("horizontal", direction.x);
        anim.SetFloat("vertical", direction.y);

        // ATTACKS
        anim.SetFloat("left_jab", left_jab_value);
        anim.SetFloat("right_jab", right_jab_value);
    }
}
