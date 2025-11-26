using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioType 
{
    [HideInInspector]
    public AudioSource Source;
    public AudioClip Clip;
    public AudioMixerGroup Group;

    public string Name;

    [Range(0f, 1f)]
    public float Volume = 1f;
    [Range(0.1f, 5f)]
    public float Pitch = 1f;
    public bool Loop;

    [HideInInspector] public List<AudioSource> Sources = new (); // 多音频源池
    [Tooltip("该音效同时可播放的最大数量（音频源池大小）")]
    public int PoolSize = 30; // 默认30个并行源
}
