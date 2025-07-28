using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Gun_Model : MonoBehaviourPun
{
    private KeyInput_Manager key_mng = new KeyInput_Manager();
    private Sound_Manager sound_mng;

    public GameObject player;
    public GameObject bullet;
    public GameObject redTeamBullet_Prefab;
    public GameObject orangeTeamBullet_Prefab;
    public GameObject yellowTeamBullet_Prefab;
    public GameObject greenTeamBullet_Prefab;
    public GameObject blueTeamBullet_Prefab;
    public GameObject navyTeamBullet_Prefab;
    public GameObject purpleTeamBullet_Prefab;

    
    public Transform effectSpawnPoint;
    private Camera following_camera;

    public LayerMask layer_mask;
    public int layer_num = 3;

    public float shooting_cool = 0.3f;
    public float reloading_cool = 2;
    public float gun_distance = 3f;
    public int max_ammo = 10;
    public bool enable_shooting = true;

    private Vector3 playerPos;
    private bool is_shooting = false;
    private bool is_reloading = false;
    private float shot_cooltime;
    private float reload_cooltime;
    private int ammo;    

    private UIManager ui_manager;
    private GameManager gameManager;
    private EffectManager effectManager;
    private TeamManager teamManager;
    private JoyStick joystick;

    private void Start()
    {
        //sound_mng = Sound_Manager.instance.GetComponent<Sound_Manager>();

        player = transform.parent.gameObject;
        following_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        ui_manager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();        
        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        joystick = GameObject.Find("HUD Canvas").GetComponentsInChildren<JoyStick>()[1];
        ammo = max_ammo;
        shot_cooltime = shooting_cool;
        reload_cooltime = reloading_cool;
    }

    
    private void Update()
    {
        // 캐릭터가 죽어있으면 입력 받지 않음
        if (player.GetComponent<Status>().isDead) {
            return;
        }
        // 게임이 시작하지 않았으면 입력 받지 않음
        if (!gameManager.isStart) {
            return;
        }

        // 로컬 플레이어만 총을 발사가능
        if (!photonView.IsMine)
        {
            return;
        }

        // 게임이 끝났으면 입력 받지 않음
        if (gameManager.isGameover) {
            return;
        }

        setPosition();
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(EventSystem.current.IsPointerOverGameObject());

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                checkShooting();
            }
            else
            {
                return;
            }
        }
        */
        checkShooting();

        if (Input.GetAxis("Reload") > 0)
        {
            is_reloading = true;
            sound_mng.SFXPlay(Sound_Manager.SFXName.Reload);
        }

        ui_manager.UpdateAmmoText(max_ammo, ammo, is_reloading);
        ui_manager.UpdateAmmoList(max_ammo, ammo);

        if (is_shooting)
        {
            shot_cooltime -= Time.deltaTime;
            if (shot_cooltime < 0 || is_reloading)
            {
                shot_cooltime = shooting_cool;
                is_shooting = false;
            }
        }

        if (is_reloading)
        {
            reload_cooltime -= Time.deltaTime;
            if(reload_cooltime < 0)
            {
                reload_cooltime = reloading_cool;
                is_reloading = false;
                sound_mng.SFXPlay(Sound_Manager.SFXName.StopGun);
                ammo = max_ammo;
            }
        }

        //        Debug.Log("dir :" + dir);
        //        Debug.Log("rot :" + transform.rotation);

    }

    private void FixedUpdate()
    {
        // 로컬 플레이어의 총만 조작 가능
        if (photonView.IsMine) {
            JoystickTurn();
        }        
    }

    private void spawnBullet()
    {
        Vector3 home = new Vector3(0, 9.26f, 0);
        PhotonNetwork.Instantiate(bullet.name, home, transform.rotation);
    }

    private void setPosition()          // player 위치 설정
    {
        playerPos = player.transform.position;
    }


    [PunRPC]
    private void checkShooting()     // bullet 인스턴스 생성
    {
        Action action;

        keyToAction(out action);


        if (joystick.InputVector != Vector3.zero && !is_shooting && !is_reloading && enable_shooting)
        {
            sound_mng.SFXPlay(Sound_Manager.SFXName.Shot);

            // // 블루팀인 경우 블루 총알 생성
            // if (teamManager.localPlayerTeam == 1) {
            //     PhotonNetwork.Instantiate(blueTeamBullet_Prefab.name, transform.position, transform.rotation);
            // }
            // // 레드팀인 경우 레드 총알 생성
            // else {
            //     PhotonNetwork.Instantiate(redTeamBullet_Prefab.name, transform.position, transform.rotation);
            // }


            // 빨주노초파남보 hex
            // #FF0000
            // #FFA500
            // #FFFF00
            // #008000
            // #0000FF
            // #000080
            // #800080

            Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;   
            if (playerCP["Color"].ToString().Equals("#FF0000") ) {
                PhotonNetwork.Instantiate(redTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            else if (playerCP["Color"].ToString().Equals("#FFA500") ) {
                PhotonNetwork.Instantiate(orangeTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            else if (playerCP["Color"].ToString().Equals("#FFFF00") ) {
                PhotonNetwork.Instantiate(yellowTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            else if (playerCP["Color"].ToString().Equals("#008000") ) {
                PhotonNetwork.Instantiate(greenTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            else if (playerCP["Color"].ToString().Equals("#0000FF") ) {
                PhotonNetwork.Instantiate(blueTeamBullet_Prefab.name, transform.position, transform.rotation);
            }   
            else if (playerCP["Color"].ToString().Equals("#000080") ) {
                PhotonNetwork.Instantiate(navyTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            else if (playerCP["Color"].ToString().Equals("#800080") ) {
                PhotonNetwork.Instantiate(purpleTeamBullet_Prefab.name, transform.position, transform.rotation);
            }
            
            //PhotonNetwork.Instantiate(bullet.name, transform.position, transform.rotation);
            effectManager.playBullet_Effect(effectSpawnPoint);

            //Instantiate(bullet, transform.position, transform.rotation);    // 총의 transform 정보 그대로 bullet에 전달
            is_shooting = true;
            ammo--;

            if (ammo <= 0)
            {
                sound_mng.SFXPlay(Sound_Manager.SFXName.Reload);
                is_reloading = true;
            }
        }

    }


    private void Turn()                 // 마우스 방향으로 총 돌리기 
    {
        //Debug.Log("Gun_Turn");

        Ray ray = following_camera.ScreenPointToRay(Input.mousePosition);

        //int layer_mask = 1 << layer_num;

        RaycastHit rayHit;
        if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 1000, layer_mask))
        {
            
            Ray debugRay = new Ray(rayHit.point, following_camera.transform.position);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);



            Vector3 nextVec = rayHit.point - playerPos;

            nextVec.y = 0;

            transform.LookAt(transform.position + nextVec);     // 마우스 방향으로 총 돌리기

            transform.position = player.transform.position + nextVec.normalized * gun_distance;   // 마우스 방향으로 총 옮기기
        }
    }

    private void JoystickTurn()                 // 마우스 방향으로 총 돌리기 
    {
        transform.LookAt(transform.position + joystick.InputVector);     // 마우스 방향으로 총 돌리기

        if(joystick.InputVector != Vector3.zero)
            transform.position = Vector3.up * 1 + player.transform.position + joystick.InputVector.normalized * gun_distance;   // 마우스 방향으로 총 옮기기
    }


    private void keyToAction(out Action action)
    {
        KeyInput key_state;

        key_mng.checkInput(out key_state);
        action = new Action();

        if (key_state.MB1)
        {
            action.Fire = true;
        }

        return;
    }
}
