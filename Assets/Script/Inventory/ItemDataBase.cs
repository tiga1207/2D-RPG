using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemDataBase : MonoBehaviourPunCallbacks
{
    public static ItemDataBase instace;

    private void Awake() {
        instace=this;
    }

    public List<Item>itemDB= new List<Item>();

    public GameObject fieldItemPrefab;
    public List<Transform> itemLocations; // 아이템 위치 목록

    // public void Initialize()
    // {
    //     for(int i=0; i<itemDB.Count; ++i)
    //     {
    //         GameObject go = PhotonNetwork.Instantiate(fieldItemPrefab.name,pos[i],Quaternion.identity);
    //         go.GetComponent<FieldItems>().SetItem(itemDB[i]); 
    //     }
    // }

    public Item GetItemByID(string id)
    {
        return itemDB.Find(item => item.itemID == id);
    }

    public void Initialize()
    {
        for (int i = 0; i < itemDB.Count; ++i)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                Vector3 itemPostion = itemLocations[i].position;
                GameObject go = PhotonNetwork.Instantiate(fieldItemPrefab.name, itemPostion, Quaternion.identity);
                go.GetComponent<PhotonView>().RPC("SetItemID", RpcTarget.AllBuffered, itemDB[i].itemID);
            }
        }
    }
    private void Start()
    {
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     Initialize();
        // }
        
    }
}
