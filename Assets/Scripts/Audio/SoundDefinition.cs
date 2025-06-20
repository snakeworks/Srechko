using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class SoundDefinition
{
    [SerializeField] public string Name;
    [SerializeField] public AudioClip Clip;
    [SerializeField] public bool Loop = false;
    [SerializeField, Range(0f, 1f)] public float Volume = 1;
    [SerializeField, Range(0.2f, 1.5f)] public float Pitch = 1;
    [SerializeField, Range(0f, 0.5f)] public float RandomVolume;
    [SerializeField, Range(0f, 0.5f)] public float RandomPitch;

    private AudioSource _source;

    public void Init(AudioSource source)
    {
        _source = source;
        _source.playOnAwake = false;
        _source.clip = Clip;
        _source.loop = Loop;
    }

    public void Play(float delay = 0.0f)
    {
        _source.volume = Volume * (1 + Random.Range(-RandomVolume / 2f, RandomVolume / 2));
        _source.pitch = Pitch * (1 + Random.Range(-RandomPitch / 2f, RandomPitch / 2));
        _source.PlayDelayed(delay);
    }

    public bool IsPlaying()
    {
        return _source.isPlaying;
    }

    public void Stop()
    {
        _source.Stop();
    }

    public void FadeOut(float duration)
    {
        _source.DOFade(0.0f, duration).OnComplete(Stop);
    }
}
