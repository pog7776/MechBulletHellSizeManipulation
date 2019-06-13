using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    //private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;

    private int inputScore;
    private string inputName;
    public bool highscoreConfirm = false;
    private GameObject enter;


    void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //clearScoreTable();

        highscoreConfirm = false;
        entryContainer = transform.Find("highScoreEntryContainer");
        entryTemplate = entryContainer.Find("highScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        // highscoreEntryList = new List<HighscoreEntry>(){
        //     new HighscoreEntry{ score = 3000, name = "pog"},
        //     new HighscoreEntry{ score = 7300, name = "jack"},
        // };

        // highscoreEntryTransformList = new List<Transform>();
        // foreach(HighscoreEntry highscoreEntry in highscoreEntryList){
        //     CreateHighScoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        // }

        // Highscores highscores = new Highscores {highscoreEntryList = highscoreEntryList};
        // string json = JsonUtility.ToJson(highscores);
        // PlayerPrefs.SetString("highScoreTable", json);
        // PlayerPrefs.Save();
        // Debug.Log(PlayerPrefs.GetString("highscoreTable"));

        AddHighscoreEntry(3, "Greg");

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

    private void Update() {
         if(highscoreConfirm){
            enter = GameObject.FindGameObjectWithTag("EnterButton");
            inputName = GameObject.FindGameObjectWithTag("TextInput").GetComponent<Text>().text;
            inputScore = int.Parse(GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text);
            AddHighscoreEntry(inputScore, inputName);
            highscoreConfirm = false;
            GameObject.FindGameObjectWithTag("TextInput").GetComponent<Text>().text = "";
            enter.SetActive(false);
            Refresh();
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

    private void Refresh(){
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

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }
    
    [System.Serializable]
    private class HighscoreEntry{
        public int score;
        public string name;
    }

    public void setConfirm(bool confirm){
        highscoreConfirm = confirm;
    }

    private void clearScoreTable(){
        //Load saved scores
        string jsonString = PlayerPrefs.GetString("scoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        //Clear scores table
        highscores.highscoreEntryList.Clear();

        //Save updated scores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("scoreTable", json);
        PlayerPrefs.Save();
        }
}
