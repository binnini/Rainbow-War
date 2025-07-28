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
		
		// 공격 당한 영역에서는 리스트가 영역에 포함되는지와 used 리스트 체크
		if (attackTrail != null) {
			bool isListInArea = true;
			foreach (var vertex in newAreaVerticesList[0])
			{
				if (!TrailArea.IsPointInPolygon(new Vector2(vertex.x, vertex.z), trail.Vertices2D(trail.areaVertices))) {
					isListInArea = false;
					break;
				}
			}             
			// 포함 안되는 리스트의 경우 제외 후 다시 재귀호출
			if (!isListInArea) {
				Debug.Log("다음 리스트");
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
			List<Vector3> tempRedundantVertices = new List<Vector3>();		//  시계, 반시계의 없어지는 부분 검사용 리스트

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

			// redundantVertices를 이용하여 시계 영역이 공격 영역에 포함되는지 체크
			if (attackVertices != null || attackTrail != null) {		
				Debug.Log("attackLineList 개수: "+attackLineList.Count);
				foreach (var border in attackLineList) {
					tempRedundantVertices = tempRedundantVertices.Except(border).ToList();
				}

				// for (int i=0; i <redundantVertices.Count; i++) {
				// 	 Debug.Log("시계방향의 " +i + "번째 redundant X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
				// }
				for (int i=1; i<tempRedundantVertices.Count-1; i++) 
				{				
					if (TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackVertices)) && !isClockWiseContained) {
						isClockWiseContained = true;
						// Debug.Log("cw의 " + i + "번째가 포함됨. X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
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

			// tempredundantVertices를 이용하여 반시계 영역이 공격 영역에 포함되는지 체크
			if (attackVertices != null || attackTrail != null) {
				foreach (var border in attackLineList) {
					tempRedundantVertices = tempRedundantVertices.Except(border).ToList();
				}

				// for (int i=0; i <redundantVertices.Count; i++) {
				// 	 Debug.Log("반시계방향의 " +i + "번째 redundant X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
				// }
				for (int i=1; i<tempRedundantVertices.Count-1; i++) 
				{				
					if (TrailArea.IsPointInPolygon(new Vector2(tempRedundantVertices[i].x, tempRedundantVertices[i].z), trail.Vertices2D(attackVertices)) && !isCounterClockWiseContained) {
						//Debug.Log("ccw의 " + i + "번째가 포함됨. X :  " + redundantVertices[i].x + " Z : " + redundantVertices[i].z);
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

			
			// 공격하는 쪽 deformarea
			if (attackVertices == null) {
				trail.newAreaVertices_containBorder = clockwiseArea > counterclockwiseArea ? tempAreaCounterclockwise : tempAreaClockwise;	
				return clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;
			}
			// 공격받는 쪽 deformarea
			else {				
				Debug.Log("시계 넓이 : "+ counterclockwiseArea+" 반시계 넓이 : " + clockwiseArea);
				Debug.Log("CW 포함 : "+isClockWiseAllContained);
				Debug.Log("CCW 포함 : "+isCounterClockWiseAllContained);	

				// Case 1. 둘다 포함 안되는 경우
				if (!isClockWiseContained && !isCounterClockWiseContained) {
					Debug.Log("Case 1.");
					return clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;	
				}
				// Case 2. 둘 중 시계만 포함되는 경우
				else if (isClockWiseContained && !isCounterClockWiseContained) {					
					// Case 2.1 시계가 클 경우
					if (clockwiseArea <= counterclockwiseArea) {
						// Case 2.1.1 시계가 전부 포함되는 경우
						if (isClockWiseAllContained) {
							Debug.Log("Case 2.1.1");
							return tempAreaClockwise;
						}
						// Case 2.1.2 시계가 전부 포함되지 않는 경우
						else {
							// 이전의 리스트 백업 후 시계 리스트로 업데이트
							Debug.Log("Case 2.1.2");
							List<Vector3> areaVertices_prev = trail.areaVertices;
							trail.areaVertices = tempAreaCounterclockwise;
							// 시계에 대해서 재귀호출 후 넓이 구해서 반시계와 비교 후 큰 리스트 리턴
							List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList, attackTrail, attackVertices);																					
							if (areaVertices_cw != null) {
								float area_cw = GetPolygonArea(areaVertices_cw);
								trail.areaVertices = areaVertices_prev;							
								Debug.Log("area_cw : "+area_cw + " clockwiseArea : " + clockwiseArea);
								return area_cw > clockwiseArea ? areaVertices_cw : tempAreaClockwise;	
							}
							// newAreaVerticesList를 리스트를 전부 써서 null이 리턴 된 경우
							else {
								trail.areaVertices = areaVertices_prev;	
								Debug.Log("newAreaVerticesList 전부 소모");
								return tempAreaClockwise;
							}
						}
					}

					
					// Case 2.2 시계가 작을 경우
					else {
						Debug.Log("Case 2.2");
						return tempAreaClockwise;
					}
				}
				// Case 3. 둘 중 반시계만 포함되는 경우
				else if (!isClockWiseContained && isCounterClockWiseContained) {					
					// Case 3.1 반시계가 클 경우
					if (clockwiseArea >= counterclockwiseArea) {
						// Case 3.1.1 반시계가 전부 포함되는 경우
						if (isCounterClockWiseAllContained) {
							Debug.Log("Case 3.1.1");
							return tempAreaCounterclockwise;
						}
						// Case 3.1.2 반시계가 전부 포함되지 않는 경우
						else {
							Debug.Log("Case 3.1.2");
							// 이전의 리스트 백업 후 반시계 리스트로 업데이트
							List<Vector3> areaVertices_prev = trail.areaVertices;
							trail.areaVertices = tempAreaClockwise;
							// 반시계에 대해서 재귀호출 후 넓이 구해서 시계와 비교 후 큰 리스트 리턴
							List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
							if (areaVertices_ccw != null) {
								float area_ccw = GetPolygonArea(areaVertices_ccw);
								trail.areaVertices = areaVertices_prev;							
								Debug.Log("area_ccw : "+area_ccw + " counterclockwiseArea : " + counterclockwiseArea);
								return area_ccw > counterclockwiseArea ? areaVertices_ccw : tempAreaCounterclockwise;	
							}
							else {
								trail.areaVertices = areaVertices_prev;	
								Debug.Log("newAreaVerticesList 전부 소모");
								return tempAreaCounterclockwise;
							}
						}
					}
					// Case 3.2 반시계가 작을 경우
					else {
						Debug.Log("Case 3.2");
						return tempAreaCounterclockwise;
					}
				}
				// Case 4 둘 다 포함되는 경우
				else {
					// Case 4.1 둘 다 전부 포함되는 경우					
					if (isClockWiseAllContained && isCounterClockWiseAllContained) {		
						Debug.Log("Case 4.1");				
						return null;
					}
					// Case 4.2 시계만 전부 포함되는 경우
					else if (isClockWiseAllContained && !isCounterClockWiseAllContained) {
						Debug.Log("Case 4.2");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// 반시계에 대해서 재귀호출
						trail.areaVertices = tempAreaClockwise;
						List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;				
						if (areaVertices_ccw == null) {
							areaVertices_ccw = tempAreaClockwise;
						}				

						return areaVertices_ccw;
					}
					// Case 4.3 반시계만 전부 포함되는 경우
					else if (!isClockWiseAllContained && isCounterClockWiseAllContained) {
						Debug.Log("Case 4.3");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// 시계에 대해서 재귀호출
						trail.areaVertices = tempAreaCounterclockwise;
						List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList,attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;		
						if (areaVertices_cw == null) {
							areaVertices_cw = tempAreaCounterclockwise;
						}			

						return areaVertices_cw;
					}
					// Case 4.4 둘 다 전부 포함되지 않는 경우
					else {
						Debug.Log("Case 4.4");		
						List<Vector3> areaVertices_prev = trail.areaVertices;
						
						// 시계에 대해서 재귀호출
						trail.areaVertices = tempAreaClockwise;
						List<Vector3> areaVertices_cw = DeformTrailArea(trail, newAreaVerticesList.ConvertAll(s => s), attackTrail, attackVertices);																																	
						
						// 반시계에 대해서 재귀호출
						trail.areaVertices = tempAreaCounterclockwise;
						List<Vector3> areaVertices_ccw = DeformTrailArea(trail, newAreaVerticesList.ConvertAll(s => s), attackTrail, attackVertices);														
						trail.areaVertices = areaVertices_prev;								

						// newAreaVerticesList 를 전부 소모하여 null이 리턴되는 경우
						if (areaVertices_cw == null && areaVertices_ccw != null) {
							Debug.Log("시계가 null");
							return areaVertices_ccw;
						}
						else if (areaVertices_cw != null && areaVertices_ccw == null) {
							Debug.Log("반시계가 null");
							return areaVertices_cw;
						}
						else if (areaVertices_cw == null && areaVertices_ccw == null) {
							Debug.Log("둘다 null");
							return null;
						}						
						else {				
							Debug.Log("둘다 null 아님");		
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
	//출처: https://icodebroker.tistory.com/4103 [ICODEBROKER]
}
