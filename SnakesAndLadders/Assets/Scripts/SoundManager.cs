using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }


    [SerializeField] AudioClip[] clips;
    AudioSource source;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(int index)
    {
        source.pitch = Random.Range(0.9f, 1.05f);
        source.PlayOneShot(clips[index]);
    }
}
