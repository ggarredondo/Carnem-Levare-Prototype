using UnityEngine;
using Cinemachine;
using System;

public class CameraFollow : MonoBehaviour
{

    //Max value of the damping parameter (Virtual camera)
    const float MAX = 1;
    const float MIN = 0;
    const float CAMERA_SPEED_DIVIDER = 1000;

    public int holdingMinFrames;

    [Header("Target Objects")]
    public PlayerController playerController;
    public CinemachineVirtualCamera vcam;

    [Header("Follow Parameters")]
    [Range(0f, 5f)] public float cameraAceleration;
    public float MAX_DAMPING = 20;
    public float MIN_DAMPING = 0;
    public bool cameraRotation;

    private float reduceDamping;
    private CinemachineTransposer transposer;

    [Header("Noise Parameters")]
    [Range(0f, 5f)] public float noiseAceleration;
    public float MAX_FREQUENCY = 3;
    public float MIN_FREQUENCY = 1;
    public float MAX_AMPLITUDE = 3;
    public float MIN_AMPLITUDE = 1;
    public bool cameraNoise;

    private float reduceAmplitude, reduceFrequency;
    private CinemachineBasicMultiChannelPerlin noiseTransposer;

    [Header("Movement Parameters")]
    public Vector2 zoomAceleration;
    public Vector2 fovAceleration;
    public Vector3[] blockingPositions;
    public float[] fieldOfView;
    private float reduceFOV, reduceBlock;

    private void Awake()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        noiseTransposer = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        reduceDamping = MIN;
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 1)
        {
            if (cameraRotation)
            {
                transposer.m_YawDamping = OscillateWithVelocity(playerController.getIsMoving, cameraAceleration, ref reduceDamping, MIN_DAMPING, MAX_DAMPING, Exponential);

                //transposer.m_FollowOffset = QuadraticCameraMovement(playerController.rightNormalSlot.chargePhase == Move.ChargePhase.performing, zoomAceleration, ref reduceCharge, positions[0], positions[1], positions[2]);

                int frameDiference = Time.frameCount - playerController.rightNormalSlot.getLastFrame;
                transposer.m_FollowOffset = LinealCameraMovement(playerController.rightNormalSlot.getChargePhase == Move.ChargePhase.performing && frameDiference >= holdingMinFrames,
                                                                 zoomAceleration, ref reduceBlock, blockingPositions[0], blockingPositions[1]);

                vcam.m_Lens.FieldOfView = OscillateWithVelocity(playerController.rightNormalSlot.getChargePhase == Move.ChargePhase.performing && frameDiference >= holdingMinFrames,
                                                                fovAceleration, ref reduceFOV, fieldOfView[0], fieldOfView[1], Mathf.Sin);
            }

            if (cameraNoise)
            {
                noiseTransposer.m_FrequencyGain = OscillateWithVelocity(playerController.getIsMovement, noiseAceleration, ref reduceFrequency, MIN_FREQUENCY, MAX_FREQUENCY, Exponential);
                noiseTransposer.m_AmplitudeGain = OscillateWithVelocity(playerController.getIsMovement, noiseAceleration, ref reduceAmplitude, MIN_AMPLITUDE, MAX_AMPLITUDE, Exponential);
            }
        }
    }

    private float NormalizeToInterval(float num, float min, float max)
    {
        return num * (max - min) + min;
    }

    private float Exponential(float x)
    {
        return Mathf.Pow(x, 4);
    }

    private float OscillateWithVelocity(bool condition, float aceleration, ref float reduce, float min, float max, Func<float, float> function)
    {
        //Apply the reduce to the actual value
        float value = MAX * function(reduce);

        //Increment or Decrement the reduce to make feel aceleration
        if (condition) reduce += aceleration / CAMERA_SPEED_DIVIDER;
        else           reduce -= aceleration / CAMERA_SPEED_DIVIDER;

        //Make values be under damping parameter range
        reduce = Mathf.Clamp(reduce, MIN, MAX);

        //Apply to the virtual camera parameter
        return NormalizeToInterval(value, min, max);   
    }

    private float OscillateWithVelocity(bool condition, Vector2 aceleration, ref float reduce, float min, float max, Func<float, float> function)
    {
        //Apply the reduce to the actual value
        float value = MAX * function(reduce);

        //Increment or Decrement the reduce to make feel aceleration
        if (condition) reduce += aceleration.x / CAMERA_SPEED_DIVIDER;
        else reduce -= aceleration.y / CAMERA_SPEED_DIVIDER;

        //Make values be under damping parameter range
        reduce = Mathf.Clamp(reduce, MIN, MAX);

        //Apply to the virtual camera parameter
        return NormalizeToInterval(value, min, max);
    }

    private Vector3 LinearBezierCurve(float time, Vector3 P0, Vector3 P1)
    {
        return P0 + time * (P1 - P0);
    }

    private Vector3 QuadraticBezierCurve(float time, Vector3 P0, Vector3 P1, Vector3 P2)
    {
        float _time = 1 - time;
        return Mathf.Pow(_time, 2) * P0 + 2 * _time * time * P1 + Mathf.Pow(time, 2) * P2;
    }

    private Vector3 LinealCameraMovement(bool condition, Vector2 aceleration, ref float reduce, Vector3 P0, Vector3 P1)
    {
        //Apply the reduce to the actual value
        Vector3 value = LinearBezierCurve(reduce, P0, P1);

        //Increment or Decrement the reduce to make feel aceleration
        if (condition) reduce += aceleration.x / CAMERA_SPEED_DIVIDER;
        else reduce -= aceleration.y / CAMERA_SPEED_DIVIDER;

        //Make values be under damping parameter range
        reduce = Mathf.Clamp(reduce, MIN, MAX);

        //Apply to the virtual camera parameter
        return value;
    }

    private Vector3 QuadraticCameraMovement(bool condition, Vector2 aceleration, ref float reduce, Vector3 P0, Vector3 P1, Vector3 P2)
    {
        //Apply the reduce to the actual value
        Vector3 value = QuadraticBezierCurve(reduce, P0, P1, P2);

        //Increment or Decrement the reduce to make feel aceleration
        if (condition) reduce += aceleration.x / CAMERA_SPEED_DIVIDER;
        else reduce -= aceleration.y / CAMERA_SPEED_DIVIDER;

        //Make values be under damping parameter range
        reduce = Mathf.Clamp(reduce, MIN, MAX);

        //Apply to the virtual camera parameter
        return value;
    }
}