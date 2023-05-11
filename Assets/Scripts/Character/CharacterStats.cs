using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    private CharacterStateMachine stateMachine;

    [SerializeField] private float stamina, maxStamina;
    [SerializeField] private float characterDamage;

    [Tooltip("Percentage of stamina damage taken when blocking")]
    [SerializeField] [Range(0f, 1f)] private float blockingMultiplier;

    [Tooltip("How quickly time disadvantage decreases through consecutive hits (combo decay in ms x number of hits)")]
    [SerializeField] private float comboDecay = 200f;

    [SerializeField] [InitializationField] private float height = 1f, mass = 1f, drag;

    [SerializeField] private List<Move> moveList;
    [SerializeField] private List<Hitbox> hitboxList;

    [SerializeField] private bool noHurt;
    [SerializeField] private bool noDeath;

    public void Initialize(in Character character, in Rigidbody rb)
    {
        stamina = maxStamina;
        character.transform.localScale *= height;
        rb.mass = mass;
        rb.drag = drag;
        moveList.ForEach(m => m.Initialize());
    }
    public void Reference(in CharacterStateMachine stateMachine) => this.stateMachine = stateMachine;

    public float CalculateAttackDamage(float baseDamage) 
    {
        return baseDamage + characterDamage;
    }

    private void AddToStamina(float addend) => stamina = Mathf.Clamp(stamina + addend, 0f + System.Convert.ToSingle(noDeath), maxStamina);
    public void DamageStamina(in Hitbox hitbox)
    {
        if (!noHurt)
        {
            AddToStamina(-hitbox.Damage);
            if (stamina <= 0) stateMachine.TransitionToKO(hitbox);
            else stateMachine.TransitionToHurt(hitbox);
        }
    }
    public void DamageStaminaBlocked(in Hitbox hitbox)
    {
        if (!noHurt)
        {
            AddToStamina(-hitbox.Damage);
            if (stamina <= 0) stateMachine.TransitionToKO(hitbox);
            else stateMachine.TransitionToBlocked(hitbox);
        }
    }

    public ref readonly float Stamina { get => ref stamina; }
    public ref readonly float MaxStamina { get => ref maxStamina; }
    public ref readonly float BlockingMultiplier { get => ref blockingMultiplier; }
    public ref readonly float ComboDecay { get => ref comboDecay; }
    public ref readonly List<Move> MoveList { get => ref moveList; }
    public ref readonly List<Hitbox> HitboxList { get => ref hitboxList; }
}
