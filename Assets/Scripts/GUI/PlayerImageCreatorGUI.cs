using UnityEngine;
using UnityEngine.UI;

public class PlayerImageCreatorGUI : MonoBehaviour
{
    [SerializeField] private Image _playerPrefab;

    private void Awake()
    {
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var obj = Instantiate(_playerPrefab).GetComponent<Image>();
            obj.color = PlayerManager.Instance.GetPlayerProfile(i).Color;
            obj.transform.SetParent(transform);
        }
    }
}
