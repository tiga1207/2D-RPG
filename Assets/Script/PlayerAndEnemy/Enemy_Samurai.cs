using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class Enemy_Samurai : EnemyBase
{
    protected override void ReqeustRespawn(Vector3 respawnPosition)
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.RespawnSamurai(respawnPosition);
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
