using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum Phases
{
    one,
    two,
    over
}

public class WallManager : MonoBehaviour
{
    public GameObject[] wallsPhase1;
    public GameObject[] wallsPhase2;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI counterTextWords;
    public float timePhase1;
    public float timePhase2;

    private float startTime;
    private Phases phase = Phases.one;
    private bool triggered = false;

    private int counterTime;

    private void Start()
    {
        startTime = Time.time;
    }
    private void Update()
    {
        RunWallCalculations();
    }

    private void RunWallCalculations()
    {
        switch (phase)
        {
            case Phases.one:
                if (Time.time - startTime >= timePhase1)
                {
                    
                    DestroyWalls(1);
                    phase = Phases.two;
                    startTime = Time.time;
                }
                else
                {
                    counterTime = (int)((timePhase1 + startTime) - Time.time + 1);
                    counterText.text = counterTime.ToString();
                }
                break;
            case Phases.two:
                if (Time.time - startTime >= timePhase2)
                {
                    DestroyWalls(2);
                    phase = Phases.over;
                }
                else
                {
                    counterTime = (int)((timePhase2 + startTime) - Time.time + 1);
                    counterText.text = counterTime.ToString();
                }
                break;
            case Phases.over:
                break;
        }
            
    }

    private void DestroyWalls(int phase)
    {
        switch (phase)
        {
            case 1:
                for (int i = 0; i < wallsPhase1.Length; i++)
                    Destroy(wallsPhase1[i]);
                for (int i = 0; i < wallsPhase2.Length; i++)
                    wallsPhase2[i].SetActive(true);
                break;
            case 2:
                for (int i = 0; i < wallsPhase2.Length; i++)
                    Destroy(wallsPhase2[i]);
                Destroy(counterTextWords.gameObject);
                Destroy(counterText.gameObject);
                break;
        }

    }
}
