using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkPlayer : MonoBehaviour
{
    [SerializeField] private GameObject pinkAttackPrefab;
    [SerializeField] private KeyCode attackKey;
    [SerializeField] private Color colorOfTheAttack = Color.magenta;

    private GameObject currentAttack;
    private bool isUnderAttack = false;
    private bool isAttacking = false;
    private bool isHaveAnAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.StartListening(EventManager.EVENT_ADD_ATTACK_TO_PINK_PLAYER, AddAttack);
        EventManager.Instance.StartListening(EventManager.EVENT_PINK_PLAYER_HIT_FROM_ATTACK, HitFromAttack);
        EventManager.Instance.StartListening(EventManager.EVENT_PINK_PLAYER_DIE, Die);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(attackKey) && isHaveAnAttack)
        {
            Attack(gameObject);
        }
    }

    private void Attack(GameObject obj)
    {
        if (!isUnderAttack && isHaveAnAttack && !isAttacking)
        {
            isAttacking = true;
            isHaveAnAttack = false;
            Debug.Log("Pink player is attacking");
            currentAttack.transform.Rotate(0, 0, 45);
            StartCoroutine(Utils.Instance.ChangeColorAndDisappear(currentAttack, colorOfTheAttack, GameManager.Instance.GetTimeOfAttack()/2f));
            EventManager.Instance.TriggerEvent(EventManager.EVENT_GREY_PLAYER_HIT_FROM_ATTACK, gameObject);
            GameManager.Instance.HitEnemiesInTheWater();
            StartCoroutine(ChangeIsHaveAnAttack());
        }
    }


    private void AddAttack(GameObject obj)
    {
        Debug.Log("Pink player is adding attack");
        if (!isHaveAnAttack && !isUnderAttack && !isAttacking)
        {
            isHaveAnAttack =true;
            currentAttack = Instantiate(pinkAttackPrefab, transform.position + new Vector3(0,1.2f,0), Quaternion.identity);
            currentAttack.transform.SetParent(transform);
        }

    }

    private void HitFromAttack(GameObject obj)
    {
        //TODO : start animation
        Utils.Instance.StartTimerAbove(gameObject);

        isUnderAttack = true;
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        StartCoroutine(playerMovement.MovePrevent(GameManager.Instance.GetTimeOfAttack()));
        StartCoroutine(ChangeIsUnderAttack());

    }

    private IEnumerator ChangeIsUnderAttack()
    {
        yield return new WaitForSeconds(GameManager.Instance.GetTimeOfAttack());
        isUnderAttack = false;
    }

    private IEnumerator ChangeIsHaveAnAttack()
    {
        yield return new WaitForSeconds(GameManager.Instance.GetTimeOfAttack());
        isHaveAnAttack = false;
        isAttacking = false;
    }

    private void Die(GameObject arg0)
    {
        Debug.Log("Pink player is dead");
    }

}
