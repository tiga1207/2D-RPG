using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip deadSound;

    // 공격 소리를 재생하는 함수
    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayDeadSound()
    {
        if (deadSound != null)
        {
            audioSource.PlayOneShot(deadSound);
        }
    }

}

