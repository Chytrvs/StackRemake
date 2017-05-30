using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour {
    public float FallSpeed=5f;
    public float TileSpeed = 5f;
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
	// Use this for initialization
	void Start () {
        hue = Random.Range(0, 10) * 0.1f;
        Tile = transform.GetChild(transform.childCount-1).gameObject;
        MaxTileScale = Tile.transform.localScale.x;
        HighestTile = Tile;
        SpawnNextTile();
        foreach(Transform tile in transform)
        {
            SetColor(tile.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //SetColor();
        InputHandle();
        MoveStackDown();
        if (isMovingOnXaxis)
        {
            MoveTileX();
        }
        else
        {
            MoveTileZ();
        }
	}
    void InputHandle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTile();
            SpawnNextTile();
        }
;
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
            }
            else if (Displacement > HighestTile.transform.localScale.x)
            {
                Debug.Log("lose");
               
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
            }
            else if (Displacement > HighestTile.transform.localScale.z)
            {
                Debug.Log("lose");
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
            }
        }


    }
    void SpawnNextTile()
    {
        if (isMovingOnXaxis)
        {
        MovingTile = Instantiate(HighestTile,HighestTile.transform.position+new Vector3(-HighestTile.transform.localScale.x,1,0), Quaternion.identity, transform) as GameObject;
            SetColor(MovingTile);
        }
        else
        {
        MovingTile = Instantiate(HighestTile, HighestTile.transform.position + new Vector3(0, 1, -HighestTile.transform.localScale.z), Quaternion.identity, transform) as GameObject;
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
        MovingTile.transform.position = new Vector3(HighestTile.transform.position.x + Mathf.PingPong(Time.time * TileSpeed, MaxTileScale * 2) - MaxTileScale, MovingTile.transform.position.y, MovingTile.transform.position.z);
    }
    void MoveTileZ() //Moves tile back and forth on Z axis
    {
        MovingTile.transform.position = new Vector3(MovingTile.transform.position.x, MovingTile.transform.position.y, HighestTile.transform.position.z + Mathf.PingPong(Time.time * TileSpeed, MaxTileScale*2) - MaxTileScale);
    }
    void SetColor(GameObject tile)
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
        tile.GetComponent<Renderer>().material.color =Color.HSVToRGB(hue, sat, val);
    }
}
