using UnityEngine;
using System.Threading.Tasks;

public class CombatEnd : MonoBehaviour
{
    private Player player;
    private Enemy enemy;

    [Header("Requirements")]
    [SerializeField] private RewardGenerator rewardGenerator;

    [Header("Parameters")]
    [SerializeField] private float waitAfterDeath;
    [SerializeField] private float waitAfterReward;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    private void Start()
    {
        enemy.StateMachine.KOState.OnEnter += Victory;
        player.StateMachine.KOState.OnEnter += Defeat;
    }

    private void OnDestroy()
    {
        enemy.StateMachine.KOState.OnEnter -= Victory;
        player.StateMachine.KOState.OnEnter -= Defeat;
    }

    private async void Victory()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(waitAfterDeath));
        GameManager.InputUtilities.EnablePlayerInput(false);

        enemy.EnemyDrops.ForEach(m =>
        {
            DataSaver.Game.moves.Add(m);
            DataSaver.Game.newMoves.Add(true);
        });

        await rewardGenerator.GenerateMove(enemy.EnemyDrops);
        await Task.Delay(System.TimeSpan.FromSeconds(waitAfterReward));

        TransitionPlayer.extraTime = 1;
        TransitionPlayer.text.text = "YOU LIVE";

        GameManager.AudioController.Play("PlayGame");
        GameManager.Scene.NextScene();
    }

    private async void Defeat()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(3));

        TransitionPlayer.extraTime = 2;
        TransitionPlayer.text.text = "YOU DIED";

        GameManager.AudioController.Play("BackMenu");
        GameManager.Scene.PreviousScene();
    }
}
