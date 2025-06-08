using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MiniGameManager : StaticInstance<MiniGameManager>
{
    [SerializeField] private GameObject _screenObject;
    [SerializeField] private MiniGame[] _games;

    public event System.Action Finished;

    private MiniGame _current;

    protected override void Awake()
    {
        base.Awake();
        _screenObject.SetActive(false);
        _screenObject.transform.localScale = Vector3.zero;
        foreach (var game in _games) game.gameObject.SetActive(false);
    }

    public async void BeginRandom()
    {
        _current = _games[Random.Range(0, _games.Length)];
        _screenObject.SetActive(true);
        _current.gameObject.SetActive(true);
        _current.OnCalled();

        await _screenObject.transform.DOScale(1.0f, 0.6f)
            .SetEase(Ease.OutCirc)
            .AsyncWaitForCompletion();

        _current.OnBegin();
    }

    public async void End()
    {
        if (_current == null) return;

        await Task.Delay(100);
        await _screenObject.transform.DOScale(0.0f, 0.6f)
            .SetEase(Ease.OutCirc)
            .AsyncWaitForCompletion();

        _screenObject.SetActive(false);
        _current.gameObject.SetActive(false);
        _current = null;

        Finished?.Invoke();
    }
}