using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ModalMenu : Menu
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    public static bool IsModalCurrent => _instance.IsCurrent;

    private static ModalMenu _instance;
    private static InputSystemUIInputModule UIInputModule => EventSystem.current.currentInputModule as InputSystemUIInputModule;

    protected override void Init()
    {
        _instance = this;
        transform.DOLocalMoveY(-60.0f, 0.0f);
        _canvasGroup.alpha = 0.0f;
    }

    private static bool Push(string message)
    {
        if (MenuNavigator.IsBufferingMenuOperations || _instance.IsTweening)
        {
            return false;
        }

        if (!IsModalCurrent)
        {
            MenuNavigator.Push(_instance);
        }
        ResetControls();
        _instance._messageText.SetText(message);
        return true;
    }

    public static void ForcePop()
    {
        if (IsModalCurrent)
        {
            _instance.CanPop = true;
            MenuNavigator.Pop();
            _instance.CanPop = false;
        }
    }

    public static async Task<Result> PushOk(string message)
    {
        if (!Push(message))
        {
            return Result.Error;
        }

        static void OnCancel(InputAction.CallbackContext context) => _instance._okButton.onClick.Invoke();

        _instance._okButton.gameObject.SetActive(true);
        _instance.LastSelectedObject = _instance._okButton.gameObject;
        UIInputModule.cancel.action.performed += OnCancel;

        await _instance._okButton.onClick;
        
        UIInputModule.cancel.action.performed -= OnCancel;
        return Result.Ok;
    }

    public static async Task<Result> PushYesNo(string message)
    {
        if (!Push(message))
        {
            return Result.Error;
        }

        _instance._yesButton.gameObject.SetActive(true);
        _instance._noButton.gameObject.SetActive(true);

        _instance.LastSelectedObject = _instance._yesButton.gameObject;

        var yesClicked = false;
        var noClicked = false;

        void OnYesClicked() => yesClicked = true;
        void OnNoClicked() => noClicked = true;
        void OnCancel(InputAction.CallbackContext context) => OnNoClicked();

        _instance._yesButton.onClick.AddListener(OnYesClicked);
        _instance._noButton.onClick.AddListener(OnNoClicked);
        UIInputModule.cancel.action.performed += OnCancel;

        try
        {
            while (!yesClicked && !noClicked)
            {
                await Task.Yield();
            }

            UIInputModule.cancel.action.performed -= OnCancel;
            return yesClicked ? Result.Yes : Result.No;
        }
        finally
        {
            _instance._yesButton.onClick.RemoveListener(OnYesClicked);
            _instance._noButton.onClick.RemoveListener(OnNoClicked);
        }
    }

    private static void ResetControls()
    {
        _instance._okButton.gameObject.SetActive(false);
        _instance._yesButton.gameObject.SetActive(false);
        _instance._noButton.gameObject.SetActive(false);
    }

    public override void TweenOpen(Sequence sequence)
    {
        sequence.Append(transform.DOLocalMoveY(0.0f, 0.15f));
        sequence.Insert(0.0f, _canvasGroup.DOFade(1.0f, 0.15f));
    }

    public override void TweenClose(Sequence sequence)
    {
        sequence.Append(transform.DOLocalMoveY(-60.0f, 0.15f));
        sequence.Insert(0.0f, _canvasGroup.DOFade(0.0f, 0.15f));
    }

    public enum Result
    {
        Ok,
        Yes,
        No,
        Error
    }
}
