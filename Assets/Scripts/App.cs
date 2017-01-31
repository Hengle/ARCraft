using UnityEngine;
using System.Collections;

// This class controls the application behaviors. Currently only contains quit action.
public class App : MonoBehaviour {

    public GameObject quitConfirmPanel;

	// Use this for initialization
	void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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
