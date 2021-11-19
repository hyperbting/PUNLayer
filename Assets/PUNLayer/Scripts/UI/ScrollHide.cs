using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollHide : MonoBehaviour
{
    public RectTransform target;
    public float showHeight;
    public float hideHeight;

    public void Show(bool enable)
    {
        var ori = target.sizeDelta;
        if(enable)
            ori = new Vector2(ori.x, showHeight);
        else
            ori = new Vector2(ori.x, hideHeight);
        target.sizeDelta = ori;
    }
}
