using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 카메라 오브젝트를 파괴하지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 인스턴스가 있으면 파괴
        }
    }
}
