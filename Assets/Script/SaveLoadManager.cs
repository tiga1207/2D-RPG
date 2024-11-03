// using System.Collections;
// using System.Collections.Generic;
using System.Threading.Tasks;
// using grpc_client;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private StubClient stubclient;
    private string host = "localhost";
    private int port = 9090;

    // Start is called before the first frame update
    void Start()
    {
        stubclient = new StubClient(host, port);
    }

    // For Save Button
    public async void OnSaveButtonClick()
    {
        await Task.Run(() =>
        {
            stubclient.saveUserInfo(1, "testuser1", 10, "2024-10-28 21:08:26");
            stubclient.saveItemRelation(1, "c100", 1);
            stubclient.saveItemRelation(1, "c101", 3);
            stubclient.saveMapProgress(1, 0, -40, -8, 0);

            Debug.Log("User info, item relations, and map progress saved successfully.");
        });
    }

    // For Load Button
    public async void OnLoadButtonClick()
    {
        await Task.Run(() =>
        {
            stubclient.getUserInfo(1);
            Debug.Log("User info, item relations, and map progress loaded successfully.");
        });
        
    }

    private async void OnDestroy()
    {
        // SaveLoadManager가 파괴될 때 StubClient 종료
        if (stubclient != null)
        {
            await stubclient.ShutdownAsync();  // 비동기 메서드 호출
        }
    }

}
