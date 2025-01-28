using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int maxLife = 3;
    [SerializeField] private int decreaseLifeCountWhenGetHit = 1;
    [SerializeField] private GameObject lifeBar;
    [SerializeField] private float increaseEnemySpeed = 1.5f;
    [SerializeField] private float increaseEnemySize = 0.7f;
    
    private int currentLife;
    private BarController lifeBarController;
    private bool isDead = false; 
    private Animator _animator;
    private bool isHit;
    private int currentStage = 1;
    private int offsetStage = 10;
    
    private void Start()
    {
        currentLife = maxLife;
        lifeBarController = lifeBar.GetComponent<BarController>();
        lifeBarController.updateBar(currentLife,maxLife);
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Debug.Log("Current Life: " + currentLife);
        if (currentLife < (maxLife * 2 / 3 + offsetStage) && currentLife > (maxLife * 1/3 + offsetStage))
        {
            if (currentStage != 2)
            {
                currentStage = 2;
                StartCoroutine(startStage2());
            }
        }
        else if (currentLife <=(maxLife * 1 / 3 + offsetStage))
        {
            if (currentStage != 3)
            {
                currentStage = 3;
                StartCoroutine(startStage3());
            }
        }
    }

    private IEnumerator startStage2()
    {
        AudioController.Instance.PlaySwitchStage();
        
        transform.localScale += new Vector3(increaseEnemySize, increaseEnemySize, increaseEnemySize);
        GetComponent<EnemyMovement>().AddToSpeed(increaseEnemySpeed);
        
        _animator.SetBool("isChangingStage", true);
        yield return new WaitForSeconds(3.9f);
        _animator.SetBool("isChangingStage", false);
        lifeBarController.updateBarColor(new Color(1,0,0,0.9f));
        yield return new WaitForSeconds(1f);
       GetComponent<AttacksController>().StartBoltAttack();
    }
    
    private IEnumerator startStage3()
    {
        AudioController.Instance.PlaySwitchStage();
        _animator.SetBool("isChangingStage", true);
        yield return new WaitForSeconds(3.9f);
        _animator.SetBool("isChangingStage", false);

        lifeBarController.updateBarColor(new Color(1,0,0,1));
        yield return new WaitForSeconds(1f);
        GetComponent<AttacksController>().StartMiniEnemiesAttack();
    }

    
    
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the othertag is with tag shot socall to decrease life function
        if (collision.gameObject.CompareTag("Shot"))
        {
            StartCoroutine(WaitForHitAnimation());
            //desrtroy the shot
            Destroy(collision.gameObject);
            DecreaseLife();
        }
    }
    

    private void DecreaseLife()
    {
        currentLife -= decreaseLifeCountWhenGetHit;
        lifeBarController.updateBar(currentLife,maxLife);
        if (currentLife <=0)
        {
            StartCoroutine(WaitForDeathAnimation());
        }
        Debug.Log("Enemy life decreased");
    }
    
    // function that wait until call Die() for Animation clip death time
    private IEnumerator WaitForDeathAnimation()
    {
        AudioController.Instance.PlayEnemyDeath();
        isDead = true;
        _animator.SetBool("isDead", isDead);    
        
        Destroy(Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject));
        // Wait for the length of the death animation clip
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length+0.4f);
        
        // Call the Die method after the animation finishes
        StartCoroutine(Die());
    }
    
    private IEnumerator WaitForHitAnimation()
    {
        isHit = true;
        AudioController.Instance.PlayEnemyHit();
        _animator.SetBool("isHit", isHit);
//        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.2f);

        isHit = false;
        _animator.SetBool("isHit", isHit);

    }
    
    

    private IEnumerator Die()
    {
        AudioController.Instance.PlayEnemyDeath();
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length-0.2f);
        GameManager.Instance.ArePlayerWon = true;
        Debug.Log("Enemy is dead");
        ScreenChanger.Instance.ActivateWinningGame();
    }
    
    
}
