using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }

    public void PlaySFXAtPosition(AudioClip ac, Vector3 worldPos, float maxDistance = 10f)
    {
        GameObject temp = new GameObject("SFX_" + ac.name);
        temp.transform.position = worldPos;

        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = ac;

        src.spatialBlend = 1f;
        src.minDistance = 2f;
        src.maxDistance = maxDistance;
        src.rolloffMode = AudioRolloffMode.Linear;

        src.Play();

        Destroy(temp, ac.length + 0.2f);
    }
}
