using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private MenuController menuController;
    [SerializeField] private InputReader inputReader;

    [Header("Parameters")]
    [SerializeField] [Range(0f, 1f)] private float slowMotion;

    private bool pauseMenuActivated = false;

    private void Awake()
    {
        menuController.pauseMenu = true;
    }

    protected void OnEnable()
    {
        inputReader.StartPauseMenuEvent += EnterPauseMenu;
    }

    protected void OnDisable()
    {
        inputReader.StartPauseMenuEvent -= EnterPauseMenu;
    }

    private void EnterPauseMenu()
    {
        if (!pauseMenuActivated) EnterPauseMode();
        else ExitPauseMode(true);
    }

    public void EnterPauseMode()
    {
        Time.timeScale = slowMotion;
        menuController.tree.Initialize();
        pauseMenuActivated = true;
        GameManager.PlayerInput.SwitchCurrentActionMap("UI");
        AudioManager.Instance.PauseGame(true);
    }

    public void ExitPauseMode(bool resumeSounds)
    {
        Time.timeScale = 1;
        menuController.DisableMenus();
        pauseMenuActivated = false;
        GameManager.PlayerInput.SwitchCurrentActionMap("Main Movement");
        AudioManager.Instance.PauseGame(false && resumeSounds);
    }
}