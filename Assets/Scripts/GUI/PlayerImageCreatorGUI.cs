using UnityEngine;
using UnityEngine.UI;

public class PlayerImageCreatorGUI : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var obj = Instantiate(_playerPrefab);
            obj.GetComponentInChildren<Image>().color = PlayerManager.Instance.GetPlayerProfile(i).Color;
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.name = $"PlayerImage{i}";
        }
    }
}
