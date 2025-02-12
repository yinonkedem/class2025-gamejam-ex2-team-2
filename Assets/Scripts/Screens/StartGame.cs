using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGme : MonoBehaviour
{

    [SerializeField] private Sprite secondScreen;
    [SerializeField] private KeyCode nextScreenKey = KeyCode.Return;
    private bool _isInFirstScrren;
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        // _player = Helpers.Instance.FindInactiveObjectByName("Player");
        // _player.SetActive(true);
        // //,ove up a liitle bit the player
        // _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + uploadPPlayerYPositionBy, _player.transform.position.z);
        _isInFirstScrren = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(nextScreenKey) && _isInFirstScrren)
        {
            //change source image in the canvas renderred
            // inable two images in the canvas with names: Open Screen 2 and Open Screen 1
            GameObject.Find("Open Screen 1").SetActive(false);
            GameObject.Find("Open Screen 2").SetActive(false);

            GetComponent<Image>().sprite = secondScreen;
            //GetComponent<SpriteRenderer>().sprite = secondScreen;
            //_player.SetActive(false);
            _isInFirstScrren = false;
            // _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - uploadPPlayerYPositionBy, _player.transform.position.z);

        }
        else if (Input.GetKeyDown(nextScreenKey) && !_isInFirstScrren)
        {
            ScreenChanger.Instance.StartTheGame();
        }
    }
}