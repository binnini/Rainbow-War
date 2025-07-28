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
    [SerializeField] float curPaintAmount = 100;   // ���� ������Ʈ�ǰ� �ִ� ����Ʈ ��
    private float prevPaintAmount;  // ������Ʈ �Ǳ� ���� ����Ʈ ��
    private bool isPainting;    // ����Ʈ�� ĥ�ϰ� �ִ°�

    public List<Trail_Action> attackedTrail_Action = new List<Trail_Action>();

    [Header("Area")]
    public int startAreaPoints = 45;                        
    public float startAreaRadius = 2f;                      // ���� ���� ����
    public float minPointDistance = 0.05f;                   
    public TrailArea area;                                             // 
    public GameObject areaOutline;                                     //
    public List<Vector3> areaVertices = new List<Vector3>();           // ���� ��������
    public List<Vector3> newAreaVertices = new List<Vector3>();         // ���ο� ����
    public List<Vector3> newAreaVertices_containBorder = new List<Vector3>(); // ��踦 ������ ���ο� ����

    public MeshRenderer areaMeshRend;
    private MeshFilter areaFilter;
    private MeshRenderer areaOutlineMeshRend;
    private MeshFilter areaOutlineFilter;

    [Header("Trail")]
    public TrailRenderer trail;                             // trail renderer
    public GameObject trailCollidersHolder;                 // trail �浹ü Ȧ��
    public List<SphereCollider> trailColls = new List<SphereCollider>();       // trail �浹ü�� sphere
    public bool isTrailAttacked = false;
    private bool start_LSHIFT;                              // PC�� drawTrail toggle
    private bool isPaintingReady;                              // ���Ͽ� drawTrail toggle
    

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
                // ���� �����̴� UI ������Ʈ            
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
            // ������ �����ϴٴ� ��� UI ǥ��
            UI_Manager.SetNoticeText("����Ʈ�� �����մϴ�!");                    
        }
        else if (isTrailAttacked) {
            sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
            // ���ݹ޾Ҵٴ� ��� UI ǥ��
            UI_Manager.SetNoticeText("���� �������ϴ�!");           
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
    
    // ����� ���� drawTrail
    private void DrawTrail()
    {
        // �̵� ���� ���� �ʱ�ȭ
        walk_direction = Vector3.zero;

        // �̵� ���� �Է� ����
        // walk_direction.x = Input.GetAxis("Horizontal");
        // walk_direction.z = Input.GetAxis("Vertical");

        // ���̽�ƽ �̵� ���� �Է�
        walk_direction.x = inputManager.joyStick.InputVector.x;
        walk_direction.z = inputManager.joyStick.InputVector.z;

        walk_direction = walk_direction.normalized;

        var transPos = transform.position;
        transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // ũ�Ⱑ 300.0f�� ������ vector ��ȯ
        bool isOutside = !IsPlayerInArea();
        int count = newAreaVertices.Count;                              // newArea List�� ����


        if (isOutside)
        {
            // ó�� ������ ��, ���ο� ������ transPos�� �������� �ʰ� ���� vertices��� ���� ��ġ�� �Ÿ��� �ּҰŸ� �̻��϶�
            if (isPaintingReady && inputManager.paint_button.isOn &&
            (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance))
            {
                // ó�� shift�� ������ ��
                if (!isPainting) {
                    prevPaintAmount = curPaintAmount;
                    isPainting = true;
                }
                
                trail.enabled = true;
                photonView.RPC("RpcTrail_On", RpcTarget.Others);

                count++;
                newAreaVertices.Add(transPos);              // ���� ������� ������ ���� ��ġ vertex �߰�

                int trailCollsCount = trailColls.Count;     // trail �浹ü�� �� ������
                float trailWidth = trail.startWidth;        // trail�� width
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

            if (curPaintAmount <= 0 || isTrailAttacked)           // ����Ʈ�� �� ���� ��, trail�� ������ ��
            {
                CancelTrail();
            }
        }
        else if (count > 0)
        {
            isPainting = false;
                
            List<List<Vector3>> tempList = new List<List<Vector3>>();
            tempList.Add(newAreaVertices);            
            this.areaVertices = TrailArea.DeformTrailArea(this, tempList,null,null);   // �����ϴ� �� ��⿵�� ���� �κ�
            this.UpdateArea();

            sound_mng.SFXPlay(Sound_Manager.SFXName.Success);
            UpdateAreaOnServer(areaVertices.ToArray());
            photonView.RPC("RpcTrail_Off", RpcTarget.Others);
            
            // ���� �÷��̾��� ���� ���� ���̷� ���� ����
            if (areaVertices == null) {
                GameManager.instance.SetScore(0, TrailArea.team);   // ���� ���� ���� ����	
            }
            else {
                GameManager.instance.SetScore(TrailArea.GetPolygonArea(areaVertices), TrailArea.team);   // ���� ���� ���� ����
            }

            // InGame ���� �ڽ��� TrailArea�� ������ ��� TrailArea���� �˻��Ͽ� ���԰��迡 �ִ��� Ȯ���Ѵ�.
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
                    //Debug.Log("�� vertex �׽�Ʈ");                    
                    if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(areaVertices)))
                    {
                        allContained = false;                        
                        break;
                    }
                }
                // ��� ���ԵǸ� ����
                if (allContained) {
                    tempTrailAction.areaVertices = null;                        
                    Destroy(tempTrailAction.area.gameObject);    
                    Destroy(tempTrailAction.gameObject);
                    // ��Ƽ ����ȭ �κ� �߰��Ұ� ������ ����                    
                    
                }
            }

            // �����ڰ� �ٸ� ������ ����� ���� ��ø ���� ����
            foreach (var trail_Action in attackedTrail_Action)
            {
                // ��ø�� ������ ����Ʈ
                List<List<Vector3>> newCharacterAreaVerticesList = new List<List<Vector3>>(10);

                int index = 0;
                newCharacterAreaVerticesList.Add(new List<Vector3>());

                foreach (var vertex in newAreaVertices)
                {
                    // ���� �߰��� ������ ��ø�� ��
                    if (TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices)))
                    {
                        newCharacterAreaVerticesList[index].Add(vertex);
                    }
                    // ���� �߰��� ������ ��ø�� �ƴ� ��
                    else
                    {
                        // ���� ����Ʈ�� �������� ���ο� ����Ʈ �����
                        if (newCharacterAreaVerticesList[index].Count != 0)
                        {
                            index++;
                            newCharacterAreaVerticesList.Add(new List<Vector3>());
                        }
                    }
                }

                List<List<Vector3>> inAreaList = new List<List<Vector3>>();     // ���ݸ���Ʈ �˿�
                List<List<Vector3>>  attackLineList = new List<List<Vector3>>();    // ���ݸ���Ʈ ��ü

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
                        Debug.Log("���ݴ����� deformTrailArea ����");                        
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

                // ��ø ���� ���Ž� ���� �÷��̾ �ƴϹǷ� ���ݹ��� �÷��̾��� ���� ����
                if (trail_Action.areaVertices == null) {
					trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, 0);
				}
				else {
					trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, TrailArea.GetPolygonArea(trail_Action.areaVertices));
				}	
            }

            foreach (var trailColl in trailColls)                   // ��� ���� ������ trail ����
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

        areaOutline = new GameObject();                                     // �ٴڸ�
        Transform areaOutlineTrans = areaOutline.transform;
        areaOutlineTrans.position += new Vector3(0, -0.495f, -0.1f);           
        areaOutlineTrans.SetParent(areaTrans);
        areaOutlineFilter = areaOutline.AddComponent<MeshFilter>();
        areaOutlineMeshRend = areaOutline.AddComponent<MeshRenderer>();
        areaOutlineMeshRend.material.color = new Color(color.r * .7f, color.g * .7f, color.b * .7f);
        //areaOutlineMeshRend.material.color = new Color(color.r, color.g, color.b);

        float step = 360f / startAreaPoints;                                // �������� �� ���� �� ���ΰ�
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
            Mesh areaMesh = GenerateMesh(areaVertices, playerName);                         // ������ mesh �����
            areaFilter.mesh = areaMesh;                                         // ������ filter�� mesh �Ҵ�
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
        // �÷��̾���� ���� �ε�Ǵ� �ӵ��� ���� ������ ���� ���� ����
        areaMeshRend.material.color = color;
        areaOutlineMeshRend.material.color = color;
        trail.material.color = new Color(color.r, color.g, color.b, 0.5f);


        List<Vector3> _areaVerticesList = new List<Vector3>(_areaVertices);
        areaVertices = _areaVerticesList;
        UpdateArea();       
    }

    private Mesh GenerateMesh(List<Vector3> vertices, string meshName)                           // vertices�� ������ mesh �����
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

    public Vector2[] Vertices2D(List<Vector3> vertices)                        // Vector3 -> Vector2 ����
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
		GameManager.instance.SetScore(newScore,TrailArea.team);   // ���� ���� ���� ����
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
        transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // ũ�Ⱑ 300.0f�� ������ vector ��ȯ
        return TrailArea.IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));  // ���� �ȿ� �ִ°�
    }

    // PC ���������� �̵�
    //  private void DrawTrail()
    // {
    //     // �̵� ���� ���� �ʱ�ȭ
    //     walk_direction = Vector3.zero;

    //     // �̵� ���� �Է� ����
    //     walk_direction.x = Input.GetAxis("Horizontal");
    //     walk_direction.z = Input.GetAxis("Vertical");

    //     // walk_direction = walk_direction.normalized;

    //     var transPos = transform.position;
    //     transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // ũ�Ⱑ 300.0f�� ������ vector ��ȯ
    //     bool isOutside = !TrailArea.IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));  // ���� �ȿ� �ִ°�
    //     int count = newAreaVertices.Count;                              // newArea List�� ����

    //     if (!isOutside)                                                 // ���� ��
    //     {
    //         if (Input.GetKeyDown(KeyCode.LeftShift))
    //         {
    //             start_LSHIFT = true;
    //         }
    //     }

    //     if (isOutside)
    //     {
    //         // ó�� ������ ��, ���ο� ������ transPos�� �������� �ʰ� ���� vertices��� ���� ��ġ�� �Ÿ��� �ּҰŸ� �̻��϶�
    //         if (Input.GetKey(KeyCode.LeftShift) && (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance) && start_LSHIFT)
    //         {
    //             // ó�� shift�� ������ ��
    //             if (!isPainting) {
    //                 prevPaintAmount = curPaintAmount;
    //                 isPainting = true;
    //             }
                
    //             trail.enabled = true;
    //             photonView.RPC("RpcTrail_On", RpcTarget.Others);

    //             count++;
    //             newAreaVertices.Add(transPos);              // ���� ������� ������ ���� ��ġ vertex �߰�

    //             int trailCollsCount = trailColls.Count;     // trail �浹ü�� �� ������
    //             float trailWidth = trail.startWidth;        // trail�� width
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


    //         if (Input.GetKeyUp(KeyCode.LeftShift) || curPaintAmount <= 0 || isTrailAttacked)                      // shift �� ���� ��
    //         {
    //             if (trail.enabled == false)
    //             {
    //                 return;
    //             }

    //             if (curPaintAmount <= 0) 
    //             {
    //                 sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
    //                 // ������ �����ϴٴ� ��� UI ǥ��
    //                 UI_Manager.SetNoticeText("����Ʈ�� �����մϴ�!");                    
    //             }
    //             else if (isTrailAttacked) 
    //             {
    //                 sound_mng.SFXPlay(Sound_Manager.SFXName.Fail);
    //                 // ���ݹ޾Ҵٴ� ��� UI ǥ��
    //                 UI_Manager.SetNoticeText("���� �������ϴ�!");           
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
    //         this.areaVertices = TrailArea.DeformTrailArea(this, tempList,null,null);   // �����ϴ� �� ��⿵�� ���� �κ�
    //         this.UpdateArea();

    //         sound_mng.SFXPlay(Sound_Manager.SFXName.Success);
    //         UpdateAreaOnServer(areaVertices.ToArray());
    //         photonView.RPC("RpcTrail_Off", RpcTarget.Others);
            
    //         // ���� �÷��̾��� ���� ���� ���̷� ���� ����
    //         if (areaVertices == null) {
    //             GameManager.instance.SetScore(0, TrailArea.team);   // ���� ���� ���� ����	
    //         }
    //         else {
    //             GameManager.instance.SetScore(TrailArea.GetPolygonArea(areaVertices), TrailArea.team);   // ���� ���� ���� ����
    //         }

    //         // InGame ���� �ڽ��� TrailArea�� ������ ��� TrailArea���� �˻��Ͽ� ���԰��迡 �ִ��� Ȯ���Ѵ�.
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
    //                 //Debug.Log("�� vertex �׽�Ʈ");                    
    //                 if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(areaVertices)))
    //                 {
    //                     allContained = false;                        
    //                     break;
    //                 }
    //             }
    //             // ��� ���ԵǸ� ����
    //             if (allContained) {
    //                 tempTrailAction.areaVertices = null;                        
    //                 Destroy(tempTrailAction.area.gameObject);    
    //                 Destroy(tempTrailAction.gameObject);
    //                 // ��Ƽ ����ȭ �κ� �߰��Ұ� ������ ����                    
                    
    //             }
    //         }

    //         // �����ڰ� �ٸ� ������ ����� ���� ��ø ���� ����
    //         foreach (var trail_Action in attackedTrail_Action)
    //         {
    //             // ��ø�� ������ ����Ʈ
    //             List<List<Vector3>> newCharacterAreaVerticesList = new List<List<Vector3>>(10);

    //             int index = 0;
    //             newCharacterAreaVerticesList.Add(new List<Vector3>());

    //             foreach (var vertex in newAreaVertices)
    //             {
    //                 // ���� �߰��� ������ ��ø�� ��
    //                 if (TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(trail_Action.areaVertices)))
    //                 {
    //                     newCharacterAreaVerticesList[index].Add(vertex);
    //                 }
    //                 // ���� �߰��� ������ ��ø�� �ƴ� ��
    //                 else
    //                 {
    //                     // ���� ����Ʈ�� �������� ���ο� ����Ʈ �����
    //                     if (newCharacterAreaVerticesList[index].Count != 0)
    //                     {
    //                         index++;
    //                         newCharacterAreaVerticesList.Add(new List<Vector3>());
    //                     }
    //                 }
    //             }

    //             List<List<Vector3>> inAreaList = new List<List<Vector3>>();     // ���ݸ���Ʈ �˿�
    //             List<List<Vector3>>  attackLineList = new List<List<Vector3>>();    // ���ݸ���Ʈ ��ü

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
    //                     Debug.Log("���ݴ����� deformTrailArea ����");                        
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

    //             // ��ø ���� ���Ž� ���� �÷��̾ �ƴϹǷ� ���ݹ��� �÷��̾��� ���� ����
    //             if (trail_Action.areaVertices == null) {
	// 				trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, 0);
	// 			}
	// 			else {
	// 				trail_Action.photonView.RPC("RPCSetScore", trail_Action.photonView.Owner, TrailArea.GetPolygonArea(trail_Action.areaVertices));
	// 			}	
    //         }

    //         foreach (var trailColl in trailColls)                   // ��� ���� ������ trail ����
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


