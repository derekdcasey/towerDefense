using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private float navigationUpdate;
    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private int rewardAmount;


    private int target = 0;

    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator anim;

    private float navigationTime = 0;
    private bool isDead = false;

    public bool IsDead
    {
        get { return isDead;}
    }

	// Use this for initialization
	void Start () {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(wayPoints != null && !isDead)
        {
            navigationTime += Time.deltaTime;
            if(navigationTime > navigationUpdate)
            {
                if(target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
                }
                navigationTime = 0;
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            target += 1;
        }else if(collision.CompareTag("Finish"))
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.IsWaveOver();


        }
        else if (collision.CompareTag("Projectile"))
        {
            Projectile p = collision.gameObject.GetComponent<Projectile>();
            EnemyHit(p.AttackStrength);
            Destroy(collision.gameObject);
        }
    }

    public void EnemyHit(int hitpoints)
    {
        if (healthPoints - hitpoints > 0)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
            anim.Play("Hurt");
            healthPoints -= hitpoints;
        }
        else
        {
            anim.SetTrigger("Died");
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
            Die();
        }
    }
    public void Die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        GameManager.Instance.TotalKilled += 1;
        GameManager.Instance.AddMoney(rewardAmount);
        GameManager.Instance.IsWaveOver();
    }

}
