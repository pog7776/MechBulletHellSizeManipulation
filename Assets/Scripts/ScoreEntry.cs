using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEntry : MonoBehaviour
{

    private int score;
    private string name;
    private bool highscoreConfirm;

    private void Update() {
        if(highscoreConfirm){
            AddHighscoreEntry(score, name);
            highscoreConfirm = false;
        }
    }

    private void AddHighscoreEntry(int score, string name){
        //create highscore entry
        HighscoreEntry highscoreEntry = new HighscoreEntry {score = score, name = name};

        //load saved highscores
        string jsonString= PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        
        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        //Save updated highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }
    
    [System.Serializable]
    private class HighscoreEntry{
        public int score;
        public string name;
    }
}
