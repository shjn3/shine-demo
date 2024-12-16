using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    public Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        SoundManager.Play(SoundKey.BUTTON_CLICK, 1);
    }

    public void SetDisable(bool isDisabled)
    {
        if (button != null) button.interactable = !isDisabled;
    }
}