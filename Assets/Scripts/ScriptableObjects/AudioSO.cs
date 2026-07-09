using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    None = 0,
    [AudioCategory("Player")] Axe = 1,
    //[AudioCategory("Player")] AxeHit = 2,
    [AudioCategory("Player")] PlayerDamage = 3,
    [AudioCategory("Player")] PlayerDeath = 4,
    [AudioCategory("Player")] Footstep = 5,
    [AudioCategory("Player")] FullBag = 6,
    [AudioCategory("Trees")] TreeStart = 10,
    [AudioCategory("Trees")] TreeFall = 11,
    [AudioCategory("Trees")] TreeBurn = 12,
    [AudioCategory("Trees")] TreeHit = 13,
    //[AudioCategory("Logs")] LogBurn = 21,
    [AudioCategory("Logs")] LogTaken = 22,
    [AudioCategory("Logs")] LogDeposited = 23,
    [AudioCategory("Furnace")] FurnaceMoving = 31,
    [AudioCategory("Furnace")] FurnaceEating = 32,
    //[AudioCategory("Furnace")] FurnaceLagBurn = 33,
    [AudioCategory("Furnace")] FurnaceSwitch = 34,
    [AudioCategory("Coins")] CoinsDroped = 41,
    [AudioCategory("Coins")] CoinsTaken = 42,
    [AudioCategory("Coins")] CoinsDeposited = 43,
    [AudioCategory("Gate")] GateOpen = 51,
    [AudioCategory("Gate")] GateTakeCoins = 52,
    [AudioCategory("Ambient")] Ambient = 61,
}

public enum AudioPlayMode
{
    Single,
    Random
}

[CreateAssetMenu(fileName = "AudioConfig", menuName = "ScriptableObjects/AudioConfig", order = 1)]
public class AudioSO : ScriptableObject
{
    public AudioData[] audioData;
    [Range(0, 1)]
    public float masterVolume = 1f;

    private Dictionary<AudioType, AudioData> cache;

    void OnEnable()
    {
        cache = new();

        foreach (var data in audioData)
            cache[data.type] = data;
    }

    public AudioData GetClip(AudioType type) => cache[type];
}

[Serializable]
public class AudioData
{
    public AudioType type;

    public AudioPlayMode playMode = AudioPlayMode.Single;

    public AudioClip clip;      // single
    public AudioClip[] clips;   // random

    public bool loop;
    public bool sfx3D;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(-3f, 3f)]
    public float pitch = 1f;

    public bool mute;

    public AudioClip GetClip()
    {
        if (mute) return null;

        if (playMode == AudioPlayMode.Random && clips != null && clips.Length > 0)
            return clips[UnityEngine.Random.Range(0, clips.Length)];

        return clip;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class AudioCategoryAttribute : Attribute
{
    public string Name;

    public AudioCategoryAttribute(string name)
    {
        Name = name;
    }
}