using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardPlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visuals;
    [SerializeField] private Transform _cameraView;
    [SerializeField] private Canvas _diceCanvas;
    [SerializeField] private CanvasGroup _diceCanvasGroup;
    [SerializeField] private TextMeshProUGUI _diceNumberText;

    public int StandingOnBoardSpaceId { get; private set; } = -1;
    public Transform CameraView => _cameraView;
    public int Index => transform.GetSiblingIndex();
    public int LastRolledDiceNumber { get; private set; }

    private WaitForSeconds _numberGenerationDelay = new(0.02f);
    private Coroutine _generateRandomNumberCoroutine;

    private void Awake()
    {
        var profile = PlayerManager.Instance.GetPlayerProfile(Index);
        _visuals.material.SetColor("SpriteColor", profile.Color);
        _diceCanvas.gameObject.SetActive(false);
        _diceCanvasGroup.alpha = 0.0f;

    }

    public void ShowDiceRolling()
    {
        _diceCanvas.gameObject.SetActive(true);
        _diceCanvasGroup.DOFade(1.0f, 0.15f);
        
        _diceNumberText.SetText(Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber).ToString());
        _generateRandomNumberCoroutine = StartCoroutine(GenerateRandomNumber());
        IEnumerator GenerateRandomNumber()
        {
            while (true)
            {
                yield return _numberGenerationDelay;
                _diceNumberText.SetText(Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber).ToString());
            }
        }
    }

    public void FinishRollingDice(int numberRolled, bool hideDice = true)
    {
        LastRolledDiceNumber = numberRolled;
        StopCoroutine(_generateRandomNumberCoroutine);
        _diceNumberText.SetText(numberRolled.ToString());

        if (hideDice)
        {
            HideDice();
        }
    }

    public void HideDice()
    {
        _diceCanvasGroup.DOFade(0.0f, 0.15f).SetDelay(1.0f).OnComplete(() => _diceCanvas.gameObject.SetActive(false));
    }

    public void MoveToSpace(BoardSpace space, System.Action onComplete)
    {
        StandingOnBoardSpaceId = space.Id;
        transform.DOMoveX(space.transform.position.x, 0.25f);
        transform.DOMoveZ(space.transform.position.z, 0.25f).OnComplete(() => onComplete());
    }
}
