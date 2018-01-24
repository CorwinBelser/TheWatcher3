using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//holds all the information for a conversation that'll be given to Dialogue Manager

[System.Serializable]
public class Dialogue {

    public string name;

    [TextArea(3, 5)]
    public string[] sentences;
    
}
