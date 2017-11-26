using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus
{
    next, play, gameover, win
}
public class GameManager : Singleton<GameManager> {
    //UI
    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyLbl;
    [SerializeField]
    private Text currentWaveLbl;
    [SerializeField]
    private Text totalEscapedLbl;
    [SerializeField]
    private Text playBtnLbl;
    [SerializeField]
    private Button playBtn;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private int EnemiesToSpawn = 0;
    public int TotalEscaped { get { return totalEscaped; } set { totalEscaped = value; } }
    public int RoundEscaped { get { return roundEscaped; } set { roundEscaped = value; } }
    public int TotalKilled { get { return totalKilled; } set { totalKilled = value; } }
    public int TotalMoney {
        get { return totalMoney; }
        set {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
            }
    }
    //GamePlay
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;
    
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;

    private GameStatus currentState = GameStatus.play;
    private AudioSource audioSource;
    
    const float spawnDelay = 0.5f;

    public List<Enemy> EnemyList = new List<Enemy>();

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        playBtn.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        ShowMenu();
	}

    private void Update()
    {
        HandleEscape();
    }


    IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (EnemyList.Count < totalEnemies)
                {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0,EnemiesToSpawn)]);
                    newEnemy.transform.position = spawnPoint.transform.position;
                   
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }
        EnemyList.Clear();
    }
   public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }
    public void SubtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public AudioSource AudioSource
    {
        get { return audioSource; }
    }

    public void IsWaveOver()
    {
        totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
        if (RoundEscaped + TotalKilled == totalEnemies)
        {
            if(waveNumber <= enemies.Length)
            {
                EnemiesToSpawn = waveNumber;
            }
            setCurrentGameState();
            ShowMenu();
        }
    }

    public void setCurrentGameState()
    {
        if(TotalEscaped >= 10)
        {
            currentState = GameStatus.gameover;
        }else if(waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
        {
            currentState = GameStatus.play;
        }else if(waveNumber >= totalWaves)
        {
            currentState = GameStatus.win;
        }
        else
        {
            currentState = GameStatus.next;
        }
    }
    public void ShowMenu()
    {
        switch (currentState)
        {
            case GameStatus.gameover:
                GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                playBtnLbl.text = "Play Again!";
                break;
            case GameStatus.next:
                playBtnLbl.text = "Next Wave";
                break;
            case GameStatus.play:
                playBtnLbl.text = "Play";
                break;
            case GameStatus.win:
                playBtnLbl.text = "Play";
                break;

        }
        playBtn.gameObject.SetActive(true);
    }


    public void PlayBtnPressed()
    {
        switch (currentState)
        {
            case GameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;
                break;
            default:
                EnemiesToSpawn = 0;
                totalEnemies = 3;
                TotalEscaped = 0;
                totalMoney = 10;
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapedLbl.text = "Escaped" + TotalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instance.Newgame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLbl.text = "Wave " + (waveNumber + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.DisableDragSprite();
            TowerManager.Instance.towerBtnPressed = null;
        }
    }


}
