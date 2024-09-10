using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Players")]
    [SerializeField] AudioSource audioPlayerPrefab;
    List<AudioSource> audioPlayers = new List<AudioSource>();

    [Header("Audio Clips")]
    [SerializeField] public AudioClip menuSFX;
    [Range(0f, 1f)] [SerializeField] public float menuSFXVolume = 1f;
    [SerializeField] public AudioClip footstepSFX;
    [Range(0f, 1f)] [SerializeField] public float footstepSFXVolume = 1f;

    public static AudioManager instance;

    void Awake()
    {
        ManageSingleton();
    }  

    void Start()
    {
        transform.position = Camera.main.transform.position;        
    }  
    
    public void PlayClip(AudioClip clip, float volume, bool loop = false)
    {
        var result = CheckExistingAudioPlayers(clip);
        int index;
        if (result.audioPlayerExists)
        {
            index = result.index;
            audioPlayers[index].volume = volume;
            audioPlayers[index].Play(); 
        }
        else
        {
            audioPlayers.Add(Instantiate(audioPlayerPrefab, this.transform));
            index = audioPlayers.Count - 1;
            audioPlayers[index].clip = clip;
            audioPlayers[index].volume = volume;
            audioPlayers[index].loop = loop;
            audioPlayers[index].Play();
        }
    }

    public void PlayClipFromList(List<AudioClip> listOfClips, List<float> listOfClipVolumes, bool loop = false)
    {
        int randomClipIndex = Random.Range(0, listOfClips.Count);
        AudioClip clip = listOfClips[randomClipIndex];

        var result = CheckExistingAudioPlayers(clip);
        int index;
        if (result.audioPlayerExists)
        {
            index = result.index;
            audioPlayers[index].volume = listOfClipVolumes[randomClipIndex];
            audioPlayers[index].Play(); 
        }
        else
        {
            audioPlayers.Add(Instantiate(audioPlayerPrefab, this.transform));
            index = audioPlayers.Count - 1;
            audioPlayers[index].clip = clip;
            audioPlayers[index].volume = listOfClipVolumes[randomClipIndex];
            audioPlayers[index].loop = loop;
            audioPlayers[index].Play();
        }
    }

    public void StopClip(AudioClip clip)
    {
        var result = CheckExistingAudioPlayers(clip);
        int index = result.index;
        if(result.audioPlayerExists)
        {
            audioPlayers[index].Stop();
        }
    }    

    public void PauseClip(AudioClip clip)
    {
        var result = CheckExistingAudioPlayers(clip);
        int index = result.index;
        if(result.audioPlayerExists)
        {
            audioPlayers[index].Pause();
        }
    }

    public AudioSource GetAudioPlayer(AudioClip clip)
    {
        var result = CheckExistingAudioPlayers(clip);
        int index = result.index;
        if (result.audioPlayerExists)
        {
            return audioPlayers[index];
        }
        else
        {
            return null;
        }
    }

    (bool audioPlayerExists, int index) CheckExistingAudioPlayers(AudioClip clip)
    {
        bool audioPlayerExists = false;
        int index = -1;
        for (int i = 0; i < audioPlayers.Count; i++)
        {
            if (audioPlayers[i].clip == clip)
            {
                audioPlayerExists = true;
                index = i;
            }
        }
        return (audioPlayerExists, index);
    }

    void ManageSingleton()
    {
        if(instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);            
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
