using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{

    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = 0.5f;
    public bool m_canRelease = false;

    public void Catch(Shape shape)
    {
        if (m_heldShape)
        {
            Debug.LogWarning("Holder Warning! Release a shape before trying to hold!");
            return;
        }
        if (!shape)
        {
            Debug.LogWarning("Holder Warning! Invalid shape!");
        }
        if (m_holderXform)
        {
            shape.transform.position = m_holderXform.position + shape.m_queueOffSet;
            shape.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            m_heldShape = shape;
        }
        else
        {
            Debug.LogWarning("Holder Warning! Holder has no transform assigned!");
        }
    }
    public Shape Release()
    {
        m_heldShape.transform.localScale = Vector3.one;
        //store shape in temp var
        Shape shape = m_heldShape;
        m_heldShape = null;

        m_canRelease = false;

        //return shape
        return shape;
    }
}
