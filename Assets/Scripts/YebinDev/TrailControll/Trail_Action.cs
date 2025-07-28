using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Trail_Action : MonoBehaviourPun
{
    private UIManager UI_Manager;
    private EffectManager effectManager;
    private Sound_Manager sound_mng;

    [SerializeField] Transform paintCharge_EffectPoint;
    public string playerName;
    private Vector3 walk_direction = Vector3.zero;
    
    public Color color;
    private float max_paintAmount = 100;
    [SerializeField] float curPaintAmount = 100;   // 현재 업데이트되고 있는 페인트 양
    private float prevPaintAmount;  // 업데이트 되기 전의 페인트 양
    private bool isPainting;    // 페인트를 칠하고 있는가

    public List<Trail_Action> attackedTrail_Action = new List<Trail_Action>();

    [Header("Area")]
    public int startAreaPoints = 45;                        
    public float startAreaRadius = 2f;                      // 시작 영역 지름
    public float minPointDistance = 0.05f;                   
    public TrailArea area;                                             // 
    public GameObject areaOutline;                                     //
    public List<Vector3> areaVertices = new List<Vector3>();           // 영역 꼭지점들
    public List<Vector3> newAreaVertices = new List<Vector3>();         // 새로운 영역
    public List<Vector3> newAreaVertices_containBorder = new List<Vector3>(); // 경계를 포함한 새로운 영역

    public MeshRenderer areaMeshRend;
    private MeshFilter areaFilter;
    private MeshRenderer areaOutlineMeshRend;
    private MeshFilter areaOutlineFilter;

    [Header("Trail")]
    public TrailRenderer trail;                             // trail renderer
    public GameObject trailCollidersHolder;                 // trail 충돌체 홀더
    public List<SphereCollider> trailColls = new List<SphereCollider>();       // trail 충돌체는 sphere
    public bool isTrailAttacked = false;
    private bool start_LSHIFT;                              // PC용 drawTrail toggle
    private bool isPaintingReady;                              // 모보일용 drawTrail toggle
    

    private InputManager inputManager;

    public virtual void Start()
    {
        UI_Manager = GameObject.Find("HUD Canvas").GetComponent<UIManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();        
        playerName = transform.parent.name;
        trail = transform.GetComponent<TrailRenderer>();
        trail.material.color = new Color(color.r, color.g, color.b, 0.5f);
        trail.enabled = false;
        
        
        
        if (photonView.IsMine)
        {
            TrailArea.team = gameObject.GetComponentInParent<Status>().team;
            inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
            inputManager.paint_button.onValueChanged.AddListener(DecideDrawingTrail);
        }
        InitializeTrail();
        areaOutline.name = "AreaOutline";  
    }
    public virtual void Update()
    {
        if (photonView.IsMine) {
            if (inputManager.joyStick_shoot.InputVector != Vector3.zero) {
                inputManager.paint_button.isOn = false;
                isPainting = false;
                isTrailAttacked = false;
                isPaintingReady = false;
            }

            if (isPainting) {
                curPaintAmount -= Time.deltaTime * 3f;            
                // 물감 슬라이더 UI 업데이트            
                UI_Manager.UpdateTempPaintSlider(curPaintAmount,max_paintAmount);
            }

            if (inputManager.paint_button.isOn && !isPaintingReady && IsPlayerInArea()) {
                isPaintingReady = true;
            }
            DrawTrail();
        }
    }

 

    private void CancelTrail() {
        if (trail.enabled == false) {
            return;
        }
        if (curPaintAmount <= 0) {
            sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
            // 물감이 부족하다는 경고 UI 표시
            UI_Manager.SetNoticeText("페인트가 부족합니다!");                    
        }
        else if (isTrailAttacked) {
            sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
            // 공격받았다는 경고 UI 표시
            UI_Manager.SetNoticeText("길을 밟혔습니다!");           
        }    

        curPaintAmount = prevPaintAmount;                
        isPainting = false;
        isTrailAttacked = false;
        isPaintingReady = false;

        trail.enabled = false;                
        photonView.RPC("RpcTrail_Off", RpcTarget.Others);

        foreach (var trailColl in trailColls)
        {
            Destroy(trailColl);
        }

        trailColls.Clear();
        trail.Clear();
        newAreaVertices.Clear();

        UI_Manager.UpdateTempPaintSlider(curPaintAmount,max_paintAmount);

    }
    public void DecideDrawingTrail(bool toggleState) {        
        // when dash_button active
        if (toggleState == true) {
            if (IsPlayerInArea()) {
                isPaintingReady = true;
            }
        }
        // when dash_button inActive
        else {
            isPaintingReady = false;
            if (isPainting) {
                CancelTrail();
            }
        }        
    }
    
    // 모바일 버전 drawTrail
    private void DrawTrail()
    {
        // 이동 방향 벡터 초기화
        walk_direction = Vector3.zero;

        // 이동 방향 입력 받음
        // walk_direction.x = Input.GetAxis("Horizontal");
        // walk_direction.z = Input.GetAxis("Vertical");

        // 조이스틱 이동 방향 입력
        walk_direction.x = inputManager.joyStick.InputVector.x;
        walk_direction.z = inputManager.joyStick.InputVector.z;

        walk_direction = walk_direction.normalized;

        var transPos = transform.position;
        transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // 크기가 300.0f로 고정된 vector 반환
        bool isOutside = !IsPlayerInArea();
        int count = newAreaVertices.Count;                              // newArea List의 개수


        if (isOutside)
        {
            // 처음 시작할 때, 새로운 영역이 transPos를 포함하지 않고 그전 vertices들과 현재 위치의 거리가 최소거리 이상일때
            if (isPaintingReady && inputManager.paint_button.isOn &&
            (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance))
            {
                // 처음 shift를 눌렀을 때
                if (!isPainting) {
                    prevPaintAmount = curPaintAmount;
                    isPainting = true;
                }
                
                trail.enabled = true;
                photonView.RPC("RpcTrail_On", RpcTarget.Others);

                count++;
                newAreaVertices.Add(transPos);              // 새로 만들어질 영역에 현재 위치 vertex 추가

                int trailCollsCount = trailColls.Count;     // trail 충돌체가 몇 개인지
                float trailWidth = trail.startWidth;        // trail의 width
                SphereCollider lastColl = trailCollsCount > 0 ? trailColls[trailCollsCount - 1] : null;     // 

                if (!lastColl || (transPos - lastColl.center).magnitude > trailWidth)   //
                {
                    SphereCollider trailCollider = trailCollidersHolder.AddComponent<SphereCollider>();
                    trailCollider.center = transPos;
                    trailCollider.radius = trailWidth / 2f;
                    trailCollider.isTrigger = true;
                    trailCollider.enabled = false;
                    trailColls.Add(trailCollider);

                    if (trailCollsCount > 1)
                    {
                        trailColls[trailCollsCount - 2].enabled = true;
                    }
                }


                if (!trail.emitting)
                {
                    trail.Clear();
                    trail.emitting = true;
                }
            }

            if (curPaintAmount <= 0 || isTrailAttacked)           // 페인트를 다 썼을 때, trail을 밟혔을 때
            {
                CancelTrail();
            }
        }
        else if (count > 0)
        {
            isPainting = false;
                
            List<List<Vector3>> tempList = new List<List<Vector3>>();
            tempList.Add(newAreaVertices);            
            this.areaVertices = TrailArea.DeformTrailArea(this, tempList,null,null);   // 공격하는 쪽 폐쇄영역 구현 부분
            this.UpdateArea();

            sound_mng.SFXPlay(Sound_Manager.SFXName.Success);
            UpdateAreaOnServer(areaVertices.ToArray());
            photonView.RPC("RpcTrail_Off", RpcTarget.Others);
            
            // 로컬 플레이어의 현재 영역 넓이로 점수 설정
            if (areaVertices == null) {
                GameManager.instance.SetScore(0, TrailArea.team);   // 현재 영역 넓이 갱신	
            }
            else {
                GameManager.instance.SetScore(TrailArea.GetPolygonArea(areaVertices), TrailArea.team);   // 현재 영역 넓이 갱신
            }

            // InGame 씬의 자신의 TrailArea를 제외한 모든 TrailArea들을 검사하여 포함관계에 있는지 확인한다.
            var objs = GameObject.FindGameObjectsWithTag("Trail");
            foreach (var obj in objs) {
                
                var tempTrailAction = obj.GetComponent<Trail_Action>();

                if (gameObject == obj || attackedTrail_Action.Contains(tempTrailAction)) {
                    continue;
                }
                var tempVertices = tempTrailAction.areaVertices;


                bool allContained = true;
                foreach (var vertex in tempVertices)
                {
                    //Debug.Log("각 vertex 테스트");                    
                    if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(areaVertices)))
                    {
                        allContained = false;                        
                        break;
                    }
                }
                // 모두 포함되면 제거
                if (allContained) {
                    tempTrailAction.areaVertices = null;                        
                    Destroy(tempTrailAction.area.gameObject);    
                    Destroy(tempTrailAction.gameObject);
                    // 멀티 동기화 부분 추가할것 점수랑 영역                    
                    
                }
            }

            // 공격자가 다른 영역을 밟았을 때의 중첩 영역 구현
            foreach (var trail_Action in attackedTrail_Action)
            {
                // 중첩된 점들의 리스트
                List<List<Vector3>> newCharacterAreaVerticesList = new List<List<Vector3>>(10);

                int index = 0;
                newCharacterAreaVerticesList.Add(new List<Vector3>());

                foreach (var vertex in newAreaVertices)
                {
                    // 새로 추가된 점들이 중첩일 때
                    if (TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices)))
                    {
                        newCharacterAreaVerticesList[index].Add(vertex);
                    }
                    // 새로 추가된 점들이 중첩이 아닐 때
                    else
                    {
                        // 현재 리스트가 차있으면 새로운 리스트 만들기
                        if (newCharacterAreaVerticesList[index].Count != 0)
                        {
                            index++;
                            newCharacterAreaVerticesList.Add(new List<Vector3>());
                        }
                    }
                }

                List<List<Vector3>> inAreaList = new List<List<Vector3>>();     // 공격리스트 팝용
                List<List<Vector3>>  attackLineList = new List<List<Vector3>>();    // 공격리스트 전체

                foreach (var list in newCharacterAreaVerticesList)
                {   
                    bool isListInArea = true;

                    if (list.Count == 0)
                    {
                        continue;
                    }
                    
                    foreach (var vertex in list)
                    {
                        if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices))) {
                            isListInArea = false;
                            break;
                        }
                    }                    
                    
                    if (isListInArea) {                        
                        Debug.Log("공격당한쪽 deformTrailArea 실행");                        
                        inAreaList.Add(list);
                        attackLineList.Add(list);
                    }                                        
                }


                if (inAreaList.Count != 0) {
                    TestTrailArea.attackLineList = attackLineList;
                    trail_Action.areaVertices = TrailArea.DeformTrailArea(trail_Action, inAreaList,this,newAreaVertices_containBorder); 
                    trail_Action.UpdateArea();  
                }
                TestTrailArea.attackLineList = null;    

                trail_Action.UpdateAreaOnServer(trail_Action.areaVertices.ToArray());                

                // 중첩 영역 제거시 로컬 플레이어가 아니므로 공격받은 플레이어의 점수 설정
                if (trail_Action.areaVertices == null) {
					trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, 0);
				}
				else {
					trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, TrailArea.GetPolygonArea(trail_Action.areaVertices));
				}	
            }

            foreach (var trailColl in trailColls)                   // 폐쇄 조건 충족시 trail 제거
            {
                Destroy(trailColl);
            }
            trailColls.Clear();

            if (trail.emitting)
            {
                trail.Clear();
                trail.emitting = false;
            }

            attackedTrail_Action.Clear();
            newAreaVertices.Clear();            
        }
    }

    [PunRPC]
    private void RpcTrail_On()
    {
        trail.enabled = true;
    }

    [PunRPC]
    private void RpcTrail_Off()
    {
        trail.enabled = false;
        trail.Clear();
    }

    [PunRPC]
    private void InitializeTrail()
    {
        area = new GameObject().AddComponent<TrailArea>();        
        
        area.mainTrail = gameObject;
        
        Transform areaTrans = area.transform;
        areaFilter = area.gameObject.AddComponent<MeshFilter>();
        areaMeshRend = area.gameObject.AddComponent<MeshRenderer>();
        areaMeshRend.material.color = color;

        areaOutline = new GameObject();                                     // 바닥면
        Transform areaOutlineTrans = areaOutline.transform;
        areaOutlineTrans.position += new Vector3(0, -0.495f, -0.1f);           
        areaOutlineTrans.SetParent(areaTrans);
        areaOutlineFilter = areaOutline.AddComponent<MeshFilter>();
        areaOutlineMeshRend = areaOutline.AddComponent<MeshRenderer>();
        areaOutlineMeshRend.material.color = new Color(color.r * .7f, color.g * .7f, color.b * .7f);
        //areaOutlineMeshRend.material.color = new Color(color.r, color.g, color.b);

        float step = 360f / startAreaPoints;                                // 꼭지점을 몇 개로 할 것인가
        for (int i = 0; i < startAreaPoints; i++)
        {
            areaVertices.Add(transform.position + Quaternion.Euler(new Vector3(0, step * i, 0)) * Vector3.forward * startAreaRadius);
        }
        UpdateArea();

        trailCollidersHolder = new GameObject();
        trailCollidersHolder.transform.SetParent(areaTrans);
        trailCollidersHolder.name = playerName + "TrailCollidersHolder";
        trailCollidersHolder.layer = 8;
        trailCollidersHolder.AddComponent<TraillColl_Manager>();
    }

    public void UpdateArea()
    {
        if (areaFilter)
        {
            Mesh areaMesh = GenerateMesh(areaVertices, playerName);                         // 영역의 mesh 만들기
            areaFilter.mesh = areaMesh;                                         // 영역의 filter에 mesh 할당
            areaOutlineFilter.mesh = areaMesh;                                   
            area.coll.sharedMesh = areaMesh;
        }
    }

    public void UpdateAreaOnServer(Vector3[] _areaVertices)
    {
        photonView.RPC("UpdateAreaOnServerProcess", RpcTarget.Others, _areaVertices);        
    }

    [PunRPC]
    private void UpdateAreaOnServerProcess(Vector3[] _areaVertices)
    {
        // 플레이어들이 씬이 로드되는 속도에 따라 오류가 생길 때가 있음
        areaMeshRend.material.color = color;
        areaOutlineMeshRend.material.color = color;
        trail.material.color = new Color(color.r, color.g, color.b, 0.5f);


        List<Vector3> _areaVerticesList = new List<Vector3>(_areaVertices);
        areaVertices = _areaVerticesList;
        UpdateArea();       
    }

    private Mesh GenerateMesh(List<Vector3> vertices, string meshName)                           // vertices를 가지고 mesh 만들기
    {
        Triangulator tr = new Triangulator(Vertices2D(vertices));
        int[] indices = tr.Triangulate();

        Mesh msh = new Mesh();
        msh.vertices = vertices.ToArray();
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
        msh.name = meshName + "Mesh";
        

        return msh;
    }

    public Vector2[] Vertices2D(List<Vector3> vertices)                        // Vector3 -> Vector2 변경
    {
        List<Vector2> areaVertices2D = new List<Vector2>();
        foreach (Vector3 vertex in vertices)
        {
            areaVertices2D.Add(new Vector2(vertex.x, vertex.z));
        }
        return areaVertices2D.ToArray();
    }

    public int GetClosestAreaVertice(Vector3 fromPos)
    {
        int closest = -1;
        float closestDist = Mathf.Infinity;
        for (int i = 0; i < areaVertices.Count; i++)
        {
            float dist = (areaVertices[i] - fromPos).magnitude;
            if (dist < closestDist)
            {
                closest = i;
                closestDist = dist;
            }
        }

        return closest;
    }

    [PunRPC]
	private void RPCSetScore(float newScore)
	{
		GameManager.instance.SetScore(newScore,TrailArea.team);   // 현재 영역 넓이 갱신
	}

    
    public void FillPaint(float amount) {        
        curPaintAmount += amount;
        if (curPaintAmount > max_paintAmount) {
            curPaintAmount = max_paintAmount;
        }
        UI_Manager.UpdateTempPaintSlider(curPaintAmount,max_paintAmount);
        effectManager.playPaintCharge_Effect(paintCharge_EffectPoint);
    }

    public void ChangeColor() {        
        areaMeshRend.material.color = color;
        areaOutlineMeshRend.material.color = color;
        trail.material.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    private bool IsPlayerInArea(){
        var transPos = transform.position;
        transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // 크기가 300.0f로 고정된 vector 반환
        return TrailArea.IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));  // 영역 안에 있는가
    }

    // PC 버전에서의 이동
    //  private void DrawTrail()
    // {
    //     // 이동 방향 벡터 초기화
    //     walk_direction = Vector3.zero;

    //     // 이동 방향 입력 받음
    //     walk_direction.x = Input.GetAxis("Horizontal");
    //     walk_direction.z = Input.GetAxis("Vertical");

    //     // walk_direction = walk_direction.normalized;

    //     var transPos = transform.position;
    //     transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // 크기가 300.0f로 고정된 vector 반환
    //     bool isOutside = !TrailArea.IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));  // 영역 안에 있는가
    //     int count = newAreaVertices.Count;                              // newArea List의 개수

    //     if (!isOutside)                                                 // 영역 안
    //     {
    //         if (Input.GetKeyDown(KeyCode.LeftShift))
    //         {
    //             start_LSHIFT = true;
    //         }
    //     }

    //     if (isOutside)
    //     {
    //         // 처음 시작할 때, 새로운 영역이 transPos를 포함하지 않고 그전 vertices들과 현재 위치의 거리가 최소거리 이상일때
    //         if (Input.GetKey(KeyCode.LeftShift) && (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance) && start_LSHIFT)
    //         {
    //             // 처음 shift를 눌렀을 때
    //             if (!isPainting) {
    //                 prevPaintAmount = curPaintAmount;
    //                 isPainting = true;
    //             }
                
    //             trail.enabled = true;
    //             photonView.RPC("RpcTrail_On", RpcTarget.Others);

    //             count++;
    //             newAreaVertices.Add(transPos);              // 새로 만들어질 영역에 현재 위치 vertex 추가

    //             int trailCollsCount = trailColls.Count;     // trail 충돌체가 몇 개인지
    //             float trailWidth = trail.startWidth;        // trail의 width
    //             SphereCollider lastColl = trailCollsCount > 0 ? trailColls[trailCollsCount - 1] : null;     // 

    //             if (!lastColl || (transPos - lastColl.center).magnitude > trailWidth)   //
    //             {
    //                 SphereCollider trailCollider = trailCollidersHolder.AddComponent<SphereCollider>();
    //                 trailCollider.center = transPos;
    //                 trailCollider.radius = trailWidth / 2f;
    //                 trailCollider.isTrigger = true;
    //                 trailCollider.enabled = false;
    //                 trailColls.Add(trailCollider);

    //                 if (trailCollsCount > 1)
    //                 {
    //                     trailColls[trailCollsCount - 2].enabled = true;
    //                 }
    //             }


    //             if (!trail.emitting)
    //             {
    //                 trail.Clear();
    //                 trail.emitting = true;
    //             }
    //         }


    //         if (Input.GetKeyUp(KeyCode.LeftShift) || curPaintAmount <= 0 || isTrailAttacked)                      // shift 손 땠을 때
    //         {
    //             if (trail.enabled == false)
    //             {
    //                 return;
    //             }

    //             if (curPaintAmount <= 0) 
    //             {
    //                 sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
    //                 // 물감이 부족하다는 경고 UI 표시
    //                 UI_Manager.SetNoticeText("페인트가 부족합니다!");                    
    //             }
    //             else if (isTrailAttacked) 
    //             {
    //                 sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
    //                 // 공격받았다는 경고 UI 표시
    //                 UI_Manager.SetNoticeText("길을 밟혔습니다!");           
    //             }                


    //             curPaintAmount = prevPaintAmount;                
    //             isPainting = false;
    //             isTrailAttacked = false;
    //             UI_Manager.UpdateTempPaintSlider(curPaintAmount,max_paintAmount);

    //             trail.enabled = false;                
    //             photonView.RPC("RpcTrail_Off", RpcTarget.Others);
    //             foreach (var trailColl in trailColls)
    //             {
    //                 Destroy(trailColl);
    //             }

    //             trailColls.Clear();
    //             trail.Clear();
    //             newAreaVertices.Clear();

    //             start_LSHIFT = false;
    //         }
    //     }
    //     else if (count > 0)
    //     {
    //         isPainting = false;
    //         List<List<Vector3>> tempList = new List<List<Vector3>>();
    //         tempList.Add(newAreaVertices);            
    //         this.areaVertices = TrailArea.DeformTrailArea(this, tempList,null,null);   // 공격하는 쪽 폐쇄영역 구현 부분
    //         this.UpdateArea();

    //         sound_mng.SFXPlay(Sound_Manager.SFXName.Success);
    //         UpdateAreaOnServer(areaVertices.ToArray());
    //         photonView.RPC("RpcTrail_Off", RpcTarget.Others);
            
    //         // 로컬 플레이어의 현재 영역 넓이로 점수 설정
    //         if (areaVertices == null) {
    //             GameManager.instance.SetScore(0, TrailArea.team);   // 현재 영역 넓이 갱신	
    //         }
    //         else {
    //             GameManager.instance.SetScore(TrailArea.GetPolygonArea(areaVertices), TrailArea.team);   // 현재 영역 넓이 갱신
    //         }

    //         // InGame 씬의 자신의 TrailArea를 제외한 모든 TrailArea들을 검사하여 포함관계에 있는지 확인한다.
    //         var objs = GameObject.FindGameObjectsWithTag("Trail");
    //         foreach (var obj in objs) {
                
    //             var tempTrailAction = obj.GetComponent<Trail_Action>();

    //             if (gameObject == obj || attackedTrail_Action.Contains(tempTrailAction)) {
    //                 continue;
    //             }
    //             var tempVertices = tempTrailAction.areaVertices;


    //             bool allContained = true;
    //             foreach (var vertex in tempVertices)
    //             {
    //                 //Debug.Log("각 vertex 테스트");                    
    //                 if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(areaVertices)))
    //                 {
    //                     allContained = false;                        
    //                     break;
    //                 }
    //             }
    //             // 모두 포함되면 제거
    //             if (allContained) {
    //                 tempTrailAction.areaVertices = null;                        
    //                 Destroy(tempTrailAction.area.gameObject);    
    //                 Destroy(tempTrailAction.gameObject);
    //                 // 멀티 동기화 부분 추가할것 점수랑 영역                    
                    
    //             }
    //         }

    //         // 공격자가 다른 영역을 밟았을 때의 중첩 영역 구현
    //         foreach (var trail_Action in attackedTrail_Action)
    //         {
    //             // 중첩된 점들의 리스트
    //             List<List<Vector3>> newCharacterAreaVerticesList = new List<List<Vector3>>(10);

    //             int index = 0;
    //             newCharacterAreaVerticesList.Add(new List<Vector3>());

    //             foreach (var vertex in newAreaVertices)
    //             {
    //                 // 새로 추가된 점들이 중첩일 때
    //                 if (TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices)))
    //                 {
    //                     newCharacterAreaVerticesList[index].Add(vertex);
    //                 }
    //                 // 새로 추가된 점들이 중첩이 아닐 때
    //                 else
    //                 {
    //                     // 현재 리스트가 차있으면 새로운 리스트 만들기
    //                     if (newCharacterAreaVerticesList[index].Count != 0)
    //                     {
    //                         index++;
    //                         newCharacterAreaVerticesList.Add(new List<Vector3>());
    //                     }
    //                 }
    //             }

    //             List<List<Vector3>> inAreaList = new List<List<Vector3>>();     // 공격리스트 팝용
    //             List<List<Vector3>>  attackLineList = new List<List<Vector3>>();    // 공격리스트 전체

    //             foreach (var list in newCharacterAreaVerticesList)
    //             {   
    //                 bool isListInArea = true;

    //                 if (list.Count == 0)
    //                 {
    //                     continue;
    //                 }
                    
    //                 foreach (var vertex in list)
    //                 {
    //                     if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices))) {
    //                         isListInArea = false;
    //                         break;
    //                     }
    //                 }                    
                    
    //                 if (isListInArea) {                        
    //                     Debug.Log("공격당한쪽 deformTrailArea 실행");                        
    //                     inAreaList.Add(list);
    //                     attackLineList.Add(list);
    //                 }                                        
    //             }


    //             if (inAreaList.Count != 0) {
    //                 TestTrailArea.attackLineList = attackLineList;
    //                 trail_Action.areaVertices = TrailArea.DeformTrailArea(trail_Action, inAreaList,this,newAreaVertices_containBorder); 
    //                 trail_Action.UpdateArea();  
    //             }
    //             TestTrailArea.attackLineList = null;    

    //             trail_Action.UpdateAreaOnServer(trail_Action.areaVertices.ToArray());                

    //             // 중첩 영역 제거시 로컬 플레이어가 아니므로 공격받은 플레이어의 점수 설정
    //             if (trail_Action.areaVertices == null) {
	// 				trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, 0);
	// 			}
	// 			else {
	// 				trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, TrailArea.GetPolygonArea(trail_Action.areaVertices));
	// 			}	
    //         }

    //         foreach (var trailColl in trailColls)                   // 폐쇄 조건 충족시 trail 제거
    //         {
    //             Destroy(trailColl);
    //         }
    //         trailColls.Clear();

    //         if (trail.emitting)
    //         {
    //             trail.Clear();
    //             trail.emitting = false;
    //         }

    //         attackedTrail_Action.Clear();
    //         newAreaVertices.Clear();
    //         start_LSHIFT = false;
    //     }
    // }
}


