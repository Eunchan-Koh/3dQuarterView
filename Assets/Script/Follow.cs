using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    void FixedUpdate(){
        transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.1f);
    }
}
