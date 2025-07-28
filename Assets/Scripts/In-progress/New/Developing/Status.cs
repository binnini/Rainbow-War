using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Status : MonoBehaviourPun
{
    private const float MAX_SPEED = 7.0f;

    public string its_name { get; private set; }
    public float health; //{ get; private set; }

    public int team;//{ get; set; }
    public float max_health = 580;
    public UIManager UI_Manager;
    public EffectManager effectManager;
    public Sound_Manager sound_mng;

    [SerializeField] Transform deadEff_SpawnPoint;    
    [SerializeField] Transform respawnEff_SpawnPoint;
    
    public Bullet bullet;

    public UnityAction<GameObject> Dead;
    public UnityAction<GameObject> Respawn;
    public bool isDead = false;
    public Player killer;
    public Player lastHitPlayer;
    
    

    // 리스폰 대기 시간
    private float respawn_maxTime = 5f;
    private float respawn_remainTime;



    private void Start() { 
        UI_Manager = GameObject.Find("HUD Canvas").GetComponent<UIManager>();       
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();    
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();

        lastHitPlayer = photonView.Owner;

        SetTeam();
        //photonView.RPC("RPCSetPlayerColor",RpcTarget.All);
    }

    
    private void SetTeam() {
        if (photonView.Owner.GetPhotonTeam().Name.Equals("Red")) {
            team = 1;
        }
        else if (photonView.Owner.GetPhotonTeam().Name.Equals("Blue")) {
            team = 2;
        }
        else {
            team = 0;
        }
    }

    // 디버그용
    private void Update()
    {
        //Debug.Log(its_name + " : " + health);
        //Debug.Log(its_name + " : " + PhotonNetwork.MasterClient + " " + PhotonNetwork.IsMasterClient);

        // // 디버그용 체력 깍기
        // if (photonView.IsMine)
        // {
        //     if (Input.GetKeyDown(KeyCode.F))
        //     {
        //         Debug.Log("체력 100 깍음");
        //         OnDamage(100);
        //         effectManager.playHit_Effect(respawnEff_SpawnPoint);
        //     }          
        // }

        // // 디버그용 색 변경
        // if (photonView.IsMine)
        // {
        //     if (Input.GetKeyDown(KeyCode.K))
        //     {
        //         gameObject.GetComponentInChildren<Trail_Action>().color = Color.black;
        //     }          
        // }

        // 리스폰 기능 실행
        if (isDead) {
            respawn_remainTime -= Time.deltaTime;
            if (killer == null) {
                return;
            }
            Hashtable killerCP = killer.CustomProperties;   
            int iconNum = (int)killerCP["Icon"];
            UI_Manager.UpdateRespawnUI(respawn_maxTime,respawn_remainTime, killer.NickName,iconNum);
            
            if(respawn_remainTime < 0) {
                Respawn.Invoke(gameObject);
                effectManager.playRespawn_Effect(respawnEff_SpawnPoint);
                sound_mng.SFXPlay(Sound_Manager.SFXName.Respawn);
                GetComponentInChildren<BoxCollider>().enabled = true;
                isDead = false;
                killer = null;
            }     
        }        
    }

 

    // 체력 업데이트 메소드
    [PunRPC]
    public void ApplyUpdateHealth(float newHealth)
    {
        health = newHealth;
    }

    [PunRPC]
    public void RPCPlaySound() {
        sound_mng.SFXPlay(Sound_Manager.SFXName.Hit);
    }
    
    // 데미지를 받은 만큼 체력을 깎음
    [PunRPC]
    public void OnDamage(float damage)
    {
        // 죽어있으면 실행 안함
        if (isDead) {
            return;
        }

        // 호스트에서만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damage;

            sound_mng.SFXPlay(Sound_Manager.SFXName.Hit);
            photonView.RPC("RPCPlaySound",RpcTarget.Others);


            // 죽인 플레이어 이름 할당
            if (health <= 0) {               
                photonView.RPC("RPCSetKiller",photonView.Owner,lastHitPlayer);
            }        

            // 호스트와 클라이언트 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);
            
            // 다른 클라이언트에서도 OnDamage 실행
            photonView.RPC("OnDamage", RpcTarget.Others, damage);                
        }

        // 체력이 0이 되면 죽음
        if (photonView.IsMine)
        {
            if (health <= 0)
            {                
                isDead = true;                   
                respawn_remainTime = respawn_maxTime;
                Debug.Log(gameObject.name + "을 " + killer.NickName + "이 죽였음");
                Dead.Invoke(gameObject);
                effectManager.playDead_Effect(deadEff_SpawnPoint);
                sound_mng.SFXPlay(Sound_Manager.SFXName.StopMove);
                sound_mng.SFXPlay(Sound_Manager.SFXName.StopPainting);
                sound_mng.SFXPlay(Sound_Manager.SFXName.Dead);
                sound_mng.SFXPlay(Sound_Manager.SFXName.Respawning);
                
                // 공격한 플레이어의 페인트 충전
                photonView.RPC("RPCFillPaint",killer,killer,20f);
            }
        }

    }

    [PunRPC]
    private void RPCFillPaint(Player targetPlayer ,float amount) {       
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in objs) {                    
            if (obj.GetComponent<PhotonView>().Owner.Equals(targetPlayer)) {
                Debug.Log(targetPlayer.NickName + "의 페인트 충전");
                sound_mng.SFXPlay(Sound_Manager.SFXName.GetPaint);

                obj.GetComponentInChildren<Trail_Action>().FillPaint(amount);
                break;
            }
        }           
    }

    [PunRPC]
    private void RPCSetKiller(Player lastHitPlayer) {
        Debug.Log("Killer 할당 완료 " + lastHitPlayer.NickName);
        killer = lastHitPlayer;
    }

    [PunRPC]
    public int getTeam()
    {
        if (photonView.IsMine)
        {
            return team;
        }
        else
        {
            return 0;
        }
    }

    // 생성되었을 때의 초기값 부여
    private void OnEnable()
    {
        team = 1;
        its_name = gameObject.name;
        health = max_health;
    }

    // 일정 수치만큼 체력 회복
    [PunRPC]
    private void RestoreHealth(float heal_amount)
    {
        // 호스트에서만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            health += heal_amount;

            // 최대 체력 이상으로는 회복이 안됨
            if (health > max_health)
            {
                health = max_health;
            }

            // 호스트와 클라이언트 체력 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);

            // 다른 클라이언트에서도 RestoreHealth 실행
            photonView.RPC("RestoreHealth", RpcTarget.Others, heal_amount);
        }       
        
    }

    [PunRPC]
    public void FullRestoreHealth()
    {
        // 호스트에서만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            health = max_health;

            // 호스트와 클라이언트 체력 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);
        }
        // 호스트가 아닌 경우 RPC
        else
        {
            photonView.RPC("FullRestoreHealth", RpcTarget.MasterClient);
        }
    }

    

    public void SetColorOnServer()
    {
        photonView.RPC("SetColorOnServerProcess", RpcTarget.OthersBuffered);              
    }

    [PunRPC]
    public void SetColorOnServerProcess()
    {
        GameObject player = gameObject;
        
        // 팀 별 플레이어 색 할당
        if (team == 1)
        {
            player.GetComponentInChildren<Trail_Action>().color = Color.blue;
        }
        else if (team == 2)
        {
            player.GetComponentInChildren<Trail_Action>().color = Color.red;
        }

        // Hashtable playerCP = photonView.Owner.CustomProperties;  
        // Color tempColor;
        // ColorUtility.TryParseHtmlString(playerCP["Color"].ToString(),out tempColor);
        // gameObject.GetComponentInChildren<Trail_Action>().color = tempColor;
    }
    
    public void SetOthersTeam()
    {
        photonView.RPC("RPCSetOthersTeamOnServer", RpcTarget.Others, team); 
    }

    [PunRPC]
    public void RPCSetOthersTeamOnServer(int team)
    {
        this.team = team;
    }

    
    public void SetPlayerColor() {        
        Hashtable playerCP = photonView.Owner.CustomProperties;  
        Color tempColor;
        ColorUtility.TryParseHtmlString(playerCP["Color"].ToString(),out tempColor);
        gameObject.GetComponentInChildren<Trail_Action>().color = tempColor;
        gameObject.GetComponentInChildren<Trail_Action>().ChangeColor();        
    }
}
