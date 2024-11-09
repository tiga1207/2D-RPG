using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private Player currentPlayer;
    public static PlayerSoundManager Instance;
    public AudioSource audioSource; 
    public AudioClip dashSound;
    public AudioClip attackSound;
    public AudioClip attackSound2;

    public AudioClip healSound;
    public AudioClip deadSound;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 공격 소리를 재생하는 함수
    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayAttackSound2()
    {
        if (attackSound2 != null)
        {
            audioSource.PlayOneShot(attackSound2);
        }
    }

    // 대시 소리를 재생하는 함수
    public void PlayDashSound()
    {
        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }

    // 치유 소리를 재생하는 함수
    public void PlayHealSound()
    {
        if (healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }
    }

    public void PlayDeadSound()
    {
        if (deadSound != null)
        {
            audioSource.PlayOneShot(deadSound);
        }
    }

    public void SetPlayer(Player player)
    {
        currentPlayer = player;
    }
}
