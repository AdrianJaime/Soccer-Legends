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
        StartIncrementEffect(0, 10000);
    }

    public void StartIncrementEffect(int _startNumber, int _goalNumber) 
    {
        actualNumber = _startNumber; goalNumber = _goalNumber;

        int increment = (goalNumber - actualNumber);

        float timeIncrement = duration / increment;

        if (timeIncrement < 0.01)
            incrementNumber = (int) (100/duration);
        else if (timeIncrement < 0.1)
            incrementNumber = (int)(10/duration);

        StartCoroutine(ScoreUpdater(timeIncrement));
    }

    private IEnumerator ScoreUpdater(float _incrementTimer)
    {
        while (actualNumber < goalNumber)
        {
            actualNumber+=incrementNumber; 
            scoreUI.text = actualNumber.ToString(); 
            yield return new WaitForSeconds(_incrementTimer);
        }
        scoreUI.text = goalNumber.ToString();
    }
}
