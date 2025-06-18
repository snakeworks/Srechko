using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private SoundDefinition[] _soundDefinitions;

    private readonly Dictionary<string, SoundDefinition> _definitionsDict = new();

    protected override void Init()
    {
        foreach (var def in _soundDefinitions)
        {
            GameObject audioSourceObj = new();
            AudioSource source = audioSourceObj.AddComponent<AudioSource>();
            audioSourceObj.transform.SetParent(transform);
            audioSourceObj.name = def.Name.ToString();
            def.Init(source);
            _definitionsDict.Add(def.Name, def);
        }
    }

    public void Play(string soundName, float delay = 0.0f)
    {
        if (_definitionsDict.TryGetValue(soundName, out var definition))
        {
            definition.Play(delay);
        }
    }

    public bool IsPlaying(string soundName)
    {
        if (_definitionsDict.TryGetValue(soundName, out var def)) return def.IsPlaying();
        return false;
    }

    public void Stop(string soundName)
    {
        if (_definitionsDict.TryGetValue(soundName, out var def))
        {
            def.Stop();
        }
    }
}
