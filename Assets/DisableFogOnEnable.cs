using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFogOnEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable() {
		RenderSettings.fog = false;
	}

	void OnDisable() {
		RenderSettings.fog = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
