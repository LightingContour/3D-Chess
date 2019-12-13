using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float rotSpeed = 170;
    private bool setFront = true;
    private bool changing = false;
    private ChessBoardManager chessBoardManager;

    private Transform m_rotParent;
    
    private Transform m_transform;
    private Vector3[] m_position = new Vector3[2];
    private Vector3[] m_rotation = new Vector3[2];
    private Transform light_transform;
    private Vector3[] light_rotation = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        m_transform = gameObject.GetComponent<Transform>();
        
        m_rotParent = GameObject.Find("CameraRot").GetComponent<Transform>();
        m_transform.parent = m_rotParent;
        
        m_position[0] = new Vector3(0, 145, -120);
        m_position[1] = new Vector3(0,145,120);
        m_rotation[0] = new Vector3(52, 0,0);
        m_rotation[1] = new Vector3(52, 180, 0);
        light_transform = GameObject.Find("Directional Light").transform;
        light_rotation[0] = new Vector3(50,0,0);
        light_rotation[1] = new Vector3(130,0,0);
    }

    private void Update()
    {
//        m_rotParent.Rotate(0, rotSpeed * Time.deltaTime, 0);
        DoSwitchPos();
    }
    
    public void SwitchPos(bool isBlack)
    {
        chessBoardManager.SwitchIcon(isBlack);
//        if (!isBlack)
//        {
//            m_transform.position = m_position[0];
//            m_transform.rotation = Quaternion.Euler(m_rotation[0]);
//            light_transform.rotation = Quaternion.Euler(light_rotation[0]);
//        }
//        else
//        {
//            m_transform.position = m_position[1];
//            m_transform.rotation = Quaternion.Euler(m_rotation[1]);
//            light_transform.rotation = Quaternion.Euler(light_rotation[1]);
//        }
        setFront = !isBlack;
        Debug.Log("SetFront:" + setFront);
        changing = true;
    }

    private void DoSwitchPos()
    {
        if (changing)
        {
            m_rotParent.Rotate(0, rotSpeed * Time.deltaTime, 0);
            if (setFront)
            {
                light_transform.rotation = Quaternion.Euler(light_rotation[0]);
                if (m_rotParent.rotation.eulerAngles.y < 180)
                {
                    changing = false;
                    m_rotParent.rotation = Quaternion.Euler(new Vector3(0,0,0));
                }
            }
            else
            {
                light_transform.rotation = Quaternion.Euler(light_rotation[1]);
                if (m_rotParent.rotation.eulerAngles.y > 180)
                {
                    changing = false;
                    m_rotParent.rotation = Quaternion.Euler(new Vector3(0,180,0));
                }
            }
        }
    }
}
