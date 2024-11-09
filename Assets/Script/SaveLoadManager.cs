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
    private Player playerinfo;

    // Called before the first frame
    void Start()
    {
        stubclient = new StubClient(host, port);
        playerinfo = FindObjectOfType<Player>();
    }

    public async void OnSaveButtonClick()
    {
        await Task.Run(() =>
        {
            // PlayerStats에서 데이터 가져오기
            int userid = playerinfo.userId;
            string nkname = playerinfo.NickNameText.text;        // NickNameText? NickName?
            float curexp = playerinfo.Exp;
            float maxexp = playerinfo.MaxExp;
            float userlevel = playerinfo.Level;
            float curhp = playerinfo.Hp;
            float maxhp = playerinfo.MaxMp;
            float curmp = playerinfo.Mp;
            float maxmp = playerinfo.MaxMp;
            float attpower = playerinfo.Damage;
            float statpoint = playerinfo.LevelupStatPoint;
            float skillpoint = playerinfo.LevelupSkillPoint;
            string savetime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 서버에 데이터 전송
            stubclient.saveUserInfo(userid, nkname, curexp, maxexp, userlevel, curhp, maxhp, curmp, maxmp, attpower, statpoint, skillpoint);

            Debug.Log("User info saved successfully.");
        });
    }

    // For Load Button
    public async void OnLoadButtonClick()
    {
        await Task.Run(() =>
        {
            stubclient.getUserInfo(1);
            Debug.Log("User info loaded successfully.");
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
