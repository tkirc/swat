using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : NetworkBehaviour
{

    public static AudioManager instance = null;
    private const int BACKGROUND_MUSIC_INDEX = 0;
    private const int SOURCE_COUNT = 15;

    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioMixerGroup mixer;
    [SerializeField] private float delay;

    void Awake()
    {

        instance = this;
        sources = new AudioSource[SOURCE_COUNT];

        for (int i = 0; i < SOURCE_COUNT; i++)
        {

            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].outputAudioMixerGroup = mixer;

        }

        sources[BACKGROUND_MUSIC_INDEX].loop = true;
        setBackgroundMusic(backgroundMusic);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();

    }

    void Start()
    {

        DontDestroyOnLoad(gameObject);
    }

    public void setBackgroundMusic(AudioClip clip)
    {

        sources[BACKGROUND_MUSIC_INDEX].clip = clip;
    }

    public void playBackgroundMusic()
    {

        playBackgroundMusicClientRpc();
    }

    [ClientRpc]
    public void playBackgroundMusicClientRpc()
    {

        sources[BACKGROUND_MUSIC_INDEX].Play();
    }

    public void playIntro()
    {

        playIntroClientRpc();
    }

    [ClientRpc]
    public void playIntroClientRpc()
    {

        AudioSource freeSource = null;
        foreach (AudioSource source in sources)
        {
            if (source.isPlaying || source == sources[BACKGROUND_MUSIC_INDEX]) continue;

            source.clip = intro;
            source.Play();
            freeSource = source;
            break;
        }

        Invoke("playIntroCoroutine", intro.length - delay);
    }

    private void playIntroCoroutine()
    {


        sources[BACKGROUND_MUSIC_INDEX].Play();
    }


    public void stopBackgroundMusic()
    {

        sources[BACKGROUND_MUSIC_INDEX].Play();
    }

    [ClientRpc]
    public void stopBackgroundMusicClientRpc()
    {

        sources[BACKGROUND_MUSIC_INDEX].Stop();
    }
    public void playClip(AudioClip clip)
    {

        foreach (AudioSource source in sources)
        {
            if (source.isPlaying || source == sources[BACKGROUND_MUSIC_INDEX]) continue;

            source.clip = clip;
            source.Play();
            break;
        }
    }


    public void stopAllClips()
    {

        stopAllClipsClientRpc();
    }

    [ClientRpc]
    public void stopAllClipsClientRpc()
    {

        foreach (AudioSource source in sources)
        {

            source.Stop();
        }
    }
}
