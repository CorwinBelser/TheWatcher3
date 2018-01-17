using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// Author: Corwin Belser
/// <summary>
/// Allows the player to move the hands of a watch forward and backward, stopping at _snapIntervalInDegrees intervals.
/// 
/// Unity events can be added to this script, and will be activated when their target time is selected
/// </summary>
/// <remarks>
/// -   This script assumes the hour hand gameObject is the first child, the minute hand gameObject the second child object.
/// -   The left and right arrow keys are used for movement. This is not multiplayer-safe, and should be pulled out to StartMovement(bool moveForward), and StopMovement()
/// -   AM vs PM is not being handled. This could be added by utilizing mod with the total rotation (360 degrees being one hour)
/// -   Holding both left and right arrow keys at the same time will only read input from the right arrow
/// </remarks>
/// <TODO>
/// -   Create a hash table with watch time as the key, and a list of Unity events as the values
///         -- Watch time could be stored and used in minute format for simplicity (Ex. 3:30 becomes 60*3 + 30 = 210
///             --- Watch time in minutes has the range [0, 720]
/// -   Implement snapping watch hands to the desired increment
///         -- Watch hands should lerp to the target position
///         -- Ideally the hands should decelerate smoothly, though using a joystick could provide better control over speed
/// </TODO>
public class WatchHandController : MonoBehaviour {

    public float ROTATION_INCREASE_PER_SECOND, MAX_ROTATION_PER_SECOND;
    [Range(1,60)]
    public int SNAP_INTERVAL_IN_MINUTES; /* Interval in minutes to snap to (0, 60] */
    private float _snapIntervalInDegrees; /* Interval in degrees to snap to [0, 360) */
    [Range(0.005f,0.95f)]
    public float SNAP_THRESHOLD; /* % away from a snap interval that is acceptable to snap */
    public float SNAP_SPEED_MULTIPLIER; /* Multiplier when rotating towards snap point when player stops input */
	
	private Dictionary<int,UnityEvent> _eventDictionary = new Dictionary<int,UnityEvent>();
	public List<int> KEYS;
	public List<UnityEvent> VALUES;

    private Transform _minuteHand, _hourHand; /* References to the hour and minute hand child gameObjects */
    private float _rotationPerSecond; /* The current amount of rotation being applied each second */
    private float _hourHandRotationMultiplier = 30f / 360f; /* The hour hand moves 30 degrees for every 360 the minute hand travels */
    private float _rotationToMinuteValue = 60f / 360f; /* Multiply to rotation [0, 360) to get the closest minute value [0,60) */
    private bool _isMoving;

	/// <summary>
	/// Gets a reference to the hour and minute hand gameObjects attached as children
	/// </summary>
	void Start () {
		if(KEYS.Count != VALUES.Count)
			Debug.LogError("The amount of keys and values for the event dictionary must be equal. Review the KEYS and VALUES lists.");
		else{
			for(int i = 0; i < KEYS.Count; i++){
				_eventDictionary.Add(KEYS[i],VALUES[i]);
			}
		}
		
        _hourHand = this.transform.GetChild(0); /* The first child gameObject is expected to be the hour hand */
        _minuteHand = this.transform.GetChild(1); /* The second child gameObject is expected to be the minute hand */
        _snapIntervalInDegrees = 360 * SNAP_INTERVAL_IN_MINUTES / 60; /* SNAP_INTERVAL_IN_MINUTES / 60 = _snapIntervalInDegrees / 360 */
        _isMoving = false; /* Initially no movement is applied */
	}
	
	/// <summary>
	/// Check for player input, and apply movement to the watch hands
	/// </summary>
	void Update () {
		if (Input.GetAxis("Horizontal") != 0)
        {
            //Debug.Log(Input.GetAxis("Horizontal"));
            /* Increment the rotation speed */
            _rotationPerSecond = Mathf.Min(MAX_ROTATION_PER_SECOND, _rotationPerSecond + (ROTATION_INCREASE_PER_SECOND * Input.GetAxis("Horizontal") * Time.deltaTime));
            _isMoving = true;

        }
        /* The player has stopped providing input. Check if the hands are at a stopping point, lerping if not */
        else if (_isMoving)
        {
            //Debug.Log("Snap Interval: " + _snapIntervalInDegrees + ", rotation: " + _minuteHand.localEulerAngles.y);
            /* if minute hand is at an interval of _snapIntervalInDegrees degrees (or super close), stop moving and process watch events for this time */
            float percentAwayFromSnap = _minuteHand.localEulerAngles.y % _snapIntervalInDegrees / _snapIntervalInDegrees;
            //Debug.Log("%: " + percentAwayFromSnap);
            if (percentAwayFromSnap < SNAP_THRESHOLD || percentAwayFromSnap > 1 - SNAP_THRESHOLD) 
            {
                /* Find the closest snap interval */
                float snapPoint = Mathf.Round(_minuteHand.localEulerAngles.y % 360 / _snapIntervalInDegrees);
                float rotationAmount = snapPoint * _snapIntervalInDegrees - _minuteHand.localEulerAngles.y;
                /* Rotate the minute hand to the snap point */
                _minuteHand.RotateAround(this.transform.position, this.transform.up, rotationAmount);

                /* Move the hour hand to its position relative to the minute hand */
                _hourHand.RotateAround(this.transform.position, this.transform.up, _hourHandRotationMultiplier * rotationAmount);

                _rotationPerSecond = 0f;
                _isMoving = false;
                ProcessWatchEvents();
            }
            else /* else  lerp towards the nearest increment of SNAP_INTERVAL_IN_DEGREES */
            {
                /* Find the closest snap point */
                float snapPoint = Mathf.Round(_minuteHand.localEulerAngles.y % 360 / _snapIntervalInDegrees);

                /* Set the rotation value to push the hands towards the snap point */
                _rotationPerSecond = (Mathf.Floor(_minuteHand.localEulerAngles.y / 360) * 360 + snapPoint * _snapIntervalInDegrees - _minuteHand.localEulerAngles.y) * SNAP_SPEED_MULTIPLIER;
            }
        }

        /* If there is rotation to be applied, move the hands */
        if (_isMoving)
        {
            /* Minute hands get full effect of speed */
            _minuteHand.RotateAround(this.transform.position, this.transform.up, _rotationPerSecond * Time.deltaTime);

            /* Hour hand gets a reduced rotation amount */
            _hourHand.RotateAround(this.transform.position, this.transform.up, _hourHandRotationMultiplier * _rotationPerSecond * Time.deltaTime);
        }
	}

    /// <summary>
    /// Called when the watch stops at a _snapIntervalInDegrees
    /// </summary>
    private void ProcessWatchEvents()
    {
        Debug.Log("Watch Time is " + GetWatchTimeInMinutes());
        if (_eventDictionary.ContainsKey(GetWatchTimeInMinutes()))
		    _eventDictionary[GetWatchTimeInMinutes()].Invoke();
    }

    /// <summary>
    /// Converts rotation to minute value
    /// </summary>
    /// <param name="rotation">Rotation in degrees of watch hand</param>
    /// <returns>[0,60) minute value of watch hand</returns>
    private int WatchHandRotationToTime(float rotation)
    {
        float positiveRot = rotation;
        while (positiveRot < 0)
            positiveRot += 360;
        return Mathf.RoundToInt(rotation % 360 * _rotationToMinuteValue);
    }

    /// <summary>
    /// Converts the hours to minutes and added to existing minutes
    /// </summary>
    /// <returns>current time in minutes on the watch </returns>
    private int GetWatchTimeInMinutes()
    {
        //Debug.Log(_hourHand.localEulerAngles.y % 360 / 30);
        return Mathf.FloorToInt(_hourHand.localEulerAngles.y % 360 / 30 + 0.1f) * 60 + WatchHandRotationToTime(_minuteHand.localEulerAngles.y);
    }
}
