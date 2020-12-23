
using UnityEngine;

public class ControllerLocationDebug : MonoBehaviour
{
    [SerializeField] GameObject leftConDebugIndicator;
    [SerializeField] GameObject rightConDebugIndicator;

    [SerializeField] HeadDetectable leftCon;
    [SerializeField] HeadDetectable rightCon;

    private void OnEnable()
    {
        leftCon.OnRelativePositionChanged += FollowControllerLeft;
        rightCon.OnRelativePositionChanged += FollowControllerRight;
    }

    private void OnDisable()
    {
        leftCon.OnRelativePositionChanged -= FollowControllerLeft;
        rightCon.OnRelativePositionChanged -= FollowControllerRight;
    }

    public void FollowControllerLeft(bool conEnabled, Vector3 conPos)
    {
        leftConDebugIndicator.SetActive(conEnabled);
        leftConDebugIndicator.transform.localPosition = new Vector3(conPos.x,conPos.z,0) * 15;
    }

    public void FollowControllerRight(bool conEnabled, Vector3 conPos)
    {
        rightConDebugIndicator.SetActive(conEnabled);
        rightConDebugIndicator.transform.localPosition = new Vector3(conPos.x, conPos.z, 0) * 15;
    }
}
