using UnityEngine;
using UnityEngine.Events;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class WatchTimeEvent
{
    [Range(1, 12), SerializeField]
    public int HOUR;
    [Range(1, 59), SerializeField]
    public int MINUTE;
    [SerializeField]
    public UnityEvent EVENT;

    public int GetTimeInMinutes()
    {
        return this.HOUR * 60 + this.MINUTE;
    }
}
