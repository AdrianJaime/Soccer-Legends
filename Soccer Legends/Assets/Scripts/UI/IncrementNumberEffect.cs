using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncrementNumberEffect : MonoBehaviour
{
    [SerializeField] float duration = 1;

    private int actualNumber, goalNumber,incrementNumber;
    private Text scoreUI;

    private void Start()
    {
        scoreUI = gameObject.GetComponent<Text>();
        StartIncrementEffect(10000, 5000,false);
    }

    public void StartIncrementEffect(int _startNumber, int _goalNumber, bool _increment) 
    {
        actualNumber = _startNumber; goalNumber = _goalNumber;

        int increment = (goalNumber - actualNumber);

        float timeIncrement = duration / increment;

        if (timeIncrement < 0.01)
            incrementNumber = (int) (100/duration);
        else if (timeIncrement < 0.1)
            incrementNumber = (int)(10/duration);

        if(_increment) StartCoroutine(ScoreUpdaterUp(timeIncrement));
        else StartCoroutine(ScoreUpdaterDown(timeIncrement));
    }

    private IEnumerator ScoreUpdaterUp(float _incrementTimer)
    {
        while (actualNumber < goalNumber)
        {
            actualNumber+=incrementNumber; 
            scoreUI.text = actualNumber.ToString(); 
            yield return new WaitForSeconds(_incrementTimer);
        }
        scoreUI.text = goalNumber.ToString();
    }
    IEnumerator ScoreUpdaterDown(float _incrementTimer)
    {
        while (actualNumber > goalNumber)
        {
            actualNumber-=incrementNumber; 
            scoreUI.text = actualNumber.ToString(); 
            yield return new WaitForSeconds(_incrementTimer);
        }
        scoreUI.text = goalNumber.ToString();
    }
}
