using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableSounds : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    [SerializeField] private bool _isNegative = false;
    [SerializeField] private bool _muteSelect = false;
    [SerializeField] private bool _muteSubmit = false;

    public void OnSelect(BaseEventData eventData)
    {
        if (_muteSelect) return;
        AudioManager.Instance.Play(SoundName.UISelect);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (_muteSubmit) return;
        AudioManager.Instance.Play(!_isNegative ? SoundName.UIPositive : SoundName.UINegative);
    }
}
