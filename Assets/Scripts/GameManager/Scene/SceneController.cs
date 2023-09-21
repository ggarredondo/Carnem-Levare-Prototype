using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    private Dictionary<string, SceneLogic> scenesTable;
    private List<SceneLogic> scenes;
    private SceneLoader sceneLoader;

    private string currentScene;
    private string currentLoadScene;

    public SceneController(string initialScene, List<SceneLogic> scenes)
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        sceneLoader = new();
        scenesTable = new();

        currentScene = initialScene;

        this.scenes = scenes;
        Initialize();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentLoadScene)
        {
            GameManager.AudioController.InitializeSoundSources(scenesTable[scene.name].initSoundStructures);
            PlayMusic();
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        if (scene.name != currentLoadScene && scenesTable[scene.name].endSoundStructures.Count > 0)
            GameManager.AudioController.DeleteSoundSources(scenesTable[scene.name].endSoundStructures);
    }

    private void Initialize()
    {
        scenes.ForEach(s => scenesTable.Add(s.sceneName, s));
    }

    private void UpdateScene(string nextScene)
    {
        currentScene = nextScene;
    }

    public void PlayMusic()
    {
        if(scenesTable[currentScene].playMusic)
            GameManager.AudioController.PlayMusic(scenesTable[currentScene].music);
    }

    public List<SoundStructure> GetInitSounds()
    {
        return scenesTable[currentScene].initSoundStructures;
    }

    public void NextScene()
    {
        string nextScene = scenesTable[currentScene].nextScene.sceneName;

        if (scenesTable[currentScene].nextScene.withLoadScreen)
        {
            currentLoadScene = scenesTable[currentScene].nextScene.loadSceneName;
            sceneLoader.LoadWithLoadingScreen(nextScene, currentLoadScene);
        }
        else
            sceneLoader.LoadScene(nextScene);

        UpdateScene(nextScene);
    }

    public void PreviousScene()
    {
        string nextScene = scenesTable[currentScene].previousScene.sceneName;

        if (scenesTable[currentScene].previousScene.withLoadScreen)
        {
            currentLoadScene = scenesTable[currentScene].previousScene.loadSceneName;
            sceneLoader.LoadWithLoadingScreen(nextScene, currentLoadScene);
        }
        else
            sceneLoader.LoadScene(nextScene);

        UpdateScene(nextScene);
    }
}