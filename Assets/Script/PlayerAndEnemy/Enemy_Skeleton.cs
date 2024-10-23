using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class Enemy_Skeleton : EnemyBase
{
    protected override void ReqeustRespawn(Vector3 respawnPosition)
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.RespawnSkeleton(respawnPosition);
        }
    }
    protected override void RespawnEnemy(Vector3 respawnPosition)
    {

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null && PhotonNetwork.IsMasterClient)
        {
            enemyManager.RespawnSkeleton(respawnPosition);
        }
    }

}
