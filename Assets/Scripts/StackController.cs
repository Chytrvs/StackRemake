﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackController : MonoBehaviour {
    public float FallSpeed=5f;
    public float TileSpeed = 5f;
    public float TileScaleSpeed = 5f;
    public GameObject Tile;
    public GameObject HighestTile;
    public GameObject MovingTile;
    private Vector3 EndPos;
    private bool isMovingOnXaxis=true;
    private float Displacement;
    public float DisplacementTolerance;
    private float MaxTileScale;
    private GameObject Leftover;
    private float hue;
    private int combo;
    Vector3 DesiredScale;
    private int Score = -1;
    public Text ScoreTxt, HighScoreTxt;
    public GameObject GameOverPanel;
    public float ZoomOutSpeed;
    bool IsGameOver;


    
	// Use this for initialization
	void Start () {
        hue = Random.Range(0, 10) * 0.1f;
        Tile = transform.GetChild(transform.childCount-1).gameObject;
        MaxTileScale = Tile.transform.localScale.x;
        HighestTile = Tile;
        DesiredScale = HighestTile.transform.localScale;
        SpawnNextTile();
        foreach(Transform tile in transform)
        {
            SetColor(tile.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //SetColor();
        if (!IsGameOver)
        {
            TouchInputHandle();
            MoveStackDown();
            if (combo >= 3)
                HighestTile.transform.localScale = Vector3.Lerp(HighestTile.transform.localScale, DesiredScale,  TileScaleSpeed * Time.deltaTime);
            if (isMovingOnXaxis)
            {
                MoveTileX();
            }
            else
            {
                MoveTileZ();
            }
        }
        else
            ZoomOutCamera();


	}
    void InputHandle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTile();
            if(!IsGameOver)
            SpawnNextTile();
        }
    }
    void TouchInputHandle()
    {
        if (Input.touchCount > 0)
        {
            Touch tap = Input.GetTouch(0);
            if (tap.phase == TouchPhase.Began)
            {
                PlaceTile();
                if (!IsGameOver)
                    SpawnNextTile();
            }

        }
    }
    void PlaceTile()
    {
        if (isMovingOnXaxis)
        {
            Displacement = Mathf.Abs(HighestTile.transform.position.x - MovingTile.transform.position.x);
            if (Displacement <= DisplacementTolerance)
            {
                MovingTile.transform.position = new Vector3(HighestTile.transform.position.x, MovingTile.transform.position.y, HighestTile.transform.position.z);
                HighestTile = MovingTile;
                isMovingOnXaxis = !isMovingOnXaxis;
                HighestTile.GetComponent<ParticleSystem>().Emit(500);
                combo++;
            }
            else if (Displacement > HighestTile.transform.localScale.x)
            {
                MovingTile.AddComponent<Rigidbody>();
                GameOver();
            }
            else
            {
                if (MovingTile.transform.localPosition.x > HighestTile.transform.localPosition.x) //Checks on which side leftover should fall
                {
                    Leftover = Instantiate(MovingTile, new Vector3(MovingTile.transform.localPosition.x+(MovingTile.transform.localScale.x-Displacement)/2,MovingTile.transform.position.y,MovingTile.transform.position.z),Quaternion.identity,transform);
                }
                else
                {
                    Leftover = Instantiate(MovingTile, new Vector3(MovingTile.transform.localPosition.x + -(MovingTile.transform.localScale.x - Displacement) / 2, MovingTile.transform.position.y, MovingTile.transform.position.z), Quaternion.identity, transform);
                }
                Leftover.transform.localScale = new Vector3(Displacement, MovingTile.transform.localScale.y, MovingTile.transform.localScale.z);
                Leftover.AddComponent<Rigidbody>();
                MovingTile.transform.localScale = new Vector3(MovingTile.transform.localScale.x - Displacement, MovingTile.transform.localScale.y, HighestTile.transform.localScale.z);
                MovingTile.transform.position = new Vector3((MovingTile.transform.position.x + HighestTile.transform.position.x) / 2, MovingTile.transform.position.y, HighestTile.transform.position.z);
                HighestTile = MovingTile;
                isMovingOnXaxis = !isMovingOnXaxis;
                combo = 0;

            }
        }
        else
        {
            Displacement = Mathf.Abs(HighestTile.transform.position.z - MovingTile.transform.position.z);
            if (Displacement <= DisplacementTolerance)
            {
                MovingTile.transform.position = new Vector3(HighestTile.transform.position.x, MovingTile.transform.position.y, HighestTile.transform.position.z);
                HighestTile = MovingTile;
                isMovingOnXaxis = !isMovingOnXaxis;
                HighestTile.GetComponent<ParticleSystem>().Emit(500);
                combo++;
            }
            else if (Displacement > HighestTile.transform.localScale.z)
            {
                MovingTile.AddComponent<Rigidbody>();
                GameOver();
                
            }
            else
            {
                if (MovingTile.transform.localPosition.z > HighestTile.transform.localPosition.z) //Checks on which side leftover should fall
                {
                    Leftover = Instantiate(MovingTile, new Vector3(MovingTile.transform.position.x, MovingTile.transform.position.y, MovingTile.transform.localPosition.z + (MovingTile.transform.localScale.z - Displacement) / 2), Quaternion.identity, transform);
                }
                else
                {
                    Leftover = Instantiate(MovingTile, new Vector3(MovingTile.transform.position.x, MovingTile.transform.position.y, MovingTile.transform.localPosition.z + -(MovingTile.transform.localScale.z - Displacement) / 2), Quaternion.identity, transform);
                }
                Leftover.AddComponent<Rigidbody>();
                Leftover.transform.localScale = new Vector3(MovingTile.transform.localScale.x, MovingTile.transform.localScale.y, Displacement);
                MovingTile.transform.localScale = new Vector3(HighestTile.transform.localScale.x, MovingTile.transform.localScale.y, MovingTile.transform.localScale.z-Displacement);
                MovingTile.transform.position = new Vector3(HighestTile.transform.position.x, MovingTile.transform.position.y,(MovingTile.transform.position.z + HighestTile.transform.position.z) / 2);
                HighestTile = MovingTile;
                isMovingOnXaxis = !isMovingOnXaxis;
                combo = 0;
            }
        }
        CheckCombo();


    }
    void SpawnNextTile()
    {
        Score++;
        ScoreTxt.text = Score.ToString();
        if (isMovingOnXaxis)
        {
        MovingTile = Instantiate(HighestTile,HighestTile.transform.position+new Vector3(-HighestTile.transform.localScale.x,1,0), Quaternion.identity, transform) as GameObject;
            MovingTile.transform.localScale = DesiredScale;
            SetColor(MovingTile);
        }
        else
        {
        MovingTile = Instantiate(HighestTile, HighestTile.transform.position + new Vector3(0, 1, -HighestTile.transform.localScale.z), Quaternion.identity, transform) as GameObject;
            MovingTile.transform.localScale = DesiredScale;
            SetColor(MovingTile);
        }
        EndPos += Vector3.down;
    }
    void MoveStackDown() //Stack is falling down every time user places a tile (which lowers EndPos)
    {
        transform.position = Vector3.Lerp(transform.position, EndPos, FallSpeed * Time.deltaTime);
    }
    void MoveTileX() //Moves tile back and forth on X axis
    {
        MovingTile.transform.position = new Vector3(1.4f*(HighestTile.transform.position.x + Mathf.PingPong(Time.time * TileSpeed, MaxTileScale * 2) - MaxTileScale), MovingTile.transform.position.y, MovingTile.transform.position.z);
    }
    void MoveTileZ() //Moves tile back and forth on Z axis
    {
        MovingTile.transform.position = new Vector3(MovingTile.transform.position.x, MovingTile.transform.position.y, 1.4f*(HighestTile.transform.position.z + Mathf.PingPong(Time.time * TileSpeed, MaxTileScale*2) - MaxTileScale));
    }
    void SetColor(GameObject tile) //Sets higher hue for each tile to create gradient effect
    {
        if (hue < 1)
        {
        hue += 0.015f;
        }
        else
        {
            hue = 0;
        }
        float sat=0.6f;
        float val=0.6f;
        tile.GetComponent<Renderer>().material.color=Color.HSVToRGB(hue, sat, val);
    }
    void CheckCombo() //Checks if tile scale should be increased based on current combo
    {
        if (combo >= 3)
        {
            if(HighestTile.transform.localScale.x + 0.5f <=MaxTileScale||HighestTile.transform.localScale.x + 0.5f <=MaxTileScale)
            DesiredScale = new Vector3(HighestTile.transform.localScale.x + 0.5f, 1, HighestTile.transform.localScale.z + 0.5f);
            else
            {
                DesiredScale = new Vector3(MaxTileScale, 1, MaxTileScale);
            }
        }
        else
        {
            DesiredScale = HighestTile.transform.localScale;
        }
    }
    void GameOver()
    {
        if (!PlayerPrefs.HasKey("HighScore"))
           PlayerPrefs.SetInt("HighScore", 0);
        IsGameOver = true;
        GameOverPanel.SetActive(true);
        if (Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Score);
            HighScoreTxt.text ="BEST "+ PlayerPrefs.GetInt("HighScore").ToString();
        }
        HighScoreTxt.text = "BEST " + PlayerPrefs.GetInt("HighScore").ToString();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void PlayAgain()
    {
        Application.LoadLevel(0);
        Time.timeScale = 1;
    }
    void ZoomOutCamera()
    {
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Score+ 7, ZoomOutSpeed*Time.deltaTime);
    }
}
