using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Note inheritance difference for callback function access
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    private void Awake()
    {
        //if an instance already exists that's not this one destroy it
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            //set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //Connect to Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    //attempt to create a new room
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    //attempt to join a room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //change scene using Photon's system because it has features
    //to stop sending messages between scene changes and more
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    //override from parent PUN class
    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to master server");
        //CreateRoom("testroom");
    }

    //override from parent PUN class
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }
}
