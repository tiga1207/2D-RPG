using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instace;

    private void Awake() {
        instace=this;
    }

    public List<Item>itemDB= new List<Item>();

    public GameObject fieldItemPrefab;
    public Vector3[] pos;
    //public List<Transform> itemLocation; // 아이템 위치 목록

    public void Initialize()
    {
        for(int i=0; i<2; ++i)
        {
            GameObject go = PhotonNetwork.Instantiate(fieldItemPrefab.name,pos[i],Quaternion.identity);
            go.GetComponent<FieldItems>().SetItem(itemDB[Random.Range(0,2)]); 
        }
    }
    private void Start()
    {
        
    }
}
