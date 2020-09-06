using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    private GameObject playerCamera;
    private GameObject currentAlphaWall;
    private void Start()
    {
        playerCamera = GameObject.Find("Main Camera");
        InvokeRepeating("CastVisibilityRays", 0f, 0.5f);
    }

    private void CastVisibilityRays()
    {
        //if this is controlled by this player
        if (GetComponent<PlayerController>().photonView.IsMine)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerCamera.transform.position - transform.position, out hit))
            {
                //if our raycast hits something
                if (hit.transform.gameObject == playerCamera || hit.transform.gameObject == currentAlphaWall)
                {
                    //if the ray hits the camera or the wall that is already transparent, do nothing
                    Debug.Log("returning");
                    return;
                }
                    

                if (hit.transform.gameObject.tag == "TransparentWall" && currentAlphaWall != null)
                {
                    Debug.Log("reverting because hitting transparent wall and alphawall is active");
                    RevertWall();
                }
                    

                if (currentAlphaWall != null && hit.transform.gameObject != currentAlphaWall)
                {
                    //if we have a wall that is transparent but it is not blocking the player, revert it
                    Debug.Log("reverting because in front of new wall");
                    RevertWall();
                }
                else if (hit.transform.gameObject.tag != "TransparentWall")
                {
                    //if it hits a new wall
                    Debug.Log("We hit a new wall");
                    currentAlphaWall = hit.transform.gameObject;
                    ChangeWall();
                }
            }
            else if (currentAlphaWall != null)
            {
                RevertWall();
            }
        }
    }

    private void RevertWall()
    {
        Material material = currentAlphaWall.GetComponent<Renderer>().material;
        //make opaque from unity code
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;

        Color revertColor = currentAlphaWall.GetComponent<Renderer>().material.color;
        revertColor.a = 1.0f;
        material.color = revertColor;
        currentAlphaWall.GetComponent<Renderer>().material = material;
        currentAlphaWall = null;
    }

    //turn the wall transparent
    private void ChangeWall()
    {
        //currentAlphaWall = wall;
        Material material = currentAlphaWall.GetComponent<Renderer>().material;
        //make transparent from unity code
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        Color newColor = currentAlphaWall.GetComponent<Renderer>().material.color;
        newColor.a = 0.2f;
        material.color = newColor;
        currentAlphaWall.GetComponent<Renderer>().material = material;
    }
}
