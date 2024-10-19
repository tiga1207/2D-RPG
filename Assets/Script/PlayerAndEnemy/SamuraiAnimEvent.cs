using UnityEngine;

public class SamuraiAnimEvent : MonoBehaviour
{   
    private Enemy_Samurai enemy_Samurai;
    
    void Start()
    {
        enemy_Samurai =GetComponentInParent<Enemy_Samurai>();
    }


    private void CheckFinishAnim()
    {
        enemy_Samurai.Enemy_DieAfter();
    }

    private void AnimationTrigger()
    {
        if (!enemy_Samurai.isTakeDamage || !enemy_Samurai.isEnemyDie)
        {
            enemy_Samurai.AttackOver();
        }
    }

    private void TakeDamageAnimation()
    {
        if (!enemy_Samurai.isAttacking || !enemy_Samurai.isEnemyDie)
        {
            enemy_Samurai.TakeDamageOver();
        }
    }

}
