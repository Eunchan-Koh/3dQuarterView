using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;

    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int hp;
    

    public int maxAmmo;
    public int maxCoin;
    public int maxHp;
    public int maxHasGrenades;
    

    Rigidbody rigid;

    Vector3 moveVec;
    [SerializeField]
    float movingSpeed;
    [SerializeField]
    float runSpeed;
    public float applySpeed;
    public float jumpPower;
    public Vector3 facingDir;
    Vector3 wasHere;
    Vector3 willMoveTo;
    [SerializeField]
    Vector3 prevMoveVec;

    public Animator anim;


    bool getRun;
    bool getJump;
    bool getItem;
    bool getFire;
    bool getReload;
    bool swapWeapon1;
    bool swapWeapon2;
    bool swapWeapon3;
    
    int facingCount;
    float rotationSpeed;
    

    bool isRun;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    Vector3 dodgeVec;
    float dodgeSpeed;
    
    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;
    
    void Start(){
        rigid = GetComponent<Rigidbody>();
        // prevMoveVec = new Vector3(0,0,1);
        prevMoveVec = moveVec;
        // moveVec = new Vector3(0,0,1);
        facingCount = 15;
        // moveVec = new Vector3(1,0,0);
        rotationSpeed = 10;
    }
    void Update(){
        InputCheck();
        Interaction();
        Swap();

        RunCheck();
        SpeedCheck();
        AnimationCheck();
        
        
        Jump();
        
        
        Dodge();
        Turn();
        Reload();
        
        
        Attack();

        
        
        // Move();
        // if(moveVec != Vector3.zero){
        //     facingDir = Vector3.Lerp(facingDir,transform.position+moveVec, 0.1f);
        // }else{
            // facingDir = transform.position + moveVec;
        // }
        
        // transform.LookAt(facingDir);
    }
    // void StopToWall(){
    //     Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
    //     isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    // }
    void FixedUpdate(){
        
        Move();
        // StopToWall();
        // if(transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0 ){
        //     transform.localEulerAngles = new Vector3(0, transform.rotation.y, 0);
        // }
        
    }
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Floor"){
            isJump = false;
            anim.SetBool("isJump", false);
        }else{
            Debug.Log(collision.gameObject);
        }
    }
    void OnTriggerEnter(Collider other){
        if(other.tag == "Item"){
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    ammo+=item.value;
                    if(ammo > maxAmmo){
                        ammo = maxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin+=item.value;
                    if(coin > maxCoin){
                        coin = maxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    hp+=item.value;
                    if(hp > maxHp){
                        hp = maxHp;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades+=item.value;
                    if(hasGrenades > maxHasGrenades){
                        hasGrenades = maxHasGrenades;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    void OnTriggerStay(Collider other){
        if(other.tag == "Weapon"){
            nearObject = other.gameObject;
            // Debug.Log(nearObject);
        }
        
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "Weapon"){
            nearObject = null;
        }
    }
    void Turn(){
        //#1. 키보드에 의한 회전
        facingDir = transform.position + moveVec;
        transform.LookAt(facingDir);

        //#2. 마우스에 의한 회전
        if(equipWeapon == null) return;
        if(getFire && isFireReady && !isDodge &&!isSwap){
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)){
                Vector3 nextVec = rayHit.point;
                nextVec.y = 0;
                transform.LookAt(nextVec);
            }
        }
    }
    void InputCheck(){
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        getRun = Input.GetButton("Run");//left shfit to run
        getJump = Input.GetButtonDown("Jump");
        getFire = Input.GetButton("Fire1");
        getReload = Input.GetButtonDown("Reload");
        getItem = Input.GetButtonDown("Interaction");
        swapWeapon1 = Input.GetButtonDown("Swap1");
        swapWeapon2 = Input.GetButtonDown("Swap2");
        swapWeapon3 = Input.GetButtonDown("Swap3");
        moveVec = new Vector3(hAxis,0,vAxis).normalized;
        if(isDodge)
            moveVec = dodgeVec;
        if(isSwap || isReload || !isFireReady)
            moveVec = Vector3.zero;
    }
    void RunCheck(){
        if(getRun && moveVec!=Vector3.zero){
            isRun = true;
        }
        if(!getRun || moveVec==Vector3.zero){
            isRun = false;
        }
    }
    void SpeedCheck(){
        float tempSpeed = (isRun?runSpeed:movingSpeed)*(isDodge?2:1);
        if(isDodge){
            applySpeed = dodgeSpeed*2;
        }else{
            applySpeed = isRun?runSpeed:movingSpeed;
        }
        
    }
    void AnimationCheck(){
        if(moveVec!= Vector3.zero){
            anim.SetBool("isWalk", true);
        }else{
            anim.SetBool("isWalk", false);
        }
        if(isRun){
            anim.SetBool("isRun", true);
        }else{
            anim.SetBool("isRun", false);
        }
        
    }

    void Move(){
        wasHere = transform.position;
        // willMoveTo = transform.position + moveVec*applySpeed*Time.deltaTime;
        willMoveTo = transform.position + moveVec*applySpeed*Time.fixedDeltaTime;
        
        RaycastHit hit;
        // bool a = Physics.Raycast(wasHere, moveVec, out hit, applySpeed*Time.deltaTime);
        bool a = Physics.Raycast(wasHere, moveVec, out hit, applySpeed*Time.fixedDeltaTime);
        if(a){
            // Debug.Log(hit.point);
            transform.position = hit.point;
        }else{
            transform.position = willMoveTo;
        }
        
    }
    void Jump(){
        if(getJump && moveVec == Vector3.zero && !isJump){
            isJump = true;
            anim.SetTrigger("doJump");
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector3.up*jumpPower, ForceMode.Impulse);
        }
    }
    void Attack(){
        if(equipWeapon==null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackRate < fireDelay;

        if(getFire && isFireReady && !isDodge &&!isSwap){
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee?"doSwing":"doShot");
            fireDelay = 0;
        }
        
    }
    void Reload(){
        if(equipWeapon == null) 
            return;

        if(equipWeapon.type == Weapon.Type.Melee)
            return;

        if(ammo == 0)
            return;

        if(getReload && !isJump && !isDodge && !isSwap && isFireReady&&!isReload){
            anim.SetTrigger("doReload");
            isReload = true;   

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut(){
        int ammoReq = equipWeapon.maxAmmo - equipWeapon.curAmmo;
        int reloadAmount = ammoReq<=ammo? equipWeapon.maxAmmo-equipWeapon.curAmmo:ammo;
        equipWeapon.curAmmo += reloadAmount;
        ammo-=reloadAmount;
        isReload = false;
    }
    void Dodge(){
        if(getJump && moveVec != Vector3.zero && !isJump && !isDodge){
            // applySpeed *= 2;
            dodgeVec = moveVec;
            dodgeSpeed = isRun?runSpeed:movingSpeed;
            // Debug.Log("dodge!");
            anim.SetTrigger("doDodge");
            isDodge = true;
            
            Invoke("DodgeOut", 0.4f);
        }
    }
    void DodgeOut(){
        isDodge = false;
    }

    void Swap(){
        if(swapWeapon1 && equipWeaponIndex == 0)//already equiping that weapon
            return;
        if(swapWeapon2 && equipWeaponIndex == 1)
            return;
        if(swapWeapon3 && equipWeaponIndex == 2)
            return;

        int weaponIndex = -1;
        if(swapWeapon1) weaponIndex = 0;
        if(swapWeapon2) weaponIndex = 1;
        if(swapWeapon3) weaponIndex = 2;
        if((swapWeapon1 || swapWeapon2 || swapWeapon3)&&
            !isJump && !isDodge && !isSwap &&
            hasWeapons[weaponIndex] && equipWeapon != weapons[weaponIndex])
            {
            if(equipWeapon!=null) 
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.5f);
            
        }
    }

    void SwapOut(){
        isSwap = false;
    }

    void Interaction(){
        if(getItem && nearObject != null && !isJump){
            
            if(nearObject.tag == "Weapon"){
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;


                Destroy(nearObject);
            }
        }
    }
}
