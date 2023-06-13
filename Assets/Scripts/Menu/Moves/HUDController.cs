using System.Collections.Generic;
using UnityEngine;
using LerpUtilities;

public class HUDController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private List<GameObject> StaticHUDMenus;
    [SerializeField] private List<GameObject> HUDMenus;

    [Header("Parameters")]
    [Range(0,1)] [SerializeField] private float lerpDuration;


    private Player player;
    private GameObject enemy;
    private CameraController cameraController;
    private bool cameraChanged;

    private int actualHUDMenu = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        cameraController = GameObject.FindGameObjectWithTag("CAMERA").GetComponent<CameraController>();
    }

    private void OnEnable()
    {
        if(HUDMenus.Count > 1)
            inputReader.SelectMenuEvent += ChangeHUDMenu;
        inputReader.ChangeCamera += ChangeCamera;
        PauseController.EnterPause += DisableHUD;
        PauseController.ExitPause += EnableHUD;
    }

    private void OnDisable()
    {
        if (HUDMenus.Count > 1)
            inputReader.SelectMenuEvent -= ChangeHUDMenu;
        inputReader.ChangeCamera -= ChangeCamera;
        PauseController.EnterPause -= DisableHUD;
        PauseController.ExitPause -= EnableHUD;
    }

    private void DisableHUD()
    {
        if (HUDMenus.Count > 1)
            HUDMenus.ForEach(m => m.SetActive(false));
        StaticHUDMenus.ForEach(m => m.SetActive(false));
    }

    private void EnableHUD()
    {
        if (HUDMenus.Count > 1)
            HUDMenus[actualHUDMenu].SetActive(true);
        StaticHUDMenus.ForEach(m => m.SetActive(true));
    }

    private void ChangeCamera()
    {
        if (!cameraChanged)
        {
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            cameraController.changeVirtualCamera = CameraType.GOPRO;
            enemy.SetActive(false);
        }
        else
        {
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            cameraController.changeVirtualCamera = CameraType.DEFAULT;
            enemy.SetActive(true);
            AudioController.Instance.gameSfxSounds.StopAllSounds();
        }

        AudioController.Instance.uiSfxSounds.Play("CameraSwitch");
        cameraChanged = !cameraChanged;
    }

    public async void ChangeHUDMenu()
    {
        CanvasGroup actualCanvas = HUDMenus[actualHUDMenu].GetComponent<CanvasGroup>();

        if (actualCanvas != null)
        {
            AudioController.Instance.uiSfxSounds.Play("ExitMoveMenu");
            await Lerp.Value(actualCanvas.alpha, 0, (a) => actualCanvas.alpha = a, lerpDuration);
        }

        DisableHUD();
        actualHUDMenu = (actualHUDMenu + 1) % HUDMenus.Count;
        EnableHUD();

        actualCanvas = HUDMenus[actualHUDMenu].GetComponent<CanvasGroup>();

        if (actualCanvas != null)
        {
            AudioController.Instance.uiSfxSounds.Play("EnterMoveMenu");
            await Lerp.Value(actualCanvas.alpha, 1, (a) => actualCanvas.alpha = a, lerpDuration);
        }
    }    
}
