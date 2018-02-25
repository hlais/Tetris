using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour {


    Board m_gameBoard;
    Spawner m_spwaner;

    //currently active shape
    Shape m_activeShape;

    public float m_dropInterval = 0.90f;
    //represents float in game time where next shape will happed

    float m_timeToDrop;
    
    float m_dropIntervalModded;

    float m_timeToNextKey;
    /*

    //this determines how quick the button will move across
    [Range(0.02f,1f)] public float m_keyRepeatRate = 0.068f;
    */

    float m_timeToNextKeyLeftRight;

    public ParticelPlayer m_gameOverFx;
    

    [Range(0.02f, 1f)] public float m_keyReaptRateLeftRight = 0.15f;
    float m_timeToNextKeyDown;

    [Range(0.01f, 1f)] public float m_keyRepeatRateDown = 0.01f;

    float m_timeToNextKeyRotate;

    [Range(0.02f, 1f)] public float m_keyRepeatRateRotate = 0.25f;
    bool m_gameOver = false;
    public GameObject m_gameOverPanel;

    SoundManager m_soundManager;

    //ghost for visualisation
    Ghost m_ghost;

    public IconToggle m_rotIconToggle;

    bool m_clockwise = true;

    public bool m_isPaused = false;

    public GameObject m_pausePanel;

    ScoreManager m_scoreManager;

    //shape holder
    Holder m_holder;

    //public Text diagnostext3;

    enum Direction { none,left,right,up,down}

    Direction m_dragDirection = Direction.none;
    Direction m_swipeDirection = Direction.none;

    float m_timeToNextSwipe;
    float m_timeToNextDrag;

    [Range(0.05f, 1f)]
    public float m_minTimeToDrag = 0.15f;

    [Range(0.05f, 1f)]
    public float m_minTimeToSwipe = 0.3f;

    bool m_didTap = false;

    private void OnEnable()
    {
        TouchController.DragEvent += DragHandler;
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.TapEvent += TapHandler;
    }

    private void OnDisable()
    {
        TouchController.DragEvent -= DragHandler;
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.TapEvent -= TapHandler;
    }




    void Start()
    {
       
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spwaner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_scoreManager =  GameObject.FindObjectOfType<ScoreManager>();
        m_ghost = GameObject.FindObjectOfType<Ghost>();
        m_holder = GameObject.FindObjectOfType<Holder>();

        //or the below is the same (But Slower according to UNITY docs)
        //m_gameBoard = FindObjectOfType<Board>();
        //m_spwaner = FindObjectOfType<Spawner>();

        m_timeToNextKeyLeftRight = Time.time + m_keyReaptRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_timeToNextKeyRotate;

        if (!m_scoreManager)
        {
            Debug.Log("There is no SCORE MANAGER defined");
        }

        if (!m_gameBoard)
        {
            Debug.Log("There is no game Board defined");
        }
        if (!m_soundManager)
        {
            Debug.LogWarning("Warning! There is no sound manager");

        }
        if (!m_spwaner)
        {
            Debug.Log("There is no Spawner defined");

        }
        else
        {
            m_spwaner.transform.position = VectorF.Round(m_spwaner.transform.position);
            //this will spawn shapes 
            if (!m_activeShape)
            {
                //will make shape spawn from top
                m_activeShape = m_spwaner.SpawnShape();
            }
        }
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }
        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }
        m_dropIntervalModded = m_dropInterval;

        /*
        if (diagnostext3)
        {
            diagnostext3.text = "";
        }
        */


    }
    void PlayerInput()
    {
        if ((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            MoveRight();
        }

        else if ((Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if (Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            Rotate();
        }
        else if ((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown)) || (Time.time > m_timeToDrop))
        {
            MoveDown();
        }

        // Touch controllers --------------------------------
        else if ((m_swipeDirection == Direction.right && Time.time > m_timeToNextSwipe) || 
            (m_dragDirection == Direction.right && Time.time > m_timeToNextDrag))
        {
            MoveRight();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
            //m_swipeDirection = Direction.none;
            //m_swipeEndDirection = Direction.none;
        }
        else if ((m_swipeDirection == Direction.left && Time.time > m_timeToNextSwipe) ||
            (m_dragDirection == Direction.left && Time.time > m_timeToNextDrag))
        {
            MoveLeft();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;

            // m_swipeDirection = Direction.none;
            //m_swipeEndDirection = Direction.none;
        }
        //rotae movement we just want a tap//  <<SPACE BUSTER??>>
        else if ((m_swipeDirection == Direction.up && Time.time > m_timeToNextSwipe) || (m_didTap))
        {
            Rotate();
            
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
            m_didTap = false;

            //m_swipeEndDirection = Direction.none;
        }

        else if (m_dragDirection == Direction.down && Time.time > m_timeToNextDrag)
        {
            MoveDown();

           // m_swipeDirection = Direction.none;
        }


        //end touch controlls ============
        else if (Input.GetButtonDown("ToggleRot"))
        {
            ToggleRotDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }

        m_dragDirection = Direction.none;
        m_swipeDirection = Direction.none;
        m_didTap = false;
    }

    private void MoveDown()
    {
        m_timeToDrop = Time.time + m_dropIntervalModded;
        //if there is an active shape call the move Down method on the shape
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_activeShape.MoveDown();

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            //before we move to an invalid position we will check if its overlimit
            if (m_gameBoard.IsOverLimit(m_activeShape))
            {
                GameOver();
            }
            else
            {
                //shape landing
                LandShape();
            }
        }
    }

    private void Rotate()
    {
        //m_activeShape.RotateRight();
        m_activeShape.RotateClockwise(m_clockwise);
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            // m_activeShape.RotateLeft();
            m_activeShape.RotateClockwise(!m_clockwise);
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    private void MoveLeft()
    {
        m_activeShape.MoveLeft();
        m_timeToNextKeyLeftRight = Time.time + m_keyReaptRateLeftRight;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveRight();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    private void MoveRight()
    {
        m_activeShape.MoveRight();
        m_timeToNextKeyLeftRight = Time.time + m_keyReaptRateLeftRight;



        //if its invalid position move it back 
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveLeft();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    void LandShape()
    {
            
            m_activeShape.MoveUp();

            m_gameBoard.StoreShapeInGrid(m_activeShape);

            m_activeShape.LandShapeFX();

            if (m_ghost)
            {
                m_ghost.Reset();
            }
            if (m_holder)
            {
                m_holder.m_canRelease = true;
            }
            m_activeShape = m_spwaner.SpawnShape();

            //as soon as shape land we can press key and not wait for another key repeat reate
            m_timeToNextKeyLeftRight = Time.time;
            m_timeToNextKeyDown = Time.time;
            m_timeToNextKeyRotate = Time.time;

            //once shape lands to check if its compete
            m_gameBoard.StartCoroutine("ClearAllRows");

            PlaySound(m_soundManager.m_dropSound, 0.15f);

            if (m_gameBoard.m_completedRows > 0)
            {
                m_scoreManager.ScoreLInes(m_gameBoard.m_completedRows);

                if (m_scoreManager.m_didLevelUP)
                {
                    PlaySound(m_soundManager.m_levelUpClip, 5f);
                    m_dropIntervalModded = Mathf.Clamp(m_dropInterval - (((float)m_scoreManager.m_level - 1) * 0.05f), 0.05f, 1f);
                }
                else
                {
                    if (m_gameBoard.m_completedRows > 1)
                    {
                        AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
                        PlaySound(randomVocal, 2.0f);
                    }
                }

                PlaySound(m_soundManager.m_clearRawSound, 0.65f);
            }
     
    }


    void Update() {
        
        //if these are not present we wont ront further code. INput
        if (!m_gameBoard || !m_spwaner || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager)
        {
            return;
        }
        PlayerInput();
    }

    private void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape,m_gameBoard);
        }
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
    void GameOver()
    {
        m_activeShape.MoveUp(); //stops shapes moving into each other
        

        StartCoroutine(GameOverRoutine());

      
        //put you suckers under Clip
        PlaySound(m_soundManager.m_gameOverVocalClip, 0.90f);
        //insert gameOVer MUsic
        PlaySound(m_soundManager.m_gameOverSound, 5f);
        /////
        m_gameOver = true;


    }
    IEnumerator GameOverRoutine()
    {
        if (m_gameOverFx)
        {
            m_gameOverFx.Play();
        }
        yield return new WaitForSeconds(0.3f);

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }
    }
    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (clip && m_soundManager.m_fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position,Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier, 0.2f,1.0f));
        }
    }

    public void ToggleRotDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
        {
            m_rotIconToggle.ToggleIcon(m_clockwise);

        }
    }
    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }
        m_isPaused = !m_isPaused;
        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.50f : m_soundManager.m_musicVolume;

            }
            Time.timeScale = (m_isPaused) ? 0 : 1;
        }
    }
    public void QuittingGame()
    {
        Application.Quit();
        Debug.Log("Application is quitting");
    }
    public void Hold()
    {
        if (!m_holder)
        {
            return;
        }
        if (!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spwaner.SpawnShape();
            PlaySound(m_soundManager.m_holdSound, 0.75f);
        }
        else if (m_holder.m_canRelease)
        {
            Shape shape = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spwaner.transform.position;
            m_holder.Catch(shape);
            PlaySound(m_soundManager.m_holdSound,0.75f);
        }
        else
        {
            Debug.LogWarning("HOllder warning, Wainting for cool down");
            PlaySound(m_soundManager.m_errorSound, 0.75f);
        }

        if (m_ghost)
        {
            m_ghost.Reset();
        }
    }
    /// <summary>
    /// below method is working with new touch controls
    /// </summary>
    /// <param name="swipeMovement"></param>
    void DragHandler(Vector2 dragMovement)
    {

        m_dragDirection = GetDirection(dragMovement);
    }
    void SwipeHandler(Vector2 swipeMovement)
    {
     
        m_swipeDirection = GetDirection(swipeMovement);
    }
    void TapHandler(Vector2 tapMovement)
    {

        m_didTap = true;
    }
    Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.none;
        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDir = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
        }
        else

        {
            swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
        }
        return swipeDir;
    }
}
