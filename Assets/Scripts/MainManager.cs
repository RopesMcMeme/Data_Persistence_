using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text PlayerName;
    [SerializeField] TMP_Text BestScore;
    public GameObject GameOverText;
    public GameObject PauseText;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    private bool m_IsPaused = false;

  
    // Start is called before the first frame update
    void Start()
    {

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        PlayerName.text = "Player Name: " + PlayerDataManager.Instance.playerName;
        SetBestPlayer();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
            
        }
        
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }
        PauseGame();
    }

   
    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        PlayerDataManager.Instance.score = m_Points;
    }

    public void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && m_IsPaused == false)
        {
            PauseText.SetActive(true);
            Time.timeScale = 0;
            m_IsPaused = true;                                  
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && m_IsPaused == true)
        {
            PauseText.SetActive(false);
            Time.timeScale = 1;
            m_IsPaused = false; 
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        CheckBestPlayer();
        GameOverText.SetActive(true);
    }

    public void ReturnToStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void CheckBestPlayer()
    {
        if(PlayerDataManager.Instance.score > PlayerDataManager.Instance.bestScore)
        {
            PlayerDataManager.Instance.bestScore = PlayerDataManager.Instance.score;
            PlayerDataManager.Instance.bestPlayer = PlayerDataManager.Instance.playerName;
            PlayerDataManager.Instance.leaderboard.Insert(0, PlayerDataManager.Instance.bestPlayer + ": " + PlayerDataManager.Instance.bestScore);
        }
        PlayerDataManager.Instance.SavePlayerData(PlayerDataManager.Instance.bestPlayer, PlayerDataManager.Instance.bestScore, PlayerDataManager.Instance.leaderboard);
    }

    public void SetBestPlayer()
    {
        if(PlayerDataManager.Instance.bestPlayer == null && PlayerDataManager.Instance.bestScore == 0)
        {
            BestScore.text = "  ";
        }
        else
        {
            BestScore.text = "Best Score: " + PlayerDataManager.Instance.bestPlayer + ": " + PlayerDataManager.Instance.bestScore;
        }
    }

    
}
