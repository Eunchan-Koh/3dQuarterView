using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;
    public float attackRate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    

    public void Use(){
        if(type == Type.Melee){
            // Debug.Log("in use");
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }else if(type == Type.Range && curAmmo > 0){
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing(){
        // Debug.Log("in swing");
        //1
        yield return new WaitForSeconds(0.1f);//0.1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);//1프레임 대기
        trailEffect.enabled = false;
        

    }

    IEnumerator Shot(){
        //총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;
        //탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3,-1) + Vector3.up*Random.Range(2,4);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    //Use() 메인 루틴 -> Swing() 서브루틴 -> 메인루틴 : 일반 함수
    //Use() 메인 루틴 + Swing() 코루틴 (Co-op): 코루틴
    
}
