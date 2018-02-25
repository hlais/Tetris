using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Transform m_emptySprite;
    public int m_height = 30;
    public int m_width = 10;

    //the death section of the game.
    public int m_header = 8;

    Transform[,] m_grid;

    public int m_completedRows = 0;

    public ParticelPlayer[] m_rowGlowFx = new ParticelPlayer[4];


    // pre start. Gets run befor start 
    private void Awake()
    {
        m_grid = new Transform[m_width, m_height];
    }


    void Start()
    {
        DrawEmptyCells();
    }


    //conditions to check 1) Is square within boundaries. Applies to One square block
    bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0);
    }

    bool IsOccupied(int x, int y, Shape shape)
    {
        //check 2d array is true is not null. && and a parent from a diffent shaped object
        return (m_grid[x, y] != null && m_grid[x, y].parent != shape.transform);
    }

    //this is to determine when the shape has hit the button of the board
    //2- is square occupited by another shape. Will be called from GameController. So public

    public bool IsValidPosition(Shape shape)
    {
        //if you use FOREACH on transform, It will go through every child of transform
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            if (!IsWithinBoard((int)pos.x, (int)pos.y))
            {
                return false;
            }
            //if on board and not on a space thats taken
            if (IsOccupied((int)pos.x, (int)pos.y, shape))
            {
                return false;
            }

        }
        //this is a valid space for the shape
        return true;
    }

    void DrawEmptyCells()
    {
        if (m_emptySprite != null)
        {
            for (int y = 0; y < m_height - m_header; y++)
            {
                for (int x = 0; x < m_width; x++)
                {

                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                    //next we will name every co ordinate on the grid
                    clone.name = "Board Space (X = " + x.ToString() + " and Y = " + y.ToString() + " )";
                    //parent positions to clone 
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("Warning! Please assign the emptySprite object!");
        }
    }
    //store shape in boards grid Array
    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
        {
            return;
        }
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            //store child in grid. cast float to int. since grid is world space.float
            m_grid[(int)pos.x, (int)pos.y] = child;
        }
    }

    //---to check if row in complete---
    bool IsComplete(int y)
    {
        for (int x = 0; x < m_width; ++x)
        {
            //if any grid cell is empty
            if (m_grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }
    //---to clear row---
    void ClearRow(int y)
    {   
        for (int x = 0; x < m_width; ++x)
        {
            if (m_grid[x, y] != null)
            {
                Destroy(m_grid[x, y].gameObject);
            }
            //then grid cell is set to null
            m_grid[x, y] = null;
        }
    }
    //---now to shift all the top rows above down--- 

    void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < m_width; ++x)
        {
            if (m_grid[x,y] !=null)
            {
                //shifting row above down
                m_grid[x, y - 1] = m_grid[x, y];
                m_grid[x, y] = null;
                m_grid[x, y - 1].position += new Vector3(0,-1,0);
        
            }
        }
    }
    void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < m_height; ++i)
        {
            ShiftOneRowDown(i);
        }
    }
    //---now putting together the above small functions---
    public IEnumerator ClearAllRows()
    {
        m_completedRows = 0;

        for (int y = 0; y < m_height; ++y)
        {
            if (IsComplete(y))
            {
                ClearRoxFX(m_completedRows, y);
                m_completedRows++;
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int y = 0; y < m_height; ++y)
        {
            if (IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y + 1);
                //test row again to make sure if the row is complete
                yield return new WaitForSeconds(0.3f);
                y--;
            }
        }

    }

    //to check if player is over the limit of lines
    //this method is invoked before we landshape
    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= (m_height - m_header - 1)) //-1 since values start at 0
            {
                //is over limit
                return true;
            }

        }
        return false;
    }
    void ClearRoxFX(int idx, int y)
    {
        if (m_rowGlowFx[idx])
        {
            m_rowGlowFx[idx].transform.position = new Vector3(0, y, -2f);
            m_rowGlowFx[idx].Play();
        }
    }
}
