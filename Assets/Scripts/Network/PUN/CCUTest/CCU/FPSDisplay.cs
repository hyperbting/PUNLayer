using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float msec = 0.0f;
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    float CalculateFPS()
    {
        msec = deltaTime * 1000.0f;
        return 1.0f / deltaTime;
    }

    string ContentFPS()
    {
        return string.Format("{0:0.0} ms ({1:0.} fps)", msec, CalculateFPS());
    }

    [SerializeField] Text tOnSelf;
    private void FixedUpdate()
    {
        if (tOnSelf != null)
            tOnSelf.text = ContentFPS();
    }

    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 100;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

    //    string text = ContentFPS();
    //    GUI.Label(rect, text, style);
    //}
}
