using System.Threading.Tasks;
using UnityEngine;

public class ArmudgeddonWorldEvent : WorldEvent
{
    private const int _spaceCount = 5;

    public override async Task Apply()
    {
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview, CameraTransition.Move);
        await Task.Delay(100);

        int availableSpaceCount = 0;
        for (int i = 0; i < BoardSpace.Count; i++)
        {
            if (!BoardSpace.Get(i).IsMudCovered) availableSpaceCount++;
        }

        int spacesToCover = Mathf.Min(_spaceCount, availableSpaceCount); 

        for (int i = 0; i < spacesToCover; i++)
        {
            var boardSpace = BoardSpace.GetRandom();
            while (boardSpace.IsMudCovered)
            {
                boardSpace = BoardSpace.GetRandom();
            }
            boardSpace.SetMudCovered(true, 5);
        }

        AudioManager.Instance.Play(SoundName.MudBomb);

        await Task.Delay(1300);
    }
}
