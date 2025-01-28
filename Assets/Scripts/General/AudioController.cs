using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
   private bool _isMuted;
    [SerializeField] private AudioSource mAudioSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSourceForLoop;
    [SerializeField] private AudioClip background;
    [SerializeField] private AudioClip losing;
    [SerializeField] private AudioClip winning;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip oxygenDecrease;
    [SerializeField] private AudioClip oxygenIncrease;
    [SerializeField] private AudioClip shooting;
    [SerializeField] private AudioClip boltAttack;
    [SerializeField] private AudioClip switchStage;
    [SerializeField] private AudioClip ink;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip openScreen;
    [SerializeField] private AudioClip miniEnemyExplosion;

    
   
   
   public static AudioController Instance { get; private set; }

   private void Awake()
   {
       _isMuted = false;
       if (Instance != null)
       {
           Debug.Log("[Singleton] Trying to instantiate a second instance of a singleton class.");
           Destroy(gameObject);
       }
       else
       {
           Instance = this;
           DontDestroyOnLoad(Instance);
       }
   }


   public void StopMusic()
   {
       mAudioSource.Stop();
   }

   public void PlayMusic()
   {
       mAudioSource.clip = background;
       mAudioSource.loop = true;
       mAudioSource.Play();
       // Debug.Log("Start playing music");
   }

   private void PlaySfx(AudioClip clip)
   {
       sfxSource.PlayOneShot(clip);
       // Debug.Log("Start playing audio clip");
   }
   public float GetClipLength(AudioClip clip)
   {
       return clip.length;
   }
   
   public void MuteOrUnmute()
   {
       if(_isMuted)
       {
           UnMute();
       }
       else
       {
           Mute();
       }

   }
   
   private void Mute()
   {
       _isMuted = true;
       mAudioSource.mute = !mAudioSource.mute;
       sfxSource.mute = !sfxSource.mute;
   }
   
   private void UnMute()
   {
       _isMuted = false;
       mAudioSource.mute = false;
       sfxSource.mute = false;
   }
   
   public void PlayWinning()
   {
       PlaySfx(winning);
   }
   
   public void PlayLoosing()
   {
       PlaySfx(losing);
   }
   
    public void PlayEnemyHit()
    {
         PlaySfx(enemyHit);
    }
    public void PlayOxygenDecrease()
    {
         PlaySfx(oxygenDecrease);
    }
    public void PlayShooting()
    {
         PlaySfx(shooting);
    }
    
    public void PlayIncreaseOxygen()
    {
        PlaySfx(oxygenIncrease);
    }
    
    public void PlayBoltAttack()
    {
         PlaySfx(boltAttack);
    }
    public void PlaySwitchStage()
    {
         PlaySfx(switchStage);
    }
    
    public void StartPlayInkInLoop()
    {
        sfxSourceForLoop.clip = ink;
        sfxSourceForLoop.loop = true;
        sfxSourceForLoop.Play();
    }
    
    public void StopPlayInkInLoop()
    {
        sfxSourceForLoop.loop = false;
        sfxSourceForLoop.Stop();
    }
    
    public void StartPlayOpenScreenInLoop()
    {
        sfxSourceForLoop.clip = openScreen;
        sfxSourceForLoop.loop = true;
        sfxSourceForLoop.Play();
    }
    
    public void StopPlayOpenScreenInLoop()
    {
        sfxSourceForLoop.loop = false;
        sfxSourceForLoop.Stop();
    }
    
    
    public void PlayEnemyDeath()
    {
         PlaySfx(enemyDeath);
    }
    
    public void PlayExplosion()
    {
         PlaySfx(miniEnemyExplosion);
    }
    
}
