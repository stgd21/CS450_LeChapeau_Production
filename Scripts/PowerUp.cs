using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviourPunCallbacks
{

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.GetPhotonView().IsMine)
        {
            coll.gameObject.GetComponent<PlayerController>().TurnInvisible();
        }
        photonView.RPC("DestroyMe", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
