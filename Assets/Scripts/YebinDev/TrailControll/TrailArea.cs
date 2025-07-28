using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;


public class TrailArea : MonoBehaviour
{
	public GameObject mainTrail;
    public MeshCollider coll;
	public static int team;
	public static List<List<Vector3>> attackLineList = new List<List<Vector3>>();

    private void Awake()
    {
		gameObject.name = "TrailArea";
		gameObject.layer = 9;
        coll = gameObject.AddComponent<MeshCollider>();
		coll.convex = true;
		coll.isTrigger = true;
    }

	public static List<Vector3> DeformTrailArea(Trail_Action trail, List<List<Vector3>> newAreaVerticesList,Trail_Action attackTrail, List<Vector3> attackVertices)
	{
		if (newAreaVerticesList.Count == 0) {
			return null;
		}
		
		// ���� ���� ���������� ����Ʈ�� ������ ���ԵǴ����� used ����Ʈ üũ
		if (attackTrail != null) {
			bool isListInArea = true;
			foreach (var vertex in newAreaVerticesList[0])
			{
				if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), trail.Vertices2D(trail.areaVertices))) {
					isListInArea = false;
					break;
				}
			}             
			// ���� �ȵǴ� ����Ʈ�� ��� ���� �� �ٽ� ���ȣ��
			if (!isListInArea) {
				Debug.Log("���� ����Ʈ");
				newAreaVerticesList.RemoveAt(0);
				return DeformTrailArea(trail,newAreaVerticesList,attackTrail,attackVertices);
			}
		}

		List<Vector3> newAreaVertices = newAreaVerticesList[0];
		newAreaVerticesList.RemoveAt(0);	

		int newAreaVerticesCount = newAreaVertices.Count;

		if (newAreaVertices.Count > 0)
		{
			bool isClockWiseContained = false;
			bool isCounterClockWiseContained = false;
			bool isClockWiseAllContained = true;
			bool isCounterClockWiseAllContained = true;

			List<Vector3> areaVertices = trail.areaVertices;
			int startPoint = trail.GetClosestAreaVertice(newAreaVertices[0]);
			int endPoint = trail.GetClosestAreaVertice(newAreaVertices[newAreaVertices.Count - 1]);


			// CLOCKWISE AREA
			// Select redundant vertices
			List<Vector3> redundantVertices = new List<Vector3>();			
			List<Vector3> tempRedundantVertices = new List<Vector3>();		//  �ð�, �ݽð��� �������� �κ� �˻�� ����Ʈ

			for (int i = startPoint; i != endPoint; i++)
			{
				if (i == areaVertices.Count)
				{
					if (endPoint == 0)
					{
						break;
					}
					i = 0;
				}
				redundantVertices.Add(areaVertices[i]);
				tempRedundantVertices.Add(areaVertices[i]);
			}
			redundantVertices.Add(areaVertices[endPoint]);
			tempRedundantVertices.Add(areaVertices[endPoint]);

			// redundantVertices�� �̿��Ͽ� �ð� ������ ���� ������ ���ԵǴ��� üũ
			if (attackVertices != null || attackTrail != null) {		
				Debug.Log("attackLineList ����: "+attackLineList.Count);
				foreach (var border in attackLineList) {
					tempRedundantVertices = tempRedundantVertices.Except(border).ToList();
				}

				// for (int i=0; i <redundantVertices.Count; i++) {
				// 	 Debug.Log("�ð������ " +i + "��° redundant X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
				// }
				for (int i=1; i<tempRedundantVertices.Count-1; i++) 
				{				
					if (TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackVertices)) && !isClockWiseContained) {
						isClockWiseContained = true;
						// Debug.Log("cw�� " + i + "��°�� ���Ե�. X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
					}

					if (!TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackTrail.areaVertices))&& isClockWiseAllContained) {
						isClockWiseAllContained = false;
					}								
				}
			}
			
			// Add new vertices to clockwise temp area
			List<Vector3> tempAreaClockwise = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				tempAreaClockwise.Insert(i + startPoint, newAreaVertices[i]);
			}

			// Remove the redundat vertices & calculate clockwise area's size
			tempAreaClockwise = tempAreaClockwise.Except(redundantVertices).ToList();
			float clockwiseArea = GetPolygonArea(tempAreaClockwise);

			// COUNTERCLOCKWISE AREA
			// Select redundant vertices
			redundantVertices.Clear();
			tempRedundantVertices.Clear();

			for (int i = startPoint; i != endPoint; i--)
			{
				if (i == -1)
				{
					if (endPoint == areaVertices.Count - 1)
					{
						break;
					}

					i = areaVertices.Count - 1;
				}
				redundantVertices.Add(areaVertices[i]);
				tempRedundantVertices.Add(areaVertices[i]);
			}
			redundantVertices.Add(areaVertices[endPoint]);
			tempRedundantVertices.Add(areaVertices[endPoint]);

			// tempredundantVertices�� �̿��Ͽ� �ݽð� ������ ���� ������ ���ԵǴ��� üũ
			if (attackVertices != null || attackTrail != null) {
				foreach (var border in attackLineList) {
					tempRedundantVertices = tempRedundantVertices.Except(border).ToList();
				}

				// for (int i=0; i <redundantVertices.Count; i++) {
				// 	 Debug.Log("�ݽð������ " +i + "��° redundant X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
				// }
				for (int i=1; i<tempRedundantVertices.Count-1; i++) 
				{				
					if (TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackVertices)) && !isCounterClockWiseContained) {
						//Debug.Log("ccw�� " + i + "��°�� ���Ե�. X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
						isCounterClockWiseContained = true;
					}					

					if (!TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackTrail.areaVertices)) && isCounterClockWiseAllContained) {
						isCounterClockWiseAllContained = false;						
					}					
				}				
			}

			// Add new vertices to clockwise temp area
			List<Vector3> tempAreaCounterclockwise = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				tempAreaCounterclockwise.Insert(startPoint, newAreaVertices[i]);
			}

			// Remove the redundant vertices & calculate counterclockwise area's size
			tempAreaCounterclockwise = tempAreaCounterclockwise.Except(redundantVertices).ToList();
			float counterclockwiseArea = GetPolygonArea(tempAreaCounterclockwise);

			
			// �����ϴ� �� deformarea
			if (attackVertices == null) {
				trail.newAreaVertices_containBorder = clockwiseArea > counterclockwiseArea ? tempAreaCounterclockwise : tempAreaClockwise;	
				return clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;
			}
			// ���ݹ޴� �� deformarea
			else {				
				Debug.Log("�ð� ���� : "+ counterclockwiseArea+" �ݽð� ���� : " + clockwiseArea);
				Debug.Log("CW ���� : "+isClockWiseAllContained);
				Debug.Log("CCW ���� : "+isCounterClockWiseAllContained);	

				// Case 1. �Ѵ� ���� �ȵǴ� ���
				if (!isClockWiseContained && !isCounterClockWiseContained) {
					Debug.Log("Case 1.");
					return clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;	
				}
				// Case 2. �� �� �ð踸 ���ԵǴ� ���
				else if (isClockWiseContained && !isCounterClockWiseContained) {					
					// Case 2.1 �ð谡 Ŭ ���
					if (clockwiseArea <= counterclockwiseArea) {
						// Case 2.1.1 �ð谡 ���� ���ԵǴ� ���
						if (isClockWiseAllContained) {
							Debug.Log("Case 2.1.1");
							return tempAreaClockwise;
						}
						// Case 2.1.2 �ð谡 ���� ���Ե��� �ʴ� ���
						else {
							// ������ ����Ʈ ��� �� �ð� ����Ʈ�� ������Ʈ
							Debug.Log("Case 2.1.2");
							List<Vector3> areaVertices_prev = trail.areaVertices;
							trail.areaVertices = tempAreaCounterclockwise;
							// �ð迡 ���ؼ� ���ȣ�� �� ���� ���ؼ� �ݽð�� �� �� ū ����Ʈ ����
							List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList, attackTrail, attackVertices);																					
							if (areaVertices_cw != null) {
								float area_cw = GetPolygonArea(areaVertices_cw);
								trail.areaVertices = areaVertices_prev;							
								Debug.Log("area_cw : "+area_cw + " clockwiseArea : " + clockwiseArea);
								return area_cw > clockwiseArea ? areaVertices_cw : tempAreaClockwise;	
							}
							// newAreaVerticesList�� ����Ʈ�� ���� �Ἥ null�� ���� �� ���
							else {
								trail.areaVertices = areaVertices_prev;	
								Debug.Log("newAreaVerticesList ���� �Ҹ�");
								return tempAreaClockwise;
							}
						}
					}

					
					// Case 2.2 �ð谡 ���� ���
					else {
						Debug.Log("Case 2.2");
						return tempAreaClockwise;
					}
				}
				// Case 3. �� �� �ݽð踸 ���ԵǴ� ���
				else if (!isClockWiseContained && isCounterClockWiseContained) {					
					// Case 3.1 �ݽð谡 Ŭ ���
					if (clockwiseArea >= counterclockwiseArea) {
						// Case 3.1.1 �ݽð谡 ���� ���ԵǴ� ���
						if (isCounterClockWiseAllContained) {
							Debug.Log("Case 3.1.1");
							return tempAreaCounterclockwise;
						}
						// Case 3.1.2 �ݽð谡 ���� ���Ե��� �ʴ� ���
						else {
							Debug.Log("Case 3.1.2");
							// ������ ����Ʈ ��� �� �ݽð� ����Ʈ�� ������Ʈ
							List<Vector3> areaVertices_prev = trail.areaVertices;
							trail.areaVertices = tempAreaClockwise;
							// �ݽð迡 ���ؼ� ���ȣ�� �� ���� ���ؼ� �ð�� �� �� ū ����Ʈ ����
							List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
							if (areaVertices_ccw != null) {
								float area_ccw = GetPolygonArea(areaVertices_ccw);
								trail.areaVertices = areaVertices_prev;							
								Debug.Log("area_ccw : "+area_ccw + " counterclockwiseArea : " + counterclockwiseArea);
								return area_ccw > counterclockwiseArea ? areaVertices_ccw : tempAreaCounterclockwise;	
							}
							else {
								trail.areaVertices = areaVertices_prev;	
								Debug.Log("newAreaVerticesList ���� �Ҹ�");
								return tempAreaCounterclockwise;
							}
						}
					}
					// Case 3.2 �ݽð谡 ���� ���
					else {
						Debug.Log("Case 3.2");
						return tempAreaCounterclockwise;
					}
				}
				// Case 4 �� �� ���ԵǴ� ���
				else {
					// Case 4.1 �� �� ���� ���ԵǴ� ���					
					if (isClockWiseAllContained && isCounterClockWiseAllContained) {		
						Debug.Log("Case 4.1");				
						return null;
					}
					// Case 4.2 �ð踸 ���� ���ԵǴ� ���
					else if (isClockWiseAllContained && !isCounterClockWiseAllContained) {
						Debug.Log("Case 4.2");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// �ݽð迡 ���ؼ� ���ȣ��
						trail.areaVertices = tempAreaClockwise;
						List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;				
						if (areaVertices_ccw == null) {
							areaVertices_ccw = tempAreaClockwise;
						}				

						return areaVertices_ccw;
					}
					// Case 4.3 �ݽð踸 ���� ���ԵǴ� ���
					else if (!isClockWiseAllContained && isCounterClockWiseAllContained) {
						Debug.Log("Case 4.3");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// �ð迡 ���ؼ� ���ȣ��
						trail.areaVertices = tempAreaCounterclockwise;
						List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;		
						if (areaVertices_cw == null) {
							areaVertices_cw = tempAreaCounterclockwise;
						}			

						return areaVertices_cw;
					}
					// Case 4.4 �� �� ���� ���Ե��� �ʴ� ���
					else {
						Debug.Log("Case 4.4");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// �ð迡 ���ؼ� ���ȣ��
						trail.areaVertices = tempAreaClockwise;
						List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList.ConvertAll(s => s), attackTrail, attackVertices);																																	
						
						// �ݽð迡 ���ؼ� ���ȣ��
						trail.areaVertices = tempAreaCounterclockwise;
						List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList.ConvertAll(s => s), attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;								

						// newAreaVerticesList �� ���� �Ҹ��Ͽ� null�� ���ϵǴ� ���
						if (areaVertices_cw == null && areaVertices_ccw != null) {
							Debug.Log("�ð谡 null");
							return areaVertices_ccw;
						}
						else if (areaVertices_cw != null && areaVertices_ccw == null) {
							Debug.Log("�ݽð谡 null");
							return areaVertices_cw;
						}
						else if (areaVertices_cw == null && areaVertices_ccw == null) {
							Debug.Log("�Ѵ� null");
							return null;
						}						
						else {				
							Debug.Log("�Ѵ� null �ƴ�");		
							float area_cw = GetPolygonArea(areaVertices_cw);
							float area_ccw = GetPolygonArea(areaVertices_ccw);
							return area_cw > area_ccw ? areaVertices_cw : areaVertices_ccw;		
						}
					}
				}
			}
		}
		else {
			return null;
		}		
	}

	// https://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
	public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
	{
		int polygonLength = polygon.Length, i = 0;
		bool inside = false;
		float pointX = point.x, pointY = point.y;
		float startX, startY, endX, endY;
		Vector2 endPoint = polygon[polygonLength - 1];
		endX = endPoint.x;
		endY = endPoint.y;
		while (i < polygonLength)
		{
			startX = endX; startY = endY;
			endPoint = polygon[i++];
			endX = endPoint.x; endY = endPoint.y;
			inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
		}
		return inside;
	}
	public static float GetPolygonArea(List<Vector3> sourceList) { 
		float polygonArea = 0f;           
		int firstIndex; int secondIndex; int sourceCount = sourceList.Count;           
		Vector3 firstPoint; 
		Vector3 secondPoint;       
		float factor = 0f;          
		for(firstIndex = 0; firstIndex < sourceCount; firstIndex++) { 
			secondIndex = (firstIndex + 1) % sourceCount;               
			firstPoint  = sourceList[firstIndex]; 
			secondPoint = sourceList[secondIndex];               
			factor = ((firstPoint.x * secondPoint.z) - (secondPoint.x * firstPoint.z));               
			polygonArea += factor; 
		}           
		polygonArea /= 2f; 
		return Mathf.Abs(polygonArea); 
	}
	//��ó: https://icodebroker.tistory.com/4103 [ICODEBROKER]
}
