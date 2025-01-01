using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 10f;
    private float currentReaminingTime;

    // Start is called before the first frame update
    void Start()
    {
        currentReaminingTime = remainingTime;
        timerText.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentReaminingTime > 0)
        {
            currentReaminingTime -= Time.deltaTime;
        }
        else if (currentReaminingTime < 0)
        {
            currentReaminingTime = 0;
            timerText.color = Color.green; // Change the color to green
            StartCoroutine(DisableAfterFrame()); // Disable the GameObject after a short delay
        }
        int minutes = Mathf.FloorToInt(currentReaminingTime / 60);
        int seconds = Mathf.FloorToInt(currentReaminingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator DisableAfterFrame()
    {
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false); // Disable the GameObject
    }

    private void OnEnable()
    {
        currentReaminingTime = remainingTime;
        timerText.color = Color.red;
    }
}
