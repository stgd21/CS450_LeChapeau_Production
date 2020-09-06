using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false; //has game ended?
    public float timeToWin; //itme a player needs to hold hat to win
    public float invincibleDuration; //how long after a plyaer gets hat are they invincible
    private float hatPickupTime; //the time the hat was picked up by current holder
    private float smallHatPickupTime; //time the small hat was picked up by current holder

    [Header("Players")]
    public string playerPrefabLocation; //path in resources folder to the Player prefab
    public Transform[] spawnPoints; //array of all available spawn points 
    public PlayerController[] players; //array of all players
    public int playerWithHat; //id of player with hat
    public int playerWithSmallHat; //id of player with small hat
    private int playersInGame; //number of players int he game

    //instance
    public static GameManager instance;

    private void Awake()
    {
        //instance
        instance = this;
    }

    private void Start()
    {
        //list of players is as big as there are players in photon's detection
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    //Called when player joins game
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        //Instantiate player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.identity);
        //GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, 7)].transform.position, Quaternion.identity);
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);

        //get the player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //initialize player
        playerScript.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    //called when a player hits the hatten player - giving them the hat
    [PunRPC]
    public void GiveHat(int playerId, bool initialGive = false)
    {
        //remove hat from currently hatted player
        if (!initialGive)
        {
            GameUI.instance.RemoveHat(playerWithHat);
            GetPlayer(playerWithHat).SetHat(false);
        }
        //give the hat to the new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        GameUI.instance.GiveHat(playerWithHat);
        hatPickupTime = Time.time;

        if (playerWithHat == playerWithSmallHat)
        {
            instance.photonView.RPC("WinGame", RpcTarget.All, playerWithHat);
        }
    }

    [PunRPC]
    public void GiveSmallHat(int playerId, bool initialGive = false)
    {
        //remove hat from currently hatted player
        if (!initialGive)
        {
            GameUI.instance.RemoveSmallHat(playerWithSmallHat);
            GetPlayer(playerWithSmallHat).SetSmallHat(false);
        }
        //give small hat to new player
        playerWithSmallHat = playerId;
        GetPlayer(playerId).SetSmallHat(true);
        GameUI.instance.GiveSmallHat(playerWithSmallHat);
        smallHatPickupTime = Time.time;
        
    }
    //is the player able to the the hat at this current time?
    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    //is player able to get small hat at this time?
    public bool CanGetSmallHat(int id)
    {
        if (Time.time > smallHatPickupTime + invincibleDuration && playerWithHat != id)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame (int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        //set the UI to show who's won

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
