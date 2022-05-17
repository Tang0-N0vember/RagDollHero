using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField] Scoreboard scoreboard;

    GameObject controller;

    const float startingDeathScore = 0f;
    float currenDeathScore = startingDeathScore;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();

            Hashtable deathScore = new Hashtable();
            deathScore.Add("playerDeathScore", currenDeathScore);
            PhotonNetwork.LocalPlayer.SetCustomProperties(deathScore);
        }
    }



    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
       controller= PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position,spawnpoint.rotation, 0,new object[] {PV.ViewID});

    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    public void ChangeDeathScore(float deathScore)
    {
        PV.RPC("RPC_ChangeDeathScore", RpcTarget.All, 1f);

    }

    [PunRPC]
    void RPC_ChangeDeathScore(float deathScore)
    {
        if (!PV.IsMine)
            return;
        currenDeathScore += deathScore;
        
        PhotonNetwork.LocalPlayer.CustomProperties["playerDeathScore"] = currenDeathScore;
        //Debug.Log("DeathScore Increased to: " + currenDeathScore);
    }
    


}
