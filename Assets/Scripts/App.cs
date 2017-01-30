using UnityEngine;
using System.Collections;

public class App : MonoBehaviour {

    public GameObject quitConfirmPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(KeyCode.Escape)) {
            quitConfirmPanel.SetActive(true);
        }
	}

    public void Quit() {
        Application.Quit();
    }
    
    public void CancelQuit() {
        quitConfirmPanel.SetActive(false);
    }
}
