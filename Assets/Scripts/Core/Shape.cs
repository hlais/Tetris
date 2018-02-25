using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {

     GameObject[] m_glowSquareFx;
    public string glowSquareTag = "LandShapeFx";
    public bool m_canRotate = true;



    public Vector3 m_queueOffSet;


    void Move (Vector3 moveDirection)
    {
        transform.position += moveDirection;

    }
    // move functions
    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }
    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }
    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }
    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }
    //Rotate Functions
    public void RotateLeft()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }
    public void RotateRight()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
    public void RotateClockwise(bool clockwise)
    {
        if (clockwise)
        {
            RotateRight();
        }
        else
        {
            RotateLeft();
        }
    }
    public void LandShapeFX()
    {
        int i = 0;

        foreach (Transform child in gameObject.transform)
        {
            if (m_glowSquareFx[i])
            {
                m_glowSquareFx[i].transform.position = new Vector3(child.position.x, child.position.y, -2f);
                ParticelPlayer particlePlayer = m_glowSquareFx[i].GetComponent<ParticelPlayer>();

                if (particlePlayer)
                {
                    particlePlayer.Play();
                }
            }
            i++;
        }
    }




    void Start () {

        if (glowSquareTag != "")
        {
            m_glowSquareFx = GameObject.FindGameObjectsWithTag(glowSquareTag);
        }
        Vector2 oringalVector = new Vector2(4.3f, 1.3f);
        Vector2 newVector = VectorF.Round(oringalVector);
        Debug.Log(newVector.ToString());
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
