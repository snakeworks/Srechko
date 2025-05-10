using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MenuStackDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string menuText = "";
        foreach (var menu in MenuNavigator.GetStack())
        {
            menuText += $"{menu.name} {(menu == MenuNavigator.CurrentMenu ? "(CURRENT)" : "")}\n";
        }
        _text.SetText(menuText);
    }
}
