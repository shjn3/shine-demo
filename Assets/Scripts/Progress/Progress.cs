using DG.Tweening;
using UnityEngine;

public class Progress : MonoBehaviour
{
    public RectTransform sliderRect;
    private float m_value = 0;
    private float prevValue = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void UpdateSlider()
    {
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = new Vector2(m_value, 1);
    }

    public void SetValue(float v)
    {
        prevValue = v;
        m_value = v;
        UpdateSlider();
    }

    Tween progressTween;
    public Shine.Promise.Promise RunProgress(float end, float delay = 0)
    {
        return new Shine.Promise.Promise((resolve) =>
        {
            float validateEndValue = Mathf.Clamp01(end);
            progressTween?.Kill();

            prevValue = m_value;

            progressTween = DOVirtual.Float(prevValue, validateEndValue, 0.5f * (validateEndValue - prevValue), (tweenValue) =>
            {
                SetValue(tweenValue);
            }).OnComplete(() =>
            {
                SetValue(validateEndValue);
                resolve();
            }).SetDelay(delay).SetEase(Ease.Linear);
        });
    }
}
