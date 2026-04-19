using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptImage : MonoBehaviour
{
    [SerializeField] private Sprite _joystickSprite;
    [SerializeField] private Sprite _sdSprite;
    
    private Image _image;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _image = GetComponent<Image>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (!PlayerManager.Instance.IsSingleDeviceMode)
        {
            if (_image != null) _image.sprite = _joystickSprite;
            if (_spriteRenderer != null) _spriteRenderer.sprite = _joystickSprite;
        }
        else
        {
            if (_image != null) _image.sprite = _sdSprite;
            if (_spriteRenderer != null) _spriteRenderer.sprite = _sdSprite;
        }
    }
}
