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

    public void SetDisable()
    {
        if (button != null) button.interactable = false;

    }

    public void SetEnable()
    {
        if (button != null) button.interactable = true;
    }
}