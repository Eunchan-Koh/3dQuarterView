using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Halo : MonoBehaviour
{
    public GameObject playerHead;
    public GameObject actualHeadforPos;
    public GameObject innerHaloForAngle;
    Vector3 haloPos;
    Quaternion haloRotation;
    public float haloMoveSpeed;
    public float haloRotationSpeed;
    // public Quaternion tempQuat;
    public Vector3 angleVec;
    public float above;//radius
    public Vector3 offset;
    Vector3 calculatedOffset;
    public Vector3 innerAngle;

    float x, y;
    Vector3 newVec;
    void FixedUpdate(){
        // moveBack = new Vector3(0, 0, 2*Mathf.PI*above*angleVec.x/360);
        x = above * Mathf.Sin(angleVec.x*Mathf.PI/180);
        y = above * Mathf.Cos(angleVec.x*Mathf.PI/180);
        // newVec = new Vector3(0, y, x);
        newVec =  y*actualHeadforPos.transform.up + x*actualHeadforPos.transform.forward;
        calculatedOffset = offset.y* actualHeadforPos.transform.up + offset.x*actualHeadforPos.transform.forward;
        // haloPos = offset.y*actualHeadforPos.transform.up + ( offset.y*actualHeadforPos.transform.up +newVec+actualHeadforPos.transform.up)*above;
        
        haloRotation = Quaternion.Euler(angleVec);
        
        // transform.position = Vector3.Lerp(transform.position, actualHeadforPos.transform.position + haloPos, haloMoveSpeed*Time.fixedDeltaTime);
        // transform.rotation = Quaternion.Lerp(transform.rotation, playerHead.transform.rotation*haloRotation, haloRotationSpeed*Time.fixedDeltaTime);


        transform.position = Vector3.Lerp(transform.position, actualHeadforPos.transform.position + newVec + calculatedOffset, haloMoveSpeed*Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerHead.transform.rotation * haloRotation, haloRotationSpeed*Time.fixedDeltaTime);
        innerHaloForAngle.transform.localRotation = Quaternion.Euler(innerAngle);
    }
}
