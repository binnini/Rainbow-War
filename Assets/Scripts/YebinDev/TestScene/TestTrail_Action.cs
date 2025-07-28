using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class TestTrail_Action : MonoBehaviourPun
{
    public string playerName;
    private Vector3 walk_direction = Vector3.zero;
    private bool start_LSHIFT;
    public Color color;
    private float max_paintAmount = 100;
    [SerializeField] float curPaintAmount = 100;   // ���� ������Ʈ�ǰ� �ִ� ����Ʈ ��
    private float prevPaintAmount;  // ������Ʈ �Ǳ� ���� ����Ʈ ��
    private bool isPainting;    // ����Ʈ�� ĥ�ϰ� �ִ°�

    public List<TestTrail_Action> attackedTrail_Action = new List<TestTrail_Action>();

    [Header("Area")]
    public int startAreaPoints = 45;                        
    public float startAreaRadius = 2f;                      // ���� ���� ����
    public float minPointDistance = 0.05f;                   
    public TestTrailArea area;                                             // 
    public GameObject areaOutline;                                     //
    public List<Vector3> areaVertices = new List<Vector3>();           // ���� ��������
    public List<Vector3> newAreaVertices = new List<Vector3>(); // ���ο� ����
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
    
    public virtual void Start()
    {
        trail = transform.GetComponent<TrailRenderer>();
        trail.material.color = new Color(color.r, color.g, color.b, 0.5f);
        trail.enabled = false;
        
        InitializeTrail();
        areaOutline.name = "AreaOutline";  
    }

    public virtual void Update()
    {
        DrawTrail();
    }

    private void DrawTrail()
    {
        // �̵� ���� ���� �ʱ�ȭ
        walk_direction = Vector3.zero;

        // �̵� ���� �Է� ����
        walk_direction.x = Input.GetAxis("Horizontal");
        walk_direction.z = Input.GetAxis("Vertical");

        walk_direction = walk_direction.normalized;

        var transPos = transform.position;
        transform.position = Vector3.ClampMagnitude(transPos, 300.0f);       // ũ�Ⱑ 300.0f�� ������ vector ��ȯ
        bool isOutside = !TrailArea.IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));  // ���� �ȿ� �ִ°�
        int count = newAreaVertices.Count;                              // newArea List�� ����

        if (!isOutside)                                                 // ���� ��
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                start_LSHIFT = true;
            }
        }

        if (isOutside)
        {
            // ó�� ������ ��, ���ο� ������ transPos�� �������� �ʰ� ���� vertices��� ���� ��ġ�� �Ÿ��� �ּҰŸ� �̻��϶�
            if (Input.GetKey(KeyCode.LeftShift) && (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance) && start_LSHIFT)
            {
                // ó�� shift�� ������ ��
                if (!isPainting) {
                    isPainting = true;
                }
                
                trail.enabled = true;

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


            if (Input.GetKeyUp(KeyCode.LeftShift) || curPaintAmount <= 0 || isTrailAttacked)                      // shift �� ���� ��
            {
                if (trail.enabled == false)
                {
                    return;
                }

                isPainting = false;
                isTrailAttacked = false;

                trail.enabled = false;                
                foreach (var trailColl in trailColls)
                {
                    Destroy(trailColl);
                }

                trailColls.Clear();
                trail.Clear();
                newAreaVertices.Clear();

                start_LSHIFT = false;
            }
        }
        else if (count > 0)
        {
            isPainting = false;
            List<List<Vector3>> tempList = new List<List<Vector3>>();
            tempList.Add(newAreaVertices);            
            this.areaVertices = TestTrailArea.DeformTrailArea(this, tempList,null,null);   // �����ϴ� �� ��⿵�� ���� �κ�
            this.UpdateArea();

            
            // InGame ���� �ڽ��� TrailArea�� ������ ��� TrailArea���� �˻��Ͽ� ���԰��迡 �ִ��� Ȯ���Ѵ�.
            var objs = GameObject.FindGameObjectsWithTag("Trail");
            foreach (var obj in objs) {
                
                var tempTrailAction = obj.GetComponent<TestTrail_Action>();

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
                    trail_Action.areaVertices = TestTrailArea.DeformTrailArea(trail_Action, inAreaList, this,newAreaVertices_containBorder); 
                    trail_Action.UpdateArea();  
                }
                TestTrailArea.attackLineList = null;               
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
            start_LSHIFT = false;
        }
    }

    private void InitializeTrail()
    {
        area = new GameObject().AddComponent<TestTrailArea>();        
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
        //float value = Mathf.Abs(areaVertices.Take(areaVertices.Count - 1).Select((p, i) => (areaVertices[i + 1].x - p.x) * (areaVertices[i + 1].z + p.z)).Sum() / 2f);
        //Debug.Log(gameObject.transform.parent.name + " ���� ������� �� "+value);
        //value = TestTrailArea.GetPolygonArea(areaVertices);
        //Debug.Log(gameObject.transform.parent.name + " �� ��� ���� �� "+value);
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
}


