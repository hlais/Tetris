using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour {
    public Text m_highestScore;
    // Use this for initialization
    void Start ()
    {
        m_highestScore.text = PlayerPrefs.GetInt("BestScore",0).ToString();
    }
	


}


