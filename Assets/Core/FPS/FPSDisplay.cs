using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    string fps = $"60 fps";
    GUIStyle style = new GUIStyle();
    Rect rect;
    private WaitForSecondsRealtime waitForFrequency;
    private float waitTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = Screen.height * 2 / 100;
        style.normal.textColor = Color.black;
        style.normal.background = Texture2D.whiteTexture;

        rect = new Rect(0, 5, Screen.width / 6, style.fontSize);
        waitForFrequency = new WaitForSecondsRealtime(waitTime);

        StartCoroutine(FPS());
    }

    void OnGUI()
    {
        GUI.Label(rect, fps, style);
    }


    private IEnumerator FPS()
    {
        while (true)
        {
            fps = string.Format("FPS: {0}", Mathf.RoundToInt(1 / Time.smoothDeltaTime));
            yield return waitForFrequency;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        // float fps = 1 / Time.smoothDeltaTime;
        // fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}
