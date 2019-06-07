using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelection : MonoBehaviour
{
    public bool shieldSelected = false;
    public bool blinkSelected = false;

    private GameObject[] abilitySelection;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        abilitySelection = GameObject.FindGameObjectsWithTag("AbilitySelection");
        if(abilitySelection.Length > 1){
            Destroy(abilitySelection[0]);
        }
    }

    public void shieldSet(bool set){
        shieldSelected = set;
        if(blinkSelected){
            blinkSelected = false;
        }
    }

    public void blinkSet(bool set){
        blinkSelected = set;
        if(shieldSelected){
            shieldSelected = false;
        }
    }
} 
