using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    wait,
    move
}
public enum TileKind
{
    Breakable,
    Blank,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    private FindMatch findMatch;

    public int width;
    public int height;
    public int offSet;

    public GameObject tilePrefab;   
    public GameObject[] dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;
    public TileType[] boardLayout;

    public Dot currentDot;

    private bool[,] blankSpaces;

    private ScoreManager scoreManager;

    public int basePieceValue = 20;
    public int streakValue = 1;
    public int scoreGoals;

    public float refillDelay = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatch = FindObjectOfType<FindMatch>();

        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];

        SetUp();
    }
    public void GenerateBlankSpaces()
    {
        for(int i = 0; i< boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    private void SetUp()
    {
        GenerateBlankSpaces();

        for (int i = 0; i< width; i++)
        {
            for(int j = 0; j< height; j++)
            {
                if(!blankSpaces[i,j])
                {
                    // Create the background tile
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.GetComponent<BackgroundTile>().SetTargetPosition(new Vector2(i, j));

                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";


                    // Create the dots
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);

                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row -2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatch.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatch.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }
    private void CheckToMakeBombs()
    {
        if (findMatch.currentMatches.Count == 4 || findMatch.currentMatches.Count == 7)
        {
            findMatch.CheckBomb();
        }
        if (findMatch.currentMatches.Count == 5 || findMatch.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Make a color bomb
                //Is the current dot matched
                if (currentDot != null && currentDot.isMatched)
                {
                    if (!currentDot.isColorBomb)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeColorBomb();
                    }
                }
                else if (currentDot != null && currentDot.otherDot != null)
                {
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot != null && otherDot.isMatched)
                    {
                        if (!otherDot.isColorBomb)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }

            }
            else
            {
                //Make a adjacent bomb
                //Is the current dot matched
                if (currentDot != null && currentDot.isMatched)
                {
                    if (!currentDot.isAdjacentBomb)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeAdjacentBomb();
                    }
                }
                else if (currentDot != null && currentDot.otherDot != null)
                {
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot != null && otherDot.isMatched)
                    {
                        if (!otherDot.isAdjacentBomb)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeAdjacentBomb();
                        }
                    }
                }
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            // How many elements are in the matched pieces list from findmatches ?
            if (findMatch.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);

            Destroy(allDots[column, row]);

            scoreManager.IncreaseScore(basePieceValue * streakValue);

            allDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatch.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }
    private IEnumerator DecreaseRowCo2()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                // if the current spot isn't blank and is empty
                if (!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    //loop from the space above to the top of the column
                    for(int k = j + 1; k < height; k++)
                    {
                        //if that dot is found
                        if (allDots[i,k] != null)
                        {
                            //move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //Set that dot to be null
                            allDots[i, k] = null;
                            //break out of the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(0.5f);

        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }

                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    allDots[i, j] = piece;

                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }
    private IEnumerator FillBoardCo()
    {
        RefillBoard();

        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnboard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }

        findMatch.currentMatches.Clear();
        currentDot = null;

        if (IsDeadLocked())
        {
            Debug.Log("Deadlocked !!!");
        }

        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }
    private bool MatchesOnboard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Take the second pieces and save it in a holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;

        //Switching the first dot to be second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];

        //Set the first dot to be the second dot
        allDots[column, row] = holder;
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Make Sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        //Check if the dots to the right and two to the right exits
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                    && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //Check if the dots above exits
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                              && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
}
