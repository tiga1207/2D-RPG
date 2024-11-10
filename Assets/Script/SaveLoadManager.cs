// using System.Collections;
// using System.Collections.Generic;
using System.Threading.Tasks;
// using grpc_client;
using UnityEngine;
using Photon.Pun;
public class SaveLoadManager : MonoBehaviour
{
    private StubClient stubclient;
    private string host = "localhost";
    private int port = 9090;

    private Player playerinfo;
    // private Skill skillinfo;
    // private Item iteminfo;
    // private InventorySlot itemcount;

    // Called before the first frame
    void Start()
    {
        stubclient = new StubClient(host, port);
        // playerinfo = FindObjectOfType<Player>();

    }

    public async void OnSaveButtonClick()
    {
        // Player playerinfo = Player.LocalPlayerInstance;
        Player playerinfo = Player.LocalPlayerInstance;
        if (playerinfo != null && playerinfo.GetComponent<PhotonView>().IsMine)
        {
            // Userinfo
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

            // User Location
            float xloc = playerinfo.xloc;
            float yloc = playerinfo.yloc;
            float zloc = playerinfo.zloc;

            // // Skill Relation
            // int skillid = skillinfo.skillId;
            // int skilllevel = skillinfo.level;

            // // // Item Relation
            // string itemid = iteminfo.itemID;
            // int quantity = itemcount.ItemCount;

            // 서버에 데이터 전송
            await stubclient.saveUserInfo(userid, nkname, curexp, maxexp, userlevel, curhp, maxhp, curmp, maxmp, attpower, statpoint, skillpoint);
            await stubclient.saveUserLocation(userid, xloc, yloc, zloc);
            // await stubclient.saveSkillRelation(userid, skillid, skilllevel);
            // await stubclient.saveItemRelation(userid, itemid, quantity);

            Debug.Log("User info saved successfully.");
        }
    }

    // For Load Button
    public async void OnLoadButtonClick()
    {
        Player playerinfo = Player.LocalPlayerInstance;

        if (playerinfo != null && playerinfo.GetComponent<PhotonView>().IsMine)
        {
            // 1. 최대 체력, 최대 경험치, 최대 마나 불러오기
            var loadedInfo = await stubclient.getUserInfo(playerinfo.userId);
            playerinfo.maxHp = loadedInfo.Maxhp;
            playerinfo.maxExp = loadedInfo.Maxexp;
            playerinfo.maxMp = loadedInfo.Maxmp;
            
            StatUI.Instance.UpdateHP(loadedInfo.Maxhp);
            StatUI.Instance.UpdateMP(loadedInfo.Maxmp);
            StatUI.Instance.UpdateEXP(loadedInfo.Maxexp);




            Debug.Log("Max stats loaded.");

            // 0.1초 대기
            await Task.Delay(100);

            // 2. 현재 체력, 현재 경험치, 현재 마나 불러오기
            playerinfo.Hp = loadedInfo.Curhp;
            playerinfo.Exp = loadedInfo.Curexp;
            playerinfo.Mp = loadedInfo.Curmp;

            playerinfo.HpBarController(loadedInfo.Curhp);
            playerinfo.MpBarController(loadedInfo.Curmp);


            Debug.Log("Current stats loaded.");

            // 다른 속성 및 위치 정보 불러오기
            playerinfo.Level = loadedInfo.Userlevel;
            playerinfo.Damage = loadedInfo.Attpower;
            playerinfo.LevelupStatPoint = loadedInfo.Statpoint;
            playerinfo.LevelupSkillPoint = loadedInfo.Skillpoint;

            var loadedLocation = await stubclient.getUserLocation(playerinfo.userId);
            playerinfo.xloc = loadedLocation.Xloc;
            playerinfo.yloc = loadedLocation.Yloc;
            playerinfo.zloc = loadedLocation.Zloc;
            playerinfo.transform.position = new Vector3(playerinfo.xloc, playerinfo.yloc, playerinfo.zloc);

            Debug.Log("Player location and stats loaded and applied successfully.");
        }
        else
        {
            Debug.Log("Local player not found or not owned by PhotonView.");
        }
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
