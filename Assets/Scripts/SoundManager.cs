using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
    public List<AudioClip> clipList;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private List<NamedAudioClip> audioClips;

    private Dictionary<string, NamedAudioClip> audioClipsDict;

    [SerializeField] private AudioSource soundObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Convert list to dictionary for fast lookup
            audioClipsDict = new Dictionary<string, NamedAudioClip>();
            foreach (var namedClip in audioClips)
            {
                if (!audioClipsDict.ContainsKey(namedClip.name))
                {
                    audioClipsDict.Add(namedClip.name, namedClip);
                }
                else
                {
                    Debug.LogWarning("Duplicate audio clip name: " + namedClip.name);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySound(string name) => PlaySound(name, 1);

    /// <summary>
    /// Plays an audio clip by name with the specified volume.
    /// </summary>
    /// <param name="name">Name of the audio clip to play</param>
    public void PlaySound(string name, float volume)
    {
        if (audioClipsDict.TryGetValue(name, out NamedAudioClip namedClip))
        {
            AudioClip clip = namedClip.clip;
            if (clip != null)
            {
                PlaySoundClip(clip, transform, volume);
            }
            else
            {
                Debug.LogWarning("Clip is null for: " + name);
            }
        }
        else
        {
            Debug.LogWarning("NamedAudioClip not found: " + name);
        }
    }

    /// <summary>
    /// Stops all instances of the audio clip associated with the specified NamedAudioClip name if they are playing.
    /// </summary>
    /// <param name="name">Name of the NamedAudioClip whose clip should be stopped</param>
    public void StopSound(string name)
    {
        if (audioClipsDict.TryGetValue(name, out NamedAudioClip namedClip))
        {
            AudioClip clipToStop = namedClip.clip;
            if (clipToStop != null)
            {
                foreach (Transform child in transform)
                {
                    AudioSource source = child.GetComponent<AudioSource>();
                    if (source != null && source.clip == clipToStop && source.isPlaying)
                    {
                        source.Stop();
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnPosition, float volume)
    {
        AudioSource audioSource = Instantiate(soundObject, spawnPosition.position, Quaternion.identity, transform);
        // AudioSource audioSource = newSoundObj.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource, audioClip.length);
        }
    }

    /// <summary>
    /// Plays a random audio clip if the list of the NamedAudioClip is not empty 
    /// </summary>
    /// <param name="name">Name of the NamedAudioClip to get the list of clips from</param>
    public void PlayRandomSoundClip(string name, float volume)
    {
        if (audioClipsDict.TryGetValue(name, out NamedAudioClip namedClip))
        {
            if (namedClip.clipList != null && namedClip.clipList.Count > 0)
            {
                int randomIndex = Random.Range(0, namedClip.clipList.Count);
                AudioClip clip = namedClip.clipList[randomIndex];
                PlaySoundClip(clip, transform, volume);
            }
            else
            {
                Debug.LogWarning("No clips in the list for: " + name);
            }
        }
        else
        {
            Debug.LogWarning("NamedAudioClip not found: " + name);
        }
    }
}