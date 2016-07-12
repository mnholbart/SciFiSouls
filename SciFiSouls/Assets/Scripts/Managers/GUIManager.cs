using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUIManager : MonoBehaviour {

    public static GUIManager instance;

    public GameObject MainCanvas;

    PlayerGUI playerGUI;



    void Start() {
        if (instance != null) {
            Debug.LogError("Attempted to create more than one GUIManager instance");
            Destroy(instance);
        }
        instance = this;
        
        playerGUI = GetComponent<PlayerGUI>();

        MainCanvas.SetActive(true);
        playerGUI.enabled = true;
    }

    void Update() {

    }
}
