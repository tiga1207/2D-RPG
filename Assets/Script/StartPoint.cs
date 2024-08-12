// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StartPoint : MonoBehaviour
// {

//     public string startPoint;
//     private Player player;

//     void Start()
//     {
//         player=FindObjectOfType<Player>(); 
//         if(player!=null&&startPoint == player.currentMapName)
//         {
//             // if(startPoint == player.currentMapName)
//             // {
//                 player.transform.position = this.transform.position; 
//             // }
//         }
//     }

//     void Update()
//     {
        
//     }
// }
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public string startPoint;
    private Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            return;
        }
        //씬 이동시 해당 위치로 이동, 씬 이동 플래그를 사욯해서 아래 조건문이 시작되도록 바꿔야함.
        // if (startPoint == player.currentMapName)
        // {
        //     player.transform.position = this.transform.position;
        // }
    }
}
