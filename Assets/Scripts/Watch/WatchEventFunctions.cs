using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WatchEventFunctions : MonoBehaviour {

    public float ROTATION_PER_SECOND = 30f;

    /// <summary>
    /// Rotates the game object around the X-axis when called
    /// </summary>
    /// <param name="direction">The sign is used to change the direction</param>
    public void RotateAroundX(float direction) {
        this.transform.Rotate(new Vector3(Mathf.Sign(direction), 0, 0), ROTATION_PER_SECOND * Time.deltaTime);
    }

    public void LoadScene(float sceneID)
    {
        SceneManager.LoadScene((int)sceneID);
    }
}
