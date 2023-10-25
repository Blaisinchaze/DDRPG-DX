using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMController : MonoBehaviour
{
    public float BPM;
    public float BPS;

    public float timerCount;
    public float currentBeat;

    BPMComboMetre comboMetre;
    // Start is called before the first frame update
    void Start()
    {
        comboMetre = GetComponent<BPMComboMetre>();
        BPS = BPM/60;
    }

    // Update is called once per frame
    void Update()
    {
        timerCount += Time.deltaTime * BPS;
        if(timerCount >= 1)
        {
            comboMetre.BeatHit();
            timerCount = -1;
        }
    }

    public float BeatAccuracyReturn()
    {
        return Mathf.Abs(timerCount);
    }
}
