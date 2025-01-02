using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : Singleton<Utils>
{

    public IEnumerator ChangeColorAndDisappear(GameObject objectTochangeItsColor, Color targetColor, float duration)
    {
        //TODO : change the color of the water to slowely become green and slowely return to the origin color

        Material attackMaterial = objectTochangeItsColor.GetComponent<Renderer>().material;
        Color initialColor = attackMaterial.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            attackMaterial.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After changing to pink, start disappearing
        elapsedTime = 0f;
        Color transparentColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

        while (elapsedTime < duration)
        {
            attackMaterial.color = Color.Lerp(targetColor, transparentColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(objectTochangeItsColor); // Destroy the attack object after it disappears
    }


    //TODO : make the timer to be above the gameObjectToAddTimerTo
    public void StartTimerAbove(GameObject gameObjectToAddTimerTo)
    {
      
        GameObject timer = FindInactiveObjectByName("Timer");
        timer.SetActive(true);
    }




    public GameObject FindInactiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform obj in objs)
        {
            if (obj.hideFlags == HideFlags.None && obj.name == name)
            {
                return obj.gameObject;
            }
        }
        return null;
    }
}
