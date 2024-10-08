using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemDataBase : MonoBehaviourPunCallbacks
{
    public static ItemDataBase instance;

    private void Awake() {
        instance=this;
    }

    public List<Item>itemDB= new List<Item>();

    public GameObject fieldItemPrefab;
    public List<Transform> itemLocations; // 아이템 위치 목록

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
    public void itemAdd(int i)
    {
        if(i < itemDB.Count)
        {
            itemDB.Add(itemDB[i]);
        }
        else
        {
            Debug.Log("i의 값이 현재 아이템 db 수보다 큽니다.");
        }
    }
}
