using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    int m_score = 0;
    int m_lines;
    public int m_level = 1;
    public ParticelPlayer m_levelUpFx;

    BestScore m_bestScore;

    public int m_linesPerLevel = 5;

    public Text m_linesText;
    public Text m_levelText;
    public Text m_scoreText;

    public bool m_didLevelUP = false;

    const int m_minLines = 1;
    const int m_maxLines = 4;

    public void ScoreLInes(int n)
    {
        m_didLevelUP = false;
        n = Mathf.Clamp(n, m_minLines, m_maxLines);

        switch (n)
        {
            case 1:
                m_score += 40 * m_level;
                break;
            case 2:
                m_score += 100 * m_level;
                break;
            case 3:
                m_score += 300 * m_level;
                break;
            case 4:
                m_score += 1200 * m_level;
                break;
        }
        m_lines -= n;

        if (m_lines <= 0)
        {
            LevelUp();
        }
        UpdateUiText();
    }
    public void Reset_ ()
    {
        m_level = 1;
        m_lines = m_linesPerLevel * m_level;
        UpdateUiText();
    }
    // Use this for initialization
    void Start ()
    {
        Reset_();
        m_bestScore = GameObject.FindObjectOfType<BestScore>();


    }
void UpdateUiText()
    {
        if (m_linesText)
        {
            m_linesText.text = m_lines.ToString();
        }
        if (m_levelText)
        {
           m_levelText.text = m_level.ToString();
        }
        if (m_scoreText)
        {
            m_scoreText.text = PadZero(m_score, 6);
        }
        BestScore();
    }
    string PadZero(int n, int padDigits)
    {
       string nStr = n.ToString();

        while (nStr.Length < padDigits)
        {
            nStr = "0" + nStr;
        }
        return nStr;
    }
    public void LevelUp()
    {
        m_level++;
        m_lines = m_linesPerLevel * m_level;
        m_didLevelUP = true;
        if (m_levelUpFx)
        {
            m_levelUpFx.Play();
        }
    }
    public void BestScore()
    {
        if (m_score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", m_score);
           // m_bestScore.m_highestScore.text = m_score.ToString();
            
        } 

    }
}
