using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class Enemy_Samurai : EnemyBase
{
    [PunRPC]
    protected override void RequestDestroy(int viewID, Vector3 respawnPosition)
    {
        PhotonView enemyPV = PhotonView.Find(viewID);
        if (enemyPV != null && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(enemyPV.gameObject);
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.RespawnSamurai(respawnPosition);
            }
    }
    }
    protected override void RespawnEnemy(Vector3 respawnPosition)
    {

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null && PhotonNetwork.IsMasterClient)
        {
            enemyManager.RespawnSamurai(respawnPosition);
        }
    }

}
