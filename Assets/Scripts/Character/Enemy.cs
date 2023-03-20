using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy-specific Parameters")]
    [SerializeField] private float enemyDirectionSpeed = 1f;
    [SerializeField] private bool block = false, jab = false;

    protected override void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        base.Start();
    }
    protected override void Update()
    {
        directionSpeed = enemyDirectionSpeed;
        Behaviour();
        base.Update();
    }

    private void Behaviour() {
        Block(block);
        LeftN(jab, 0);
    }
}
