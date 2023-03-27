using UnityEngine;
using UnityEngine.Audio;

public class AudioSaver : MonoBehaviour
{
    public static AudioSaver Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Mixer")]
    public float globalVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool mute = false;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #region Public

    /// <summary>
    /// Save and apply the changes made in the audio menu
    /// </summary>
    public void ApplyChanges()
    {
        ApplyUI();

        //PERMANENT CHANGES
        SaveManager.Instance.activeSave.globalVolume = globalVolume;
        SaveManager.Instance.activeSave.musicVolume = musicVolume;
        SaveManager.Instance.activeSave.sfxVolume = sfxVolume;
        SaveManager.Instance.activeSave.mute = mute;
    }

    /// <summary>
    /// Apply the changes to the AudioMixer
    /// </summary>
    public void ApplyUI()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(globalVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolume) * 20);

        if (mute) MuteAll(); else UnMuteAll();
    }

    /// <summary>
    /// Load changes from the SaveManager to obtain the initial values
    /// </summary>
    public void LoadChanges()
    {
        globalVolume = SaveManager.Instance.activeSave.globalVolume;
        musicVolume = SaveManager.Instance.activeSave.musicVolume;
        sfxVolume = SaveManager.Instance.activeSave.sfxVolume;
        mute = SaveManager.Instance.activeSave.mute;

        ApplyUI();
    }

    /// <summary>
    /// Mute all the sounds
    /// </summary>
    public void MuteAll()
    {
        audioMixer.SetFloat("MasterVolume", -80);
    }

    /// <summary>
    /// UnMute all the sounds
    /// </summary>
    public void UnMuteAll()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(globalVolume) * 20);
    }

    #endregion
}
