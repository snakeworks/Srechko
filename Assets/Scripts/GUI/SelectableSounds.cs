using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableSounds : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    [SerializeField] private bool _isNegative = false;

    public void OnSelect(BaseEventData eventData)
    {
        AudioManager.Instance.Play(SoundName.UISelect);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.Instance.Play(!_isNegative ? SoundName.UIPositive : SoundName.UINegative);
    }
}
