using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
   
    public bool m_musicEnabled = true;
    
    public bool m_fxEnabled = true;

    [Range(0,1)]
    public float m_musicVolume = 1.0f;

    [Range(0, 1)]
    public float m_fxVolume = 1.0f;

   public AudioClip m_clearRawSound;

    public AudioClip m_moveSound;

    public AudioClip m_dropSound;

    public AudioClip m_gameOverSound;

    //error sound
    public AudioClip m_errorSound;

    //public AudioClip m_backgroundMusic;

    public AudioSource m_musicSource;

    public AudioClip[] m_musicClips;

    AudioClip m_randomMusicClip;

    public AudioClip[] m_vocalClips;

    public AudioClip m_gameOverVocalClip;

    public AudioClip m_levelUpClip;

    public AudioClip m_holdSound;

    public IconToggle m_musicIconToggle;

    public IconToggle m_fxIconToggle;

    

    // Use this for initialization
    void Start ()
    {

        
        m_randomMusicClip = GetRandomClip(m_musicClips);
        PlayBackgroundMusic(m_randomMusicClip);
       // PlayBackgroundMusic(GetRandomClip(m_musicClips));
		
	}
    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }
    
	
	// Update is called once per frame
	void Update ()
    {
       
	}
    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        //return if music is disabled or if music Source is null or is musicClip is null
        if (!m_musicEnabled || !musicClip || !m_musicSource)
        {
            return;
        }
        //if music is playing stop it 
        m_musicSource.Stop();

        m_musicSource.clip = musicClip;
        //set the music volue
        m_musicSource.volume = m_musicVolume;

        //music repeats forever
        m_musicSource.loop = true;

        //start playing
        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        //if the tracks are not the same we exit,
        //if not we run this to enable music
        if (m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                m_randomMusicClip = GetRandomClip(m_musicClips);
                PlayBackgroundMusic(m_randomMusicClip);
            }
            else
            {
                m_musicSource.Stop();
            }
        }
    }
    public void ToggleMusic()
    {
        //switches between true to false
        m_musicEnabled =  !m_musicEnabled;
        //updates the music function
        UpdateMusic();

        if (m_musicIconToggle)
        {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }
    public void ToggleFX()
    {
        m_fxEnabled = !m_fxEnabled;

        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }
}
