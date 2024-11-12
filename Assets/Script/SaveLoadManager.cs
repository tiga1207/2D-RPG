using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
public class SaveLoadManager : MonoBehaviour
{
    private StubClient stubclient;
    private string host = "Public IPv4";
    private int port = 9090;

    private Player playerinfo;
    // private Skill skillinfo;
    // private Item iteminfo;
    // private InventorySlot itemcount;

    // Called before the first frame
    void Start()
    {
        stubclient = new StubClient(host, port);
    }

    public async void OnSaveButtonClick()
    {
        Player playerinfo = Player.LocalPlayerInstance;
        if (playerinfo != null && playerinfo.GetComponent<PhotonView>().IsMine)
        {
            // Userinfo
            int userid = playerinfo.userId;
            string nkname = playerinfo.NickNameText.text;   
            float curexp = playerinfo.Exp;
            float maxexp = playerinfo.MaxExp;
            float userlevel = playerinfo.Level;
            float curhp = playerinfo.Hp;
            float maxhp = playerinfo.MaxHp;
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
            var loadedInfo = await stubclient.getUserInfo(playerinfo.userId);
            playerinfo.maxHp = loadedInfo.Maxhp;
            playerinfo.maxExp = loadedInfo.Maxexp;
            playerinfo.maxMp = loadedInfo.Maxmp;
            
            StatUI.Instance.UpdateHP(loadedInfo.Maxhp);
            StatUI.Instance.UpdateMP(loadedInfo.Maxmp);

            await Task.Delay(100);

            playerinfo.Hp = loadedInfo.Curhp;
            playerinfo.Exp = loadedInfo.Curexp;
            playerinfo.Mp = loadedInfo.Curmp;

            playerinfo.HpBarController(loadedInfo.Curhp);
            playerinfo.MpBarController(loadedInfo.Curmp);

            playerinfo.Level = loadedInfo.Userlevel;
            playerinfo.LevelController(loadedInfo.Userlevel);


            playerinfo.Damage = loadedInfo.Attpower;
            playerinfo.LevelupStatPoint = loadedInfo.Statpoint;
            playerinfo.LevelupSkillPoint = loadedInfo.Skillpoint;

            var loadedLocation = await stubclient.getUserLocation(playerinfo.userId);
            playerinfo.xloc = loadedLocation.Xloc;
            playerinfo.yloc = loadedLocation.Yloc;
            playerinfo.zloc = loadedLocation.Zloc;
            playerinfo.transform.position = new Vector3(playerinfo.xloc, playerinfo.yloc, playerinfo.zloc);

            Debug.Log("User info loaded successfully.");
        }
        else
        {
            Debug.Log("Local player not found or not owned by PhotonView.");
        }
    }

    public async void OnRespawnFunc()
    {
        Player playerinfo = Player.LocalPlayerInstance;

        if (playerinfo != null && playerinfo.GetComponent<PhotonView>().IsMine)
        {
            // Userinfo
            int userid = playerinfo.userId;
            string nkname = playerinfo.NickNameText.text;
            float curexp = playerinfo.Exp;
            float maxexp = playerinfo.MaxExp;
            float userlevel = playerinfo.Level;
            float curhp = playerinfo.Hp;
            float maxhp = playerinfo.MaxHp;
            float curmp = playerinfo.Mp;
            float maxmp = playerinfo.MaxMp;
            float attpower = playerinfo.Damage;
            float statpoint = playerinfo.LevelupStatPoint;
            float skillpoint = playerinfo.LevelupSkillPoint;

            // Respawn
            float xloc = -39.60f;
            float yloc = -8.00f;
            float zloc = 0f;

            await stubclient.saveUserInfo(userid, nkname, curexp, maxexp, userlevel, curhp, maxhp, curmp, maxmp, attpower, statpoint, skillpoint);
            await stubclient.saveUserLocation(playerinfo.userId, xloc, yloc, zloc);

            Debug.Log("User Respawn successfully.");
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
