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

    private GameObject m_IconWhiteRook;
    private GameObject m_IconWhiteHorse;
    private GameObject m_IconWhiteBishop;
    private GameObject m_IconWhiteQueen;
    private GameObject m_IconWhiteKing;
    private GameObject m_IconWhitePawn;

    private GameObject m_IconBlackRook;
    private GameObject m_IconBlackHorse;
    private GameObject m_IconBlackBishop;
    private GameObject m_IconBlackQueen;
    private GameObject m_IconBlackKing;
    private GameObject m_IconBlackPawn;

    private GameObject m_ChessWhiteRook;
    private GameObject m_ChessWhiteHorse;
    private GameObject m_ChessWhiteBishop;
    private GameObject m_ChessWhiteQueen;
    private GameObject m_ChessWhiteKing;
    private GameObject m_ChessWhitePawn;

    private GameObject m_ChessBlackRook;
    private GameObject m_ChessBlackHorse;
    private GameObject m_ChessBlackBishop;
    private GameObject m_ChessBlackQueen;
    private GameObject m_ChessBlackKing;
    private GameObject m_ChessBlackPawn;

    public GameObject[,] miniBoard = new GameObject[8, 8];
    public Transform m_ChessTransform;
    private GameObject[,] ChessIcon = new GameObject[2, 16];
    public GameObject[,] ChessPre = new GameObject[2, 16];

    private Vector3 prfOffset = new Vector3(0, -0.5f, 0);
    private Vector3 posOffset = new Vector3(0, 1.5f, 0);

    // Start is called before the first frame update3
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_ChessTransform = GameObject.Find("Chesses").GetComponent<Transform>();

        // Load ChessBoard Prefabs
        m_BlackSideBoard = Resources.Load<GameObject>("Prefabs/MiniBoard/BlackSideBoard_TinyChess");
        m_WhiteSideBoard = Resources.Load<GameObject>("Prefabs/MiniBoard/WhiteSideBoard_TinyChess");

        // Load Chess Icons
        m_IconWhiteRook = Resources.Load<GameObject>("Prefabs/Icons/White Rook Icon");
        m_IconWhiteHorse = Resources.Load<GameObject>("Prefabs/Icons/White Horse Icon");
        m_IconWhiteBishop = Resources.Load<GameObject>("Prefabs/Icons/White Bishop Icon");
        m_IconWhiteQueen = Resources.Load<GameObject>("Prefabs/Icons/White Queen Icon");
        m_IconWhiteKing = Resources.Load<GameObject>("Prefabs/Icons/White King Icon");
        m_IconWhitePawn = Resources.Load<GameObject>("Prefabs/Icons/White Pawn Icon");

        m_IconBlackRook = Resources.Load<GameObject>("Prefabs/Icons/Black Rook Icon");
        m_IconBlackHorse = Resources.Load<GameObject>("Prefabs/Icons/Black Horse Icon");
        m_IconBlackBishop = Resources.Load<GameObject>("Prefabs/Icons/Black Bishop Icon");
        m_IconBlackQueen = Resources.Load<GameObject>("Prefabs/Icons/Black Queen Icon");
        m_IconBlackKing = Resources.Load<GameObject>("Prefabs/Icons/Black King Icon");
        m_IconBlackPawn = Resources.Load<GameObject>("Prefabs/Icons/Black Pawn Icon");

        // Load Chess Prefabs
        m_ChessWhiteRook = Resources.Load<GameObject>("Prefabs/Chess/White Stone Rook");
        m_ChessWhiteHorse = Resources.Load<GameObject>("Prefabs/Chess/White Stone Horse");
        m_ChessWhiteBishop = Resources.Load<GameObject>("Prefabs/Chess/White Stone Bishop");
        m_ChessWhiteQueen = Resources.Load<GameObject>("Prefabs/Chess/White Stone Queen");
        m_ChessWhiteKing = Resources.Load<GameObject>("Prefabs/Chess/White Stone King");
        m_ChessWhitePawn = Resources.Load<GameObject>("Prefabs/Chess/White Stone Pawn");

        m_ChessBlackRook = Resources.Load<GameObject>("Prefabs/Chess/Black Stone Rook");
        m_ChessBlackHorse  = Resources.Load<GameObject>("Prefabs/Chess/Black Stone Horse");
        m_ChessBlackBishop = Resources.Load<GameObject>("Prefabs/Chess/Black Stone Bishop");
        m_ChessBlackQueen = Resources.Load<GameObject>("Prefabs/Chess/Black Stone Queen");
        m_ChessBlackKing = Resources.Load<GameObject>("Prefabs/Chess/Black Stone King");
        m_ChessBlackPawn = Resources.Load<GameObject>("Prefabs/Chess/Black Stone Pawn");

        CreatBasicBoard();
        InitChess();
    }

    /// <summary>
    /// Creats the basic board.
    /// 生成基础棋盘
    /// </summary>
    void CreatBasicBoard()
    {
        float offset = (float)-3.5;
        float multiple = 10;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 pos = new Vector3((j + offset) * multiple, 0, (i + offset) * multiple);
                Vector3 rot = new Vector3(0, 0, 0);
                GameObject board;

                if ((i + j)%2 == 0)
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

    /// <summary>
    /// Initiate Chesses
    /// 首次进入棋盘生成棋子
    /// </summary>
    void InitChess()
    {
        Vector3 rot = new Vector3(0, 180, 0);
        ChessIcon[0, 0] = Instantiate(m_IconWhiteRook, miniBoard[0, 0].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 1] = Instantiate(m_IconWhiteHorse, miniBoard[0, 1].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 2] = Instantiate(m_IconWhiteBishop, miniBoard[0, 2].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 3] = Instantiate(m_IconWhiteQueen, miniBoard[0, 3].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 4] = Instantiate(m_IconWhiteKing, miniBoard[0, 4].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 5] = Instantiate(m_IconWhiteBishop, miniBoard[0, 5].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 6] = Instantiate(m_IconWhiteHorse, miniBoard[0, 6].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 7] = Instantiate(m_IconWhiteRook, miniBoard[0, 7].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 8] = Instantiate(m_IconWhitePawn, miniBoard[1, 0].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 9] = Instantiate(m_IconWhitePawn, miniBoard[1, 1].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 10] = Instantiate(m_IconWhitePawn, miniBoard[1, 2].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 11] = Instantiate(m_IconWhitePawn, miniBoard[1, 3].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 12] = Instantiate(m_IconWhitePawn, miniBoard[1, 4].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 13] = Instantiate(m_IconWhitePawn, miniBoard[1, 5].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 14] = Instantiate(m_IconWhitePawn, miniBoard[1, 6].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[0, 15] = Instantiate(m_IconWhitePawn, miniBoard[1, 7].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;

        ChessIcon[1, 0] = Instantiate(m_IconBlackRook, miniBoard[7, 0].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 1] = Instantiate(m_IconBlackHorse, miniBoard[7, 1].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 2] = Instantiate(m_IconBlackBishop, miniBoard[7, 2].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 3] = Instantiate(m_IconBlackQueen, miniBoard[7, 3].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 4] = Instantiate(m_IconBlackKing, miniBoard[7, 4].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 5] = Instantiate(m_IconBlackBishop, miniBoard[7, 5].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 6] = Instantiate(m_IconBlackHorse, miniBoard[7, 6].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 7] = Instantiate(m_IconBlackRook, miniBoard[7, 7].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 8] = Instantiate(m_IconBlackPawn, miniBoard[6, 0].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 9] = Instantiate(m_IconBlackPawn, miniBoard[6, 1].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 10] = Instantiate(m_IconBlackPawn, miniBoard[6, 2].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 11] = Instantiate(m_IconBlackPawn, miniBoard[6, 3].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 12] = Instantiate(m_IconBlackPawn, miniBoard[6, 4].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 13] = Instantiate(m_IconBlackPawn, miniBoard[6, 5].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 14] = Instantiate(m_IconBlackPawn, miniBoard[6, 6].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;
        ChessIcon[1, 15] = Instantiate(m_IconBlackPawn, miniBoard[6, 7].GetComponent<Transform>().position + posOffset, Quaternion.Euler(rot)) as GameObject;

        ChessPre[0, 0] = Instantiate(m_ChessWhiteRook, ChessIcon[0, 0].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 1] = Instantiate(m_ChessWhiteHorse, ChessIcon[0, 1].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 2] = Instantiate(m_ChessWhiteBishop, ChessIcon[0, 2].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 3] = Instantiate(m_ChessWhiteQueen, ChessIcon[0, 3].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 4] = Instantiate(m_ChessWhiteKing, ChessIcon[0, 4].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 5] = Instantiate(m_ChessWhiteBishop, ChessIcon[0, 5].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 6] = Instantiate(m_ChessWhiteHorse, ChessIcon[0, 6].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 7] = Instantiate(m_ChessWhiteRook, ChessIcon[0, 7].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 8] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 8].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 9] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 9].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 10] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 10].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 11] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 11].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 12] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 12].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 13] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 13].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 14] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 14].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[0, 15] = Instantiate(m_ChessWhitePawn, ChessIcon[0, 15].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;

        ChessPre[1, 0] = Instantiate(m_ChessBlackRook, ChessIcon[1, 0].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 1] = Instantiate(m_ChessBlackHorse, ChessIcon[1, 1].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 2] = Instantiate(m_ChessBlackBishop, ChessIcon[1, 2].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 3] = Instantiate(m_ChessBlackQueen, ChessIcon[1, 3].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 4] = Instantiate(m_ChessBlackKing, ChessIcon[1, 4].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 5] = Instantiate(m_ChessBlackBishop, ChessIcon[1, 5].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 6] = Instantiate(m_ChessBlackHorse, ChessIcon[1, 6].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 7] = Instantiate(m_ChessBlackRook, ChessIcon[1, 7].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 8] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 8].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 9] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 9].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 10] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 10].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 11] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 11].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 12] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 12].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 13] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 13].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 14] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 14].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;
        ChessPre[1, 15] = Instantiate(m_ChessBlackPawn, ChessIcon[1, 15].GetComponent<Transform>().position + prfOffset, Quaternion.Euler(rot)) as GameObject;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                ChessPre[i, j].GetComponent<Transform>().SetParent(m_ChessTransform);
                ChessIcon[i, j].GetComponent<Transform>().SetParent(ChessPre[i, j].GetComponent<Transform>());
            }
        }
    }

    /// <summary>
    /// 返回当前Board的位置
    /// </summary>
    /// <param name="chessBoard">需要查询的Board</param>
    /// <returns>返回i,j值</returns>
    public int[] GetBoardPos(GameObject chessBoard)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (chessBoard == miniBoard[i, j])
                {
                    return new int[2] { i, j };
                }
            }
        }
        // 未检索到，返回异常值8,8
        return new int[2] { 8, 8 };
    }

    /// <summary>
    /// 获取Board，并重置其颜色
    /// </summary>
    /// <param name="chessBoard"></param>
    public void ResetBoard(GameObject chessBoard)
    {
        int[] pos = GetBoardPos(chessBoard);
        if ((pos[0] + pos[1]) % 2 == 0)
        {
            chessBoard.GetComponent<Renderer>().material = m_BlackSideBoard.GetComponent<MeshRenderer>().sharedMaterial;
        }
        else
        {
            chessBoard.GetComponent<Renderer>().material = m_WhiteSideBoard.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }

    /// <summary>
    /// 通过Board，返回需要转换到该位置的棋子的位置
    /// </summary>
    /// <param name="chessBoard"></param>
    /// <returns>需要的棋子position</returns>
    public Vector3 GetChessVector(GameObject chessBoard)
    {
        return chessBoard.GetComponent<Transform>().position + prfOffset + posOffset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
