using DG.Tweening;
using UnityEngine;

public class BoardTurnOrderGUI : MonoBehaviour
{
    [SerializeField] private BoardPlayerTurnCard _turnCardPrefab;
    [SerializeField] private Transform _creationParent;

    private void Start()
    {
        BoardManager.Instance.OnTurnOrderSet += OnTurnOrderSet;
        BoardManager.Instance.OnGameEnd += () =>
        {
            _creationParent
                .GetComponent<RectTransform>()
                .DOAnchorPosY(-90.0f, 0.25f);
        };
    }

    private void OnDestroy()
    {
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.OnTurnOrderSet -= OnTurnOrderSet;
        }
    }

    private void OnTurnOrderSet()
    {
        var rectTransform = _creationParent.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(
            rectTransform.anchoredPosition.x,
            -90.0f
        );
        foreach (var index in BoardManager.Instance.BoardPlayerIndexOrder)
        {
            var playerData = GameManager.Instance.GetBoardPlayerData(index);
            var gameObject = Instantiate(_turnCardPrefab, _creationParent);
            gameObject.GetComponent<BoardPlayerTurnCard>().Setup(playerData);
        }
        rectTransform.DOAnchorPosY(70.0f, 0.25f);
    }
}
