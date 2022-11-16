using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Egam202;

public class GameController : MonoBehaviour
{
    public BlueCatcher blueCatcher;
    public RedCatcher redCatcher;
    public TMP_Text RedObjective;
    public TMP_Text BlueObjective;
    public MicrogameInstance microgame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(blueCatcher.ObjectiveNumberBlue <= 0 && redCatcher.ObjectiveNumberRed <= 0)
        {
            microgame.OnGameWin();
        }

        DisplayObjective();
    }

    void DisplayObjective()
    {
        if(blueCatcher.ObjectiveNumberBlue < 0)
        {
            blueCatcher.ObjectiveNumberBlue = 0;
        }

        BlueObjective.text = "Blue:" + blueCatcher.ObjectiveNumberBlue;

        if(redCatcher.ObjectiveNumberRed < 0)
        {
            redCatcher.ObjectiveNumberRed = 0;
        }

        RedObjective.text = "Red:" + redCatcher.ObjectiveNumberRed;
    }
}
