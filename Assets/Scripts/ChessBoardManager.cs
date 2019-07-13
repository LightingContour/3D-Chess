using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chess board manager.
/// 用于管理棋盘相关逻辑
/// </summary>
public class ChessBoardManager : MonoBehaviour
{
    private Transform m_Transform;

    private GameObject m_BlackSideBoard;
    private GameObject m_WhiteSideBoard;

    public GameObject[,] miniBoard = new GameObject[8, 8];

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        // Load Prefabs
        m_BlackSideBoard = Resources.Load<GameObject>("Prefabs/BlackSideBoard");
        m_WhiteSideBoard = Resources.Load<GameObject>("Prefabs/WhiteSideBoard");

        CreatBasicBoard();
    }

    /// <summary>
    /// Creats the basic board.
    /// 生成基础棋盘
    /// </summary>
    void CreatBasicBoard()
    {
        float offset = (float)-3.5;
        float multiple = 10;
        GameObject[,] item = new GameObject[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 pos = new Vector3((j + offset) * multiple, 0, (i + offset) * multiple);
                Vector3 rot = new Vector3(0, 0, 0);
                GameObject board;

                if ((i + j)%2 != 0)
                {
                    board = Instantiate(m_BlackSideBoard, pos, Quaternion.Euler(rot)) as GameObject;
                } else
                {
                    board = Instantiate(m_WhiteSideBoard, pos, Quaternion.Euler(rot)) as GameObject;
                }
                board.GetComponent<Transform>().SetParent(m_Transform);
                miniBoard[i, j] = board;
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
