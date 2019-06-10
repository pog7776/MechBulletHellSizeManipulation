using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    private ScoreDatabase scoreDatabase;
    private string scoreDatabasePath = "ScoreDatabase";

    private string path;

    private void Awake() {
        SetSingleton();
        SetData();
        //LoadData();
        ScoreData data1;
            data1.name = "John";
            data1.value = 500;

        ScoreData data2;
            data2.name = "Jim";
            data2.value = 600;
        scoreDatabase = new ScoreDatabase();
        scoreDatabase.AddScore(data1);
        scoreDatabase.AddScore(data2);
        SaveData(scoreDatabase, scoreDatabasePath);
    }

    private void SetSingleton() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }


    private void SetData() {
        path = Application.dataPath + "/StreamingAssets/JSON/";
    }

    public void SaveData(object obj, string fileName) {
        string contents = JsonUtility.ToJson(obj, true);
        System.IO.File.WriteAllText(path + fileName, contents);
    }


    private void LoadData() {
        string contents = System.IO.File.ReadAllText(path + scoreDatabasePath);
        scoreDatabase = JsonUtility.FromJson<ScoreDatabase>(contents);
    }

}

public class ScoreDatabase {
    [SerializeField] private List<ScoreData> scoreList = new List<ScoreData>();

    public void AddScore(ScoreData data) {
        scoreList.Add(data);
    }

    public List<ScoreData> GetAllScoreData(int id) {
        return scoreList;
    }

}

[System.Serializable]
public struct ScoreData {
    public string name;
    public float value;
}