using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject levelSelect;
    
    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.FindWithTag("MainMenu");
        levelSelect = GameObject.FindWithTag("LevelSelect");

        levelSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);
        Debug.Log("Level Select");
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        Debug.Log("Load Level");
    }

    public void ReturnMainMenu()
    {
        mainMenu.SetActive(true);
        levelSelect.SetActive(false);
        Debug.Log("Back To Main Menu");
    }
}
