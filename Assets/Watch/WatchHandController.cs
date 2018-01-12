using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchHandController : MonoBehaviour {

    public float ROTATION_INCREMENT, MAX_ROTATION_SPEED;
    /* public delegate Tuple<int, int> WatchHandListener(); */

    private Transform _minuteHand, _hourHand;
    private float _rotationSpeed;
    private float _hourHandRotationMultiplier = 30f / 360f; /* The hour hand moves 30 degrees for every 360 the minute hand travels */
    //private int[] _snapPoints = { 0, 3, 6, 9, 12 };
    //private List<WatchHandListener> _listeners;

	// Use this for initialization
	void Start () {
        _hourHand = this.transform.GetChild(0);
        _minuteHand = this.transform.GetChild(1);
        //_listeners = new List<WatchHandListener>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.RightArrow))
        {
            /* Increment the rotation speed */
            _rotationSpeed = Mathf.Min(MAX_ROTATION_SPEED, _rotationSpeed + ROTATION_INCREMENT * Time.deltaTime);

            /*** Move the hands forward ***/
            /* Minute hands get full effect of speed */
            _minuteHand.RotateAround(this.transform.position, this.transform.up, _rotationSpeed * Time.deltaTime);

            /* Hour hand gets a reduced rotation amount */
            _hourHand.RotateAround(this.transform.position, this.transform.up, _hourHandRotationMultiplier * _rotationSpeed * Time.deltaTime);

        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            /* Decrement the rotation speed */
            _rotationSpeed = Mathf.Max(-1 * MAX_ROTATION_SPEED, _rotationSpeed - ROTATION_INCREMENT * Time.deltaTime);

            /*** Move the hands backward ***/
            /* Minute hand get full effect of speed */
            _minuteHand.RotateAround(this.transform.position, this.transform.up, _rotationSpeed * Time.deltaTime);

            /* Hour hand gets a reduced rotation amount */
            _hourHand.RotateAround(this.transform.position, this.transform.up, _hourHandRotationMultiplier * _rotationSpeed * Time.deltaTime);
        } else
        {
            /* If movement has stopped, kill rotation speed, lerp the hands to the closest position */
            _rotationSpeed = 0f;

            /* Pass the time to all listeners */
            /*Tuple<int, int> time = new Tuple<int, int>(WatchHandRotationToTime(_hourHand.rotation.y), WatchHandRotationToTime(_minuteHand.rotation.y));
            foreach (WatchHandListener listener in _listeners)
                listener(time);*/
        }
	}

    /* public void AddListener(WatchHandListener listener)
    {
        _listeners.Add(listener);
    } */

    private int WatchHandRotationToTime(float rotation)
    {
        return (int)rotation % 360 / 30;

        /* Snap to 15 minute increments(?) */
        //return _snapPoints.aggregate((x, y) => Mathf.Abs(x - clampedRotation) < Mathf.Abs(y - clampedRotation) ? x : y);
    }
}
