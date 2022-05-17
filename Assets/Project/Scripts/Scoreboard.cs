using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;

    public bool open;

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();


    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
        //UpdateScoreboardItem(player);
    }
    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }
    
    public void UpdateScoreboardItem(Player player)
    {
        PV.RPC("RPC_UpdateScoreboardItem", RpcTarget.All, player);
    }
    [PunRPC]
    void RPC_UpdateScoreboardItem(Player player)
    {
        if (!PV.IsMine)
            return;
        
        scoreboardItems[player].UpdateDeathScoreBoardItem((float)player.CustomProperties["playerDeathScore"]);
    }

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            UpdateScoreboardItem(targetPlayer);
        }
    }
}
