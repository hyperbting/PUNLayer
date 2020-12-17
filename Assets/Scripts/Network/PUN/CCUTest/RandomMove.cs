using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{

    public bool isOwner = false;
    public bool lerpToTarget = false;
    public Animator animator;

    [Header("Debug")]
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    [SerializeField] Transform myMesh;

    private void Start()
    {
        InvokeRepeating("HostMove", 1, 5);
    }

    #region mono
    private void FixedUpdate()
    {
        if (isOwner || lerpToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            myMesh.rotation = Quaternion.Lerp(myMesh.rotation, targetRotation, Time.deltaTime);
        }
    }
    #endregion

    private void HostMove()
    {
        if (isOwner)
        {
            ////Random choose target location
            var nextDelta = new Vector3(Random.Range(-5, 5), 0, Random.Range(-4, 4));
            targetPosition = nextDelta;

            targetRotation = Quaternion.Euler(Vector3.up * Random.Range(-90, 90));

            if (animator != null)
                HostAnimatorSet();
        }
    }

    void HostAnimatorSet()
    {
        if (animator == null)
            return;

        var casee = Random.Range(0, 5);
        switch (casee)
        {
            case 0:
                animator.SetTrigger("RollForward");

                animator.SetBool("Bool01", true);
                animator.SetBool("Bool02", false);
                animator.SetBool("Bool03", false);
                break;
            case 1:
                animator.SetBool("Bool01", false);
                animator.SetBool("Bool02", true);
                animator.SetBool("Bool03", false);
                break;
            case 2:
                animator.SetBool("Bool01", false);
                animator.SetBool("Bool02", false);
                animator.SetBool("Bool03", true);
                break;
            default:
                animator.SetTrigger("Idle");
                break;
        }
    }
}
