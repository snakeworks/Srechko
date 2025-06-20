using System.Collections.Generic;
using System.Threading.Tasks;

public class EndState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();

        var results = new List<PlayerResult>();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var data = GameManager.Instance.GetBoardPlayerData(i);
            var result = new PlayerResult()
            {
                Index = data.Index,
                Coins = data.CoinCount,
            };
            results.Add(result);
        }
        results.Sort((a, b) => b.Coins.CompareTo(a.Coins));
        for (int i = 0; i < results.Count; i++) results[i].Place = i;

        PlayerManager.Instance.SetResults(results);

        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview);
        AudioManager.Instance.Play(SoundName.GameEnd);
        await BoardGUIAnimations.Instance.PlayPopupAnimation("GAME FINISHED!");
        await Task.Delay(800);
        
        SceneLoader.Load(Scene.Results, showLoadingScreen: false);
    }

    public override void OnExit()
    {
    }
}