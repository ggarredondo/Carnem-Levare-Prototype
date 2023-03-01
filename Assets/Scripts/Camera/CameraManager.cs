using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public VirtualCameras changeVirtualCamera;
    public static VirtualCameras actualVirtualCamera;

    private CameraTargets playerTargets, enemyTargets;
    private CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        playerTargets = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraTargets>();
        enemyTargets = GameObject.FindGameObjectWithTag("Enemy").GetComponent<CameraTargets>();
        targetGroup = GameObject.FindGameObjectWithTag("TARGET_GROUP").GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        InitializeTargets();
    }

    private void InitializeTargets()
    {
        targetGroup.m_Targets[0].target = playerTargets.GetTarget(0, false);
        targetGroup.m_Targets[1].target = enemyTargets.GetTarget(0, false);

        int cont = 0;
        foreach (CinemachineVirtualCamera camera in GetComponentsInChildren<CinemachineVirtualCamera>())
        {
            camera.m_Follow = playerTargets.GetTarget(cont, false);

            if (cont != 2 && cont != 1)
                camera.GetComponent<CameraEffects>().InitilizeTargets(playerTargets.GetTarget(cont, true), enemyTargets.GetTarget(cont, true));

            if (cont == 2)
                camera.m_LookAt = playerTargets.GetTarget(cont, true);

            cont++;
        }
    }

    private void ChangeVirtualCamera()
    {
        actualVirtualCamera = changeVirtualCamera;

        int cont = 0;
        foreach (CinemachineVirtualCamera camera in GetComponentsInChildren<CinemachineVirtualCamera>())
        {
            if (cont == (int) actualVirtualCamera)
            {
                camera.enabled = true;
                camera.GetComponent<CameraEffects>().enabled = true;
            }
            else
            {
                camera.enabled = false;
                camera.GetComponent<CameraEffects>().enabled = false;
            }

            cont++;
        }
    }

    private void LateUpdate()
    {
        if (actualVirtualCamera != changeVirtualCamera) ChangeVirtualCamera();
    }

}

public enum VirtualCameras
{
    STANDARD = 0,
    FIRST_PERSON = 1,
    GOPRO = 2,
    DRONE = 3
}
