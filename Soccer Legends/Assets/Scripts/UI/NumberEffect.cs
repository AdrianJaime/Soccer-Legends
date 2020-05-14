using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberEffect : MonoBehaviour
{
    public bool enabled;
    [SerializeField] bool local;

    public void StartEffect() 
    {
        Text scoreUI = gameObject.GetComponent<Text>();
        string goalText = scoreUI.text;
        string goalNumber = local ? 
            goalText.Substring(4, goalText.Length - 4) : goalText.Substring(0, goalText.Length - 4);

        char[] randomChars = goalNumber.ToCharArray();
        string statText = local ? goalText.Substring(0, 4) : goalText.Substring(goalText.Length - 4, 4);

        transform.parent.parent.parent.localScale = local ? new Vector3(0.65f, 1, 1) : new Vector3(-0.65f, 1, 1);

        enabled = true;
        StartCoroutine(RandomEffect(scoreUI, goalText, randomChars, statText));
    }

    private IEnumerator RandomEffect(Text scoreUI, string goalText, char[] randomChars, string statText)
    {
        while(enabled)
        {
            string randomNumber ="";
            for (int i = 0; i < randomChars.Length; i++)
            {
                randomChars[i] = char.Parse(Random.Range(0, 10).ToString());
                randomNumber += randomChars[i];
            }

            scoreUI.transform.GetChild(0).GetComponent<Text>().text = scoreUI.text = local ? 
                statText + randomNumber : randomNumber + statText;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
