using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EffectManager : MonoBehaviour
{
    [Header ("EffPrefab")]
    [SerializeField] GameObject bulletFire_EffPrefab; 
    [SerializeField] GameObject dash_EffPrefab;
    [SerializeField] GameObject playerHit_EffPrefab;
    [SerializeField] GameObject playerDead_EffPrefab;
    [SerializeField] GameObject playerRespawn_EffPrefab;    
    [SerializeField] GameObject paintCharge_EffPrefab;       

    public void playBullet_Effect(Transform transform) {        
        Debug.Log("bullet 이펙트 실행 완료");        
        //Instantiate(bulletFire_EffPrefab,transform.position,transform.rotation,transform);        
        GameObject effect = PhotonNetwork.Instantiate(bulletFire_EffPrefab.name, transform.position, transform.rotation);
        effect.transform.SetParent(transform);
    }

    public void playDash_Effect(Vector3 pos, Quaternion rot) {
        Debug.Log("dash 이펙트 실행 완료");        
        //Instantiate(dash_EffPrefab,pos,rot);     
        // pos = pos + new Vector3(0,-2f,0);
        PhotonNetwork.Instantiate(dash_EffPrefab.name, pos, rot);
    }

    public void playHit_Effect(Transform transform) {
        Debug.Log("playerHit 이펙트 실행 완료");        
        GameObject effect = PhotonNetwork.Instantiate(playerHit_EffPrefab.name, transform.position, transform.rotation);
        effect.transform.SetParent(transform);
    }

    public void playDead_Effect(Transform transform) {
        Debug.Log("playerDead 이펙트 실행 완료");        
        GameObject effect = PhotonNetwork.Instantiate(playerDead_EffPrefab.name, transform.position, transform.rotation);
        effect.transform.SetParent(transform);
    }

    public void playRespawn_Effect(Transform transform) {
        Debug.Log("playerRespawn 이펙트 실행 완료");        
        GameObject effect = PhotonNetwork.Instantiate(playerRespawn_EffPrefab.name, transform.position, transform.rotation);
        effect.transform.SetParent(transform);
    }

    public void playPaintCharge_Effect(Transform transform) {
        Debug.Log("paintCharge 이펙트 실행 완료");        
        GameObject effect = PhotonNetwork.Instantiate(paintCharge_EffPrefab.name, transform.position, transform.rotation);
        effect.transform.SetParent(transform);
    }
}
