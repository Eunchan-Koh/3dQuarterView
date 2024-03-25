using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();//sphere collider가 두개 이상일 시, inspector창에서 가장 위에잇는 sphere collider를 가져옴
    }

    void Update(){
        transform.Rotate(Vector3.up*30*Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Floor"){
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }

}
