using UnityEngine;

public class BossAnimEvent : MonoBehaviour
{   
    private Boss boss;
    
    void Start()
    {
        boss =GetComponentInParent<Boss>();
    }


    private void CheckFinishAnim()
    {
        boss.Enemy_DieAfter();
    }

    private void AnimationTrigger()
    {
        if (!boss.isTakeDamage || !boss.isEnemyDie)
        {
            boss.BossAttackOver();
        }
    }

    private void TakeDamageAnimation()
    {
        if (!boss.isAttacking || !boss.isEnemyDie)
        {
            boss.TakeDamageOver();
        }
    }

}
