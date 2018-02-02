using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startGameHack : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.LoadLevel(1));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Destroy()
    {
        GetComponent<Button>().onClick.RemoveListener(() => GameManager.Instance.LoadLevel(1));
    }
}

