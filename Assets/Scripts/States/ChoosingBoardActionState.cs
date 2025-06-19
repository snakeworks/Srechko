using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ChoosingBoardActionState : GameState
{
    public override async void OnEnter()
    {
        await BoardCamera.Instance.TransitionToPlayer(CurrentBoardPlayerController.Index);
        
        BoardManager.Instance.BoardActionMenu.ResetLastSelectedObject();
        BoardManager.Instance.BoardActionMenu.ItemMenu.ResetLastSelectedObject();

        await BoardGUIAnimations.Instance.AnimatePlayerTurn();

        if (BoardManager.Instance.CurrentPlayer.SkipNextTurn)
        {
            CurrentBoardPlayerController.SetSkipNextTurn(false);
            ChangeState(NextTurnState);
            return;
        }

        await Task.Delay(250);

        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.GiveOwnershipTo(CurrentController);

        BoardManager.Instance.BoardActionMenu.OnDiceRollPressed += OnDiceRollPressed;
        BoardManager.Instance.BoardActionMenu.ItemMenu.OnItemPressed += OnItemPressed;
        BoardManager.Instance.BoardActionMenu.OnViewBoardPressed += OnViewBoardPressed;
        BoardManager.Instance.BoardActionMenu.Push();
    }

    private void ResetListeners()
    {
        PlayerManager.Instance.DisableInput();
        BoardManager.Instance.BoardActionMenu.OnDiceRollPressed -= OnDiceRollPressed;
        BoardManager.Instance.BoardActionMenu.ItemMenu.OnItemPressed -= OnItemPressed;
        BoardManager.Instance.BoardActionMenu.OnViewBoardPressed -= OnViewBoardPressed;
    }

    private async void OnDiceRollPressed()
    {
        ResetListeners();
        BoardManager.Instance.BoardActionMenu.ForcePop();
        await Task.Delay(100);
        ChangeState(RollingDiceState);
    }

    private async void OnItemPressed()
    {
        ResetListeners();
        MenuNavigator.ForcePopUntilEmpty();
        await Task.Delay(100);
        ChangeState(UseItemState);
    }

    private async void OnViewBoardPressed()
    {
        BoardCameraTransforms.BoardView.transform.position = new(
            CurrentBoardPlayerController.transform.position.x,
            BoardCameraTransforms.BoardView.transform.position.y,
            CurrentBoardPlayerController.transform.position.z
        );

        MenuNavigator.ForcePop();
        
        PlayerManager.Instance.DisableInput();
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.BoardView, CameraTransition.Move);
        PlayerManager.Instance.EnableInput();
        
        Vector2 lastDirection = Vector2.zero;
        float cameraSpeed = 5.0f;
        Coroutine updateCoroutine = BoardManager.Instance.StartCoroutine(CustomUpdate());

        CurrentController.MovePerformed += OnMove;

        // TODO: Replace with CurrentController.CancelPerformed later when Unity stops bitching
        // about some error. Doing this EventSystem input module workaround for now.
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnCancel;

        IEnumerator CustomUpdate()
        {
            while (true)
            {
                BoardCameraTransforms.BoardView.transform.position += cameraSpeed * Time.deltaTime * new Vector3(lastDirection.x, 0, lastDirection.y).normalized;
                yield return null;
            }
        }

        void OnMove(Vector2 direction)
        {
            lastDirection = direction;
        }

        async void OnCancel(InputAction.CallbackContext context)
        {
            CurrentController.MovePerformed -= OnMove;
            (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnCancel;
            AudioManager.Instance.Play(SoundName.UINegative);

            BoardManager.Instance.StopCoroutine(updateCoroutine);

            PlayerManager.Instance.DisableInput();
            await BoardCamera.Instance.TransitionToPlayer(CurrentBoardPlayerController.Index);
            PlayerManager.Instance.EnableInput();
            BoardManager.Instance.BoardActionMenu.Push();
        }
    }

    public override void OnExit()
    {
    }
}