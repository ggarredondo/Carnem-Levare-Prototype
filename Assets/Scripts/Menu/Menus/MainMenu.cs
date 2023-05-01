using UnityEngine;
using UnityEngine.UI;

public class MainMenu : AbstractMenu
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private bool isLoading;

    protected override void Configure()
    {
        AudioManager.Instance.PlayMusic("Intro");

        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        if (!isLoading)
        {
            AudioManager.Instance.uiSfxSounds.Play("PlayGame");
            isLoading = true;
            GameManager.SceneLoader.LoadWithLoadingScreen(SceneNumber.GAME);
        }
    }
}
