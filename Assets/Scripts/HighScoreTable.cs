using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    void Awake()
    {
        entryContainer = transform.Find("highScoreEntryContainer");
        entryTemplate = entryContainer.Find("highScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        //AddHighscoreEntry(10000, "pog");

        string jsonString= PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        //sort list by score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++){
            for(int j=i  +1; j < highscores.highscoreEntryList.Count; j++){
                if(highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score){
                    //swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];     
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach(HighscoreEntry highscoreEntry in highscores.highscoreEntryList){
            CreateHighScoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighScoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList){
        float templateHeight = 20f;
        Transform entryTransform = Instantiate(entryTemplate, container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            entryTransform.Find("Place").GetComponent<Text>().text = rank.ToString();

            string name = highscoreEntry.name;
            entryTransform.Find("Name").GetComponent<Text>().text = name;

            int score = highscoreEntry.score;
            entryTransform.Find("Score").GetComponent<Text>().text = score.ToString();
            
            //set backgrounds
            entryTransform.Find("BG").gameObject.SetActive(rank % 2 == 1);

            if(rank == 1){
                entryTransform.Find("Name").GetComponent<Text>().color = Color.green;
            }

            transformList.Add(entryTransform);
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
