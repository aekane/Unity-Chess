using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInput : MonoBehaviour
{
    public SpriteRenderer boardSprite;
    public GameObject testPrefab;
    public GameObject[] peicesPrefab;
    
    bool player = true;
    Peice[,] boardState = new Peice[8,8];
    Vector3 boardMaxBounds;
    Vector3 boardMinBounds;
    Vector2Int selected = new Vector2Int(-1,-1);
    

    void Start() {
        for (int i = 0; i < boardState.GetLength(0); i++)
        {
            for (int j = 0; j < boardState.GetLength(1); j++)
            {
                boardState[j,i] = new Peice();
                boardState[j,i].Spawn(j, i, peicesPrefab, this.GetComponent<Transform>());
            }
        }
        //Getting the edges of the board sprite in Viewport Point
        boardMaxBounds = Camera.main.WorldToViewportPoint(boardSprite.bounds.max);
        boardMinBounds = Camera.main.WorldToViewportPoint(boardSprite.bounds.min);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 pos = new Vector2((Screen.width - Input.mousePosition.x)/ Screen.width, (Screen.height - Input.mousePosition.y)/ Screen.height);

            if(CheckInside(pos) && selected.x == -1)
                SetSelection(pos);

            else if (CheckInside(pos))
                MoveSelected(pos);
                
        }
    }

    void SetSelection(Vector2 pos){
        selected = new Vector2Int(Map(8, 0, boardMinBounds.x, boardMaxBounds.x, pos.x), Map(8, 0, boardMinBounds.y, boardMaxBounds.y, pos.y));
        if((boardState[selected.x,selected.y].currentType != Peice.PeiceType.AIR) && (boardState[selected.x,selected.y].isWhite == player))
            boardState[selected.x, selected.y].mySelf.GetComponent<SpriteRenderer>().color = Color.yellow;
        else
            selected.x = -1;
            //Debug.Log(arrayPos);
            //Debug.Log(boardState[arrayPos.x, arrayPos.y].currentType);
    }

    void MoveSelected(Vector2 pos){
        Vector2Int moveLoc = new Vector2Int(Map(8, 0, boardMinBounds.x, boardMaxBounds.x, pos.x), Map(8, 0, boardMinBounds.y, boardMaxBounds.y, pos.y));
        if (moveLoc != selected){
            if ((boardState[moveLoc.x,moveLoc.y].isWhite == player) && boardState[moveLoc.x,moveLoc.y].currentType != Peice.PeiceType.AIR)
                return;

            Peice[,] temp = boardState[selected.x,selected.y].Move(selected, moveLoc, boardState);           
            
            if(temp != null)
            {
                boardState = temp;
                boardState[moveLoc.x, moveLoc.y].mySelf.GetComponent<SpriteRenderer>().color = boardState[moveLoc.x, moveLoc.y].isWhite ? Color.white : Color.gray;
                selected.x = -1;
                player = !player;
            }
        }
        else if(moveLoc == selected)
        {
            boardState[selected.x, selected.y].mySelf.GetComponent<SpriteRenderer>().color = boardState[selected.x, selected.y].isWhite ? Color.white : Color.gray;
            selected.x = -1;
        }
    }

    bool CheckInside(Vector2 pos)
    {   
        //calculating if mouse press was inside the sprite
        return (pos.x > boardMinBounds.x && pos.x < boardMaxBounds.x && pos.y > boardMinBounds.y && pos.y < boardMaxBounds.y);
    }

    int Map(float nMin,float nMax,float oMin,float oMax, float value)
    {
        return Mathf.FloorToInt(Mathf.Lerp(nMin, nMax, Mathf.InverseLerp(oMin, oMax, value)));
    }
}
