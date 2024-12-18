using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FieldItems : MonoBehaviourPun
{
    public Item item;
    public SpriteRenderer image;

    [PunRPC]
    public void SetItemID(string itemID)
    {
        item = ItemDataBase.instance.GetItemByID(itemID);
        if (item != null)
        {
            image.sprite = item.itemImage;
        }
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroyItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            photonView.RPC("RequestDestroyItem", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    public void RequestDestroyItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    
}