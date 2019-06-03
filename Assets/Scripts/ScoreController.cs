using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    //Score functions
    private int score;
    public Text scoreTxt;
    private int highScore;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        //Update Score Text
        scoreTxt.text = score.ToString();
    }

    //Add to score
    public void AddScore(int points)
    {
        score += points;
        Debug.Log(score);
    }

    //Get current score
    public int GetScore()
    {
        return score;
    }

    //Update High score
    //If function is needed
    /*private void UpdateHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }*/
}
