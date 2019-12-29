using UnityEngine;

public class Peice 
{
    public bool isWhite = true;
    public Vector2 postion;
    public enum PeiceType {POND, ROOK, KNIGHT, BISHOB, QUEEN, KING, AIR};
    public PeiceType currentType;
    public GameObject mySelf;
    Peice[,] _boardState;
    bool firstMove = true;

    public Peice(PeiceType setType = PeiceType.AIR)
    {
        currentType = setType;
    }

    public void Spawn(int indexX, int indexY, GameObject[] prefabs, Transform parent)
    {
        Vector3 ppos = new Vector3(indexX, indexY, 0);

        switch (indexY)
        {
            case 0:
                PlayerPeices(indexX, indexY, ppos, prefabs, parent);
                break;
            case 1:
                currentType = PeiceType.POND;
                mySelf = DesktopInput.Instantiate(prefabs[0], ppos, Quaternion.identity, parent);
                break;
            case 6:
                currentType = PeiceType.POND;
                isWhite = false;
                mySelf = DesktopInput.Instantiate(prefabs[0], ppos, Quaternion.identity, parent);
                mySelf.GetComponent<SpriteRenderer>().color = Color.gray;
                break;
            case 7:
                isWhite = false;
                PlayerPeices(indexX, indexY, ppos, prefabs, parent);
                break;

            default:
                break;
        }                  
    }

   
    public Peice[,] Move(Vector2Int currentPos, Vector2Int movePos, Peice[,] arr)
    {
        _boardState = arr;
        if(LegalMove(currentPos, movePos)){
            mySelf.transform.position = new Vector3(movePos.x,movePos.y,0);
            firstMove = false;

            if (_boardState[movePos.x,movePos.y].mySelf != null)
                DesktopInput.Destroy(_boardState[movePos.x,movePos.y].mySelf);

            _boardState[movePos.x,movePos.y] = _boardState[currentPos.x,currentPos.y];
            _boardState[currentPos.x,currentPos.y] = new Peice();
            return _boardState;
        }
        else
        {
            return null;
        }
    }
    private bool LegalMove(Vector2Int currentPos, Vector2Int movePos)
    {
        switch (currentType)
        {
            case PeiceType.POND:
                if(firstMove && (currentPos.x == movePos.x))
                {
                    int difference = (currentPos.y - movePos.y) < 0 ? 1 : -1;
                    return(((Mathf.Abs(movePos.y - currentPos.y) < 3) && (movePos.x == currentPos.x)) 
                            && _boardState[currentPos.x, currentPos.y + difference].currentType == PeiceType.AIR);
                }
                
                else if((currentPos.x == movePos.x) && _boardState[movePos.x, movePos.y].currentType != PeiceType.AIR)
                    return false;

                else if((Mathf.Abs(movePos.x - currentPos.x) != 0) && (Mathf.Abs(movePos.y - currentPos.y) == 1))
                    return(_boardState[movePos.x, movePos.y].currentType != PeiceType.AIR);

                else return ((Mathf.Abs(movePos.y - currentPos.y) == 1) && (movePos.x == currentPos.x));

            case PeiceType.ROOK:
                return ((currentPos.x == movePos.x || currentPos.y == movePos.y) && CheckForPieces(PeiceType.ROOK, currentPos, movePos));

            case PeiceType.KNIGHT:
                return ((Mathf.Abs(movePos.x - currentPos.x) == 2 && Mathf.Abs(movePos.y - currentPos.y) == 1) || 
                        (Mathf.Abs(movePos.x - currentPos.x) == 1 && Mathf.Abs(movePos.y - currentPos.y) == 2));

            case PeiceType.BISHOB:
                return ((Mathf.Abs(movePos.x - currentPos.x) == Mathf.Abs(movePos.y - currentPos.y)) && CheckForPieces(PeiceType.BISHOB, currentPos, movePos));

            case PeiceType.QUEEN:
                return ((movePos.x == currentPos.x || movePos.y == currentPos.y || 
                        Mathf.Abs(movePos.x - currentPos.x) == Mathf.Abs(movePos.y - currentPos.y))
                        && CheckForPieces(PeiceType.QUEEN, currentPos, movePos));

            case PeiceType.KING:
                return ((Mathf.Abs(movePos.x - currentPos.x) == 1) || (Mathf.Abs(movePos.y - currentPos.y) == 1) || 
                        (Mathf.Abs(movePos.x - currentPos.x) == Mathf.Abs(movePos.y - currentPos.y) && Mathf.Abs(movePos.x - currentPos.x)==1));
            
            default:
                return false;
        }
    }

    private bool CheckForPieces(PeiceType currentPiece, Vector2Int currentPos, Vector2Int movePos)
    {
        switch (currentPiece)
        {
            case PeiceType.ROOK:
                for (int i = 1; i < Mathf.Abs(currentPos.x - movePos.x); i++)
                {
                    if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y].currentType != PeiceType.AIR)
                        return false;
                    else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y].currentType != PeiceType.AIR)
                        return false;
                }
                for (int i = 1; i < Mathf.Abs(currentPos.y - movePos.y); i++)
                {
                    if((currentPos.y < movePos.y) &&_boardState[currentPos.x, currentPos.y + i].currentType != PeiceType.AIR)
                        return false;
                    else if((currentPos.y > movePos.y) &&_boardState[currentPos.x, currentPos.y - i].currentType != PeiceType.AIR)
                        return false;
                }
                return true;

            case PeiceType.BISHOB:
                for (int i = 1; i < Mathf.Abs(currentPos.x - movePos.x); i++)
                {
                    if(currentPos.y < movePos.y)
                    {
                        if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y + i].currentType != PeiceType.AIR)
                            return false;
                        else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y + i].currentType != PeiceType.AIR)
                            return false;
                    }
                    else if(currentPos.y > movePos.y)
                    {
                        if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y - i].currentType != PeiceType.AIR)
                            return false;
                        else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y - i].currentType != PeiceType.AIR)
                            return false;
                    }
                }
                return true;
            
            case PeiceType.QUEEN:
                if (Mathf.Abs(movePos.x - currentPos.x) == Mathf.Abs(movePos.y - currentPos.y))
                {
                    for (int i = 1; i < Mathf.Abs(currentPos.x - movePos.x); i++)
                    {
                        if(currentPos.y < movePos.y)
                        {
                            if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y + i].currentType != PeiceType.AIR)
                               return false;
                            else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y + i].currentType != PeiceType.AIR)
                                return false;
                        }
                        else if(currentPos.y > movePos.y)
                        {
                           if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y - i].currentType != PeiceType.AIR)
                                return false;
                            else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y - i].currentType != PeiceType.AIR)
                             return false;
                        }
                    }
                }
                else if((currentPos.x == movePos.x || currentPos.y == movePos.y))
                {
                    for (int i = 1; i < Mathf.Abs(currentPos.x - movePos.x); i++)
                    {
                        if((currentPos.x < movePos.x) &&_boardState[currentPos.x + i, currentPos.y].currentType != PeiceType.AIR)
                            return false;
                        else if((currentPos.x > movePos.x) &&_boardState[currentPos.x - i, currentPos.y].currentType != PeiceType.AIR)
                            return false;
                    }
                    for (int i = 1; i < Mathf.Abs(currentPos.y - movePos.y); i++)
                    {
                        if((currentPos.y < movePos.y) &&_boardState[currentPos.x, currentPos.y + i].currentType != PeiceType.AIR)
                            return false;
                        else if((currentPos.y > movePos.y) &&_boardState[currentPos.x, currentPos.y - i].currentType != PeiceType.AIR)
                            return false;
                    }
                }
                return true;


            default:
                return false;
        }
    }

    private void PlayerPeices(int indexX, int indexY, Vector3 ppos, GameObject[] prefabs, Transform parent)
    {
        switch(indexX)
            {
            case 0: //Right Side Rooks
                currentType = PeiceType.ROOK;
                mySelf = DesktopInput.Instantiate(prefabs[1], ppos, Quaternion.identity, parent);
                break;
            case 7: // Left Side Rooks
                currentType = PeiceType.ROOK;
                mySelf = DesktopInput.Instantiate(prefabs[1], ppos, Quaternion.identity, parent);
                break;
            case 1:
                currentType = PeiceType.KNIGHT;
                mySelf = DesktopInput.Instantiate(prefabs[2], ppos, Quaternion.identity, parent);
                break;
            case 6:
                currentType = PeiceType.KNIGHT;
                mySelf = DesktopInput.Instantiate(prefabs[2], ppos, Quaternion.identity, parent);
                break;
            case 2:
                currentType = PeiceType.BISHOB;
                mySelf = DesktopInput.Instantiate(prefabs[3], ppos, Quaternion.identity, parent);
                break;
            case 5:
                currentType = PeiceType.BISHOB;
                mySelf = DesktopInput.Instantiate(prefabs[3], ppos, Quaternion.identity, parent);
                break;
            case 3:
                currentType = PeiceType.QUEEN;
                mySelf = DesktopInput.Instantiate(prefabs[4], ppos, Quaternion.identity, parent);
                break;
            case 4:
                currentType = PeiceType.KING;
                mySelf = DesktopInput.Instantiate(prefabs[5], ppos, Quaternion.identity, parent);
                break;

            default:
                break;
        }
        if (!isWhite)
            mySelf.GetComponent<SpriteRenderer>().color = Color.gray;
    }
}