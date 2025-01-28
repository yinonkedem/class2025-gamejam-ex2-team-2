using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScreenController : MonoBehaviour
{
    [SerializeField] private KeyCode nextScreenKey = KeyCode.Return;


    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // if (Input.GetKeyDown(nextScreenKey))
        // {
        //     ScreenChanger.Instance.StartTheGame();
        // }
    }
}
