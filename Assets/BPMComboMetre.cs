using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMComboMetre : MonoBehaviour
{
    public int maxMissedBeats = 5;
    public int currentMissedBeats = 0;
    public float currentComboMetre = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeatHit()
    {
        currentMissedBeats -= 1;
        if (currentMissedBeats == 0)
        {
            currentMissedBeats = maxMissedBeats;
            currentComboMetre -= 5f;
        }
    }
}
