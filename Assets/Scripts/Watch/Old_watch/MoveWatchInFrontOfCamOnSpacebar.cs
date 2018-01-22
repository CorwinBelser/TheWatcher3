using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWatchInFrontOfCamOnSpacebar : MonoBehaviour {
	public Transform infrontpoint;
	public Transform outofthewaypoint;

    private static MoveWatchInFrontOfCamOnSpacebar _instance;

	// -1 is lower1d
	// 1 is raised
	public int phase = 1;

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		// toggle watch position on spacebar press
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (phase > 0) {
				phase = -1;
			} else {
				phase = 1;
			}
		}

		// animate if in raise or lower phase. retain information about raise/lower, but shut off animation, once we get very close.
		if (phase == 1) {
			transform.position = Vector3.Slerp(transform.position, infrontpoint.position, Time.deltaTime * 2f);
			if (Vector3.Distance(transform.position, infrontpoint.position) < -.01f) {
				phase *= 2;
			}
		} else if (phase == -1) {
			transform.position = Vector3.Slerp(transform.position, outofthewaypoint.position, Time.deltaTime * 2f);
			if (Vector3.Distance(transform.position, outofthewaypoint.position) < -.01f) {
				phase *= 2;
			}
		}
	}
}
