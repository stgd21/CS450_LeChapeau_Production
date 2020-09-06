using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;
    public GameObject smallHatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    private Camera playerCam;
    private float camDistanceFromPlayer;
    private Transform respawnPoint;

    void Start()
    {
        respawnPoint = GameObject.Find("RespawnPoint").transform;
        playerCam = GameObject.FindObjectOfType<Camera>();
        camDistanceFromPlayer = playerCam.transform.position.y - rig.transform.position.y;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
        if (photonView.IsMine)
        {
            CheckIfFalling();
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();

            //Track amount of time we're wearing the hat
            //if (hatObject.activeInHierarchy)
            //    curHatTime += Time.deltaTime;
        }
        
    }

    private void CheckIfFalling()
    {
        if (rig.transform.position.y < -35f)
        {
            rig.transform.position = respawnPoint.transform.position;
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);

        //Parent the camera's vertical transform to the player's
        playerCam.transform.position = new Vector3(playerCam.transform.position.x, rig.transform.position.y + camDistanceFromPlayer, playerCam.transform.position.z);

    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    //Called when player object is instantiated
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        //give first player the big hat
        if (id == 1)
            GameManager.instance.GiveHat(id, true);

        //give second player the small hat
        if (id == 2)
            //GameManager.instance.GiveSmallHat(id, true);
            GameManager.instance.photonView.RPC("GiveSmallHat", RpcTarget.All, id, true);

        //if this isn't our local player, disable physics as that's
        //controlled by the user and synced to all other clients
        if (!photonView.IsMine)
            rig.isKinematic = true;
    }

    //sets player's hat active or not
    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    public void SetSmallHat(bool hasSmallHat)
    {
        smallHatObject.SetActive(hasSmallHat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        //did we hit another player?
        if (collision.gameObject.CompareTag("Player"))
        {
            //do they have the hat?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                //can we get the hat
                if (GameManager.instance.CanGetHat())
                {
                    //give me hat
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }

            //do they have the small hat?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithSmallHat)
            {
                //can we get the small hat
                if (GameManager.instance.CanGetSmallHat(id))
                {
                    //give me small hat
                    GameManager.instance.photonView.RPC("GiveSmallHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }

    //Start process of turning player invisible
    public void TurnInvisible()
    {
        //GetComponent<Renderer>().enabled = false;
        Color color = gameObject.GetComponent<Renderer>().material.color;
        color.g = 255f;
        gameObject.GetComponent<Renderer>().material.color = color;
        photonView.RPC("SetPlayerInvisible", RpcTarget.Others, id);
    }

    //Totally hide player from others' POV
    [PunRPC]
    public void SetPlayerInvisible(int newId)
    {
        PlayerController target = GameManager.instance.GetPlayer(newId);
        target.gameObject.GetComponent<Renderer>().enabled = false;
    }

}
