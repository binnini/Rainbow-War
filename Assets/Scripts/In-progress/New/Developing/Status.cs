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
    
    

    // ������ ��� �ð�
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

    // ����׿�
    private void Update()
    {
        //Debug.Log(its_name + " : " + health);
        //Debug.Log(its_name + " : " + PhotonNetwork.MasterClient + " " + PhotonNetwork.IsMasterClient);

        // // ����׿� ü�� ���
        // if (photonView.IsMine)
        // {
        //     if (Input.GetKeyDown(KeyCode.F))
        //     {
        //         Debug.Log("ü�� 100 ����");
        //         OnDamage(100);
        //         effectManager.playHit_Effect(respawnEff_SpawnPoint);
        //     }          
        // }

        // // ����׿� �� ����
        // if (photonView.IsMine)
        // {
        //     if (Input.GetKeyDown(KeyCode.K))
        //     {
        //         gameObject.GetComponentInChildren<Trail_Action>().color = Color.black;
        //     }          
        // }

        // ������ ��� ����
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

 

    // ü�� ������Ʈ �޼ҵ�
    [PunRPC]
    public void ApplyUpdateHealth(float newHealth)
    {
        health = newHealth;
    }

    [PunRPC]
    public void RPCPlaySound() {
        sound_mng.SFXPlay(Sound_Manager.SFXName.Hit);
    }
    
    // �������� ���� ��ŭ ü���� ����
    [PunRPC]
    public void OnDamage(float damage)
    {
        // �׾������� ���� ����
        if (isDead) {
            return;
        }

        // ȣ��Ʈ������ ����
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damage;

            sound_mng.SFXPlay(Sound_Manager.SFXName.Hit);
            photonView.RPC("RPCPlaySound",RpcTarget.Others);


            // ���� �÷��̾� �̸� �Ҵ�
            if (health <= 0) {               
                photonView.RPC("RPCSetKiller",photonView.Owner,lastHitPlayer);
            }        

            // ȣ��Ʈ�� Ŭ���̾�Ʈ ����ȭ
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);
            
            // �ٸ� Ŭ���̾�Ʈ������ OnDamage ����
            photonView.RPC("OnDamage", RpcTarget.Others, damage);                
        }

        // ü���� 0�� �Ǹ� ����
        if (photonView.IsMine)
        {
            if (health <= 0)
            {                
                isDead = true;                   
                respawn_remainTime = respawn_maxTime;
                Debug.Log(gameObject.name + "�� " + killer.NickName + "�� �׿���");
                Dead.Invoke(gameObject);
                effectManager.playDead_Effect(deadEff_SpawnPoint);
                sound_mng.SFXPlay(Sound_Manager.SFXName.StopMove);
                sound_mng.SFXPlay(Sound_Manager.SFXName.StopPainting);
                sound_mng.SFXPlay(Sound_Manager.SFXName.Dead);
                sound_mng.SFXPlay(Sound_Manager.SFXName.Respawning);
                
                // ������ �÷��̾��� ����Ʈ ����
                photonView.RPC("RPCFillPaint",killer,killer,20f);
            }
        }

    }

    [PunRPC]
    private void RPCFillPaint(Player targetPlayer ,float amount) {       
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in objs) {                    
            if (obj.GetComponent<PhotonView>().Owner.Equals(targetPlayer)) {
                Debug.Log(targetPlayer.NickName + "�� ����Ʈ ����");
                sound_mng.SFXPlay(Sound_Manager.SFXName.GetPaint);

                obj.GetComponentInChildren<Trail_Action>().FillPaint(amount);
                break;
            }
        }           
    }

    [PunRPC]
    private void RPCSetKiller(Player lastHitPlayer) {
        Debug.Log("Killer �Ҵ� �Ϸ� " + lastHitPlayer.NickName);
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

    // �����Ǿ��� ���� �ʱⰪ �ο�
    private void OnEnable()
    {
        team = 1;
        its_name = gameObject.name;
        health = max_health;
    }

    // ���� ��ġ��ŭ ü�� ȸ��
    [PunRPC]
    private void RestoreHealth(float heal_amount)
    {
        // ȣ��Ʈ������ ����
        if (PhotonNetwork.IsMasterClient)
        {
            health += heal_amount;

            // �ִ� ü�� �̻����δ� ȸ���� �ȵ�
            if (health > max_health)
            {
                health = max_health;
            }

            // ȣ��Ʈ�� Ŭ���̾�Ʈ ü�� ����ȭ
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);

            // �ٸ� Ŭ���̾�Ʈ������ RestoreHealth ����
            photonView.RPC("RestoreHealth", RpcTarget.Others, heal_amount);
        }       
        
    }

    [PunRPC]
    public void FullRestoreHealth()
    {
        // ȣ��Ʈ������ ����
        if (PhotonNetwork.IsMasterClient)
        {
            health = max_health;

            // ȣ��Ʈ�� Ŭ���̾�Ʈ ü�� ����ȭ
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health);
        }
        // ȣ��Ʈ�� �ƴ� ��� RPC
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
        
        // �� �� �÷��̾� �� �Ҵ�
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
