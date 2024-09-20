using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        Instance = this;
        cinemachinePerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (cinemachinePerlin != null)
        {
            cinemachinePerlin.m_AmplitudeGain = 0f;
        }
        
    }

    // 카메라 흔들림 함수
    public void ShakeCamera(float intensity, float time)
    {
        cinemachinePerlin.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                // 흔들림이 끝났을 때 강도를 0으로 설정하여 완전히 멈추기
                cinemachinePerlin.m_AmplitudeGain = 0f;
            }
            else
            {
                // 흔들림이 남아 있는 동안 부드럽게 줄이기.
                cinemachinePerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }
}

