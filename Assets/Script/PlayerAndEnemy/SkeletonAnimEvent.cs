using UnityEngine;

public class SkeletonAnimEvent : MonoBehaviour
{   
    private Enemy_Skeleton enemy_Skeleton;
    
    void Start()
    {
        enemy_Skeleton =GetComponentInParent<Enemy_Skeleton>();
    }


    private void CheckFinishAnim()
    {
        enemy_Skeleton.Enemy_DieAfter();
    }

    private void AnimationTrigger()
    {
        if (!enemy_Skeleton.isTakeDamage || !enemy_Skeleton.isEnemyDie)
        {
            enemy_Skeleton.AttackOver();
        }
    }

    private void TakeDamageAnimation()
    {
        if (!enemy_Skeleton.isAttacking || !enemy_Skeleton.isEnemyDie)
        {
            enemy_Skeleton.TakeDamageOver();
        }
    }

}
