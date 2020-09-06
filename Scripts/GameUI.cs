using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    //instance
    public static GameUI instance;

    private void Awake()
    {
        //set instance to this script
        instance = this;
    }

    private void Start()
    {
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {
        //loop through all containers
        for (int i = 0; i < playerContainers.Length; ++i)
        {
            PlayerUIContainer container = playerContainers[i];

            //only enable and modify UI containers we need
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                //container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        //UpdatePlayerUI();
    }

    public void RemoveHat(int id)
    {
        playerContainers[id-1].hasBigHatText.gameObject.SetActive(false);
        ////loop through all players
        //for (int i = 0; i < GameManager.instance.players.Length; ++i)
        //{
        //    //if (GameManager.instance.players[i] != null)
        //     //   playerContainers[i].hatTimeSlider.value = GameManager.instance.players[i].curHatTime;
        //}
    }

    public void RemoveSmallHat(int id)
    {
        playerContainers[id - 1].hasSmallHatText.gameObject.SetActive(false);
    }

    public void GiveHat(int id)
    {
        playerContainers[id-1].hasBigHatText.gameObject.SetActive(true);
        //Debug.Log(id);
    }

    public void GiveSmallHat(int id)
    {
        playerContainers[id - 1].hasSmallHatText.gameObject.SetActive(true);
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins!";
    }
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    //public Slider hatTimeSlider;
    public TextMeshProUGUI hasBigHatText;
    public TextMeshProUGUI hasSmallHatText;
}
