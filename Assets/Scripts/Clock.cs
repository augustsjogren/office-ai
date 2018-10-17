using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Text timerLabel;

    private float time;

    public float hours = 7.0f;
    public float minutes = 0.0f;

    public Slider slider;
    public float timeScale = 1.0f;

    void Update()
    {
        time += Time.deltaTime * 400;

        //var seconds = time % 60;//Use the euclidean division for the seconds.

        minutes = time / 120; //Divide the guiTime by sixty to get the minutes.

        if (minutes > 59)
        {
            minutes = 0;
            hours++;
            time = 0.0f;
        }

        //update the label value
        // timerLabel.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        // timerLabel.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minutes, seconds);
        timerLabel.text = string.Format("{0:00} : {1:00}", hours, minutes);
    }

    public void SetTimescale()
    {
        //timeScale = slider.value;
        Time.timeScale = slider.value;
    }
}