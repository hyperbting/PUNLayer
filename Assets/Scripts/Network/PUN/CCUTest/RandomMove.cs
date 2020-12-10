using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{

    public bool isOwner = false;
    public bool usingSerializeView = false;

    [Header("Debug")]
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    [SerializeField] Transform myMesh;

    private void Start()
    {
        InvokeRepeating("HostMove", 1, 5);
    }

    private void HostMove()
    {
        if (isOwner)
        {
            ////Random choose target location
            var nextDelta = new Vector3(Random.Range(-5, 5), 0, Random.Range(-4, 4));
            targetPosition = nextDelta;

            targetRotation = Quaternion.Euler(Vector3.up * Random.Range(-90, 90));
        }
    }

    private void FixedUpdate()
    {
        if (isOwner || usingSerializeView)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            myMesh.rotation = Quaternion.Lerp(myMesh.rotation, targetRotation, Time.deltaTime);
        }
    }


}
