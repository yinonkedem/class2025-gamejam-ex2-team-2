using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
   private bool _isMuted;
    [SerializeField] private AudioSource mAudioSource;
    [SerializeField] private AudioSource sfxSource;
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
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip openScreen;
    
   
   
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

   public void Start()
   {
       PlayMusic(background);
       // make the background be with littler volume
   }

   public void StopMusic()
   {
       mAudioSource.Stop();
   }

   private void PlayMusic(AudioClip clip)
   {
       mAudioSource.clip = clip;
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
   
   public void PlayBackground()
   {
       PlaySfx(background);
   }
   public void PlayLosing()
   {
       PlaySfx(losing);
   }
   public void PlayWinning()
   {
       PlaySfx(winning);
   }
   public void PlayOpenScreen()
   {
       PlaySfx(openScreen);
   }
    public void PlayEnemyHit()
    {
         PlaySfx(enemyHit);
    }
    public void PlayOxygenDecrease()
    {
         PlaySfx(oxygenDecrease);
    }
    public void PlayOxygenIncrease()
    {
         PlaySfx(oxygenIncrease);
    }
    public void PlayShooting()
    {
         PlaySfx(shooting);
    }
    public void PlayBoltAttack()
    {
         PlaySfx(boltAttack);
    }
    public void PlaySwitchStage()
    {
         PlaySfx(switchStage);
    }
    public void PlayInk()
    {
         PlaySfx(ink);
    }

    public void PlayExplosion()
    {
         PlaySfx(explosion);
    }
}
