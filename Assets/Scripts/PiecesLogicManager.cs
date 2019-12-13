using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PiecesLogic Manager
/// 单独处理棋子们的逻辑
/// </summary>
public class PiecesLogicManager : MonoBehaviour
{
    private ChessBoardManager chessBoardManager;
    private ManipulationManager manipulationManager;

    private Transform board_Transform;

    public bool animing; // 是否正在动画
    private Transform move_Chess; // 需要移动的棋子
    private Vector3 target_Pos; // 棋子目标位置

    private readonly string Flag = "PiecesLogicManager-";

    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        manipulationManager = GameObject.Find("BoardPlane").GetComponent<ManipulationManager>();
    }

    /// <summary>
    /// 判断兵是否已经走过
    /// </summary>
    /// <param name="i">选中棋盘的i，第几排</param>
    /// <param name="j">选中棋盘的j，第几列</param>
    /// <param name="chessClass0">棋子的黑白属性，0为白、1为黑</param>
    /// <param name="chessClass1">棋子的类型，0->兵;1->王;2->后;3->车;4->象;5->马；参考ChessClassCheck</param>
    /// <returns></returns>
    public bool PawnMovedCheck(int i, int j, int chessClass0, int chessClass1)
    {
        if (chessClass0 == 0 || chessClass0 == 1)
        {
            if (chessClass1 == 0)
            {
                if (j >= 0 && j<= 7)
                {
                    switch (chessClass0)
                    {
                        case 0:
                            if (i == 1)
                            {
                                return false;
                            }
                            break;
                        case 1:
                            if (i == 6)
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 检测选中的棋盘格中是否存在棋子
    /// 存在的话返回棋子ij
    /// </summary>
    /// <param name="i">选中棋盘的i，第几排</param>
    /// <param name="j">选中棋盘的j，第几列</param>
    /// <param name="chessI">返回棋子i，参考ChessBoardManager的ChessPre</param>
    /// <param name="chessJ">返回棋子j，参考ChessBoardManager的ChessPre</param>
    /// <returns>有棋子返回true，无返回false</returns>
    public bool ChessExistCheck(int i, int j, out int chessI, out int chessJ)
    {
        chessI = 8;
        chessJ = 8;
        float EPSILON = 0.2f;
        board_Transform = chessBoardManager.miniBoard[i, j].GetComponent<Transform>();
        for (int n = 0; n < 2; n++)
        {
            for (int m = 0; m < 16; m++)
            {
                Vector3 position = chessBoardManager.ChessPre[n, m].GetComponent<Transform>().position;
                if (System.Math.Abs(position.x - board_Transform.position.x) < EPSILON && System.Math.Abs(position.z - board_Transform.position.z) < EPSILON && chessBoardManager.ChessPre[n, m].activeInHierarchy == true)
                {
                    chessI = n;
                    chessJ = m;
                    break;
                }
            }
        }
        bool flag = (chessI == 8 && chessJ == 8) ? false : true;
        return flag;
    }

    /// <summary>
    /// 检测选中的棋盘格中是否存在棋子
    /// </summary>
    /// <param name="i">选中棋盘的i，参考ChessBoardManager的ChessPre</param>
    /// <param name="j">选中棋盘的j，参考ChessBoardManager的ChessPre</param>
    /// <returns>有棋子返回true，无返回false</returns>
    public bool ChessExistCheck(int i, int j)
    {
        int chessI = 8;
        int chessJ = 8;
        float EPSILON = 0.2f;
        board_Transform = chessBoardManager.miniBoard[i, j].GetComponent<Transform>();
        for (int n = 0; n < 2; n++)
        {
            for (int m = 0; m < 16; m++)
            {
                Vector3 position = chessBoardManager.ChessPre[n, m].GetComponent<Transform>().position;
                if (System.Math.Abs(position.x - board_Transform.position.x) < EPSILON && System.Math.Abs(position.z - board_Transform.position.z) < EPSILON && chessBoardManager.ChessPre[n, m].activeInHierarchy == true)
                {
                    chessI = n;
                    chessJ = m;
                    break;
                }
            }
        }
        bool flag = (chessI == 8 && chessJ == 8) ? false : true;
        return flag;
    }

    /// <summary>
    /// 判断棋子是否可以移动
    /// </summary>
    /// <param name="i">chessI，参考ChessBoardManager的ChessPre</param>
    /// <param name="j">chessJ，参考ChessBoardManager的ChessPre</param>
    /// <param name="chessClass0">棋子的黑白属性，0为白、1为黑</param>
    /// <param name="eat">可走步是否是吃棋子</param>
    /// <returns></returns>
    public bool ChessCouldMoveCheck(int i, int j, int chessClass0, out bool eat)
    {
        int chessI = 0;
        int chessJ = 0;
        eat = false;
        bool flag = false;
        if (ChessExistCheck(i, j, out chessI, out chessJ))
        {
            int[] chessClass = ChessClassCheck(chessI, chessJ);
            if (chessClass0 != chessClass[0])
            {
                eat = true;
                flag = true;
            }
        }
        else {
            flag = true;
        }
        return flag;
    }

    /// <summary>
    /// 判断棋子是否可以移动
    /// </summary>
    /// <param name="i">chessI，参考ChessBoardManager的ChessPre</param>
    /// <param name="j">chessJ，参考ChessBoardManager的ChessPre</param>
    /// <param name="chessClass0">棋子的黑白属性，0为白、1为黑</param>
    /// <returns></returns>
    public bool ChessCouldMoveCheck(int i, int j, int chessClass0)
    {
        int chessI = 0;
        int chessJ = 0;
        bool flag = false;
        if (ChessExistCheck(i, j, out chessI, out chessJ))
        {
            int[] chessClass = ChessClassCheck(chessI, chessJ);
            if (chessClass0 != chessClass[0])
            {
                flag = true;
            }
        }
        else
        {
            flag = true;
        }
        return flag;
    }
    
    /// <summary>
    /// 判断是否增加棋子可移动区域
    /// </summary>
    /// <param name="i">chessI，参考ChessBoardManager的ChessPre</param>
    /// <param name="j">chessJ，参考ChessBoardManager的ChessPre</param>
    /// <param name="chessClass0">棋子的黑白属性，0为白、1为黑</param>
    /// <param name="couldStep">返回棋子之后可走的位置、是否为吃子</param>
    /// <returns></returns>
    public bool ChessCouldMoveAdd(int i, int j, int chessClass0, out int[] couldStep)
    {
        bool flag = false;
        couldStep = null;
        if (ChessCouldMoveCheck(i, j, chessClass0, out bool eat))
        {
            if (eat == false)
            {
                couldStep = new int[3] { i, j, 0 };
                flag = true;
            }
            else {
                couldStep = new int[3] { i, j, 1 };
                flag = true;
            }
        }
        return flag;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="i">chessI，参考ChessBoardManager的ChessPre</param>
    /// <param name="j">chessJ，参考ChessBoardManager的ChessPre</param>
    /// <param name="chessClass0">棋子的黑白属性，0为白、1为黑</param>
    /// <param name="isPawnMove">false为棋子普通移动；true为棋子斜吃子</param>
    /// <param name="couldStep">返回棋子之后可走的位置、是否为吃子</param>
    /// <returns></returns>
    public bool ChessCouldMoveAdd(int i, int j, int chessClass0, bool isPawnMove,out int[] couldStep)
    {
        bool flag = false;
        couldStep = null;
        if (ChessCouldMoveCheck(i, j, chessClass0, out bool eat))
        {
            if (isPawnMove && !eat)
            {
                couldStep = new int[3] { i, j, 0 };
                flag = true;
            }
            else if (!isPawnMove && eat)
            {
                couldStep = new int[3] { i, j, 1 };
                flag = true;
            }
        }
        return flag;
    }

    /// <summary>
    /// 移动棋子
    /// </summary>
    /// <param name="i">Board的i</param>
    /// <param name="j">Board的j</param>
    /// <param name="k">前往的Board的i</param>
    /// <param name="l">前往的Board的j</param>
    /// <param name="kill">是否吃子</param>
    public void ChessMove(int i, int j, int k, int l, bool kill)
    {
        if (kill)
        {
            ChessExistCheck(k, l, out int chessK, out int chessL);
            chessBoardManager.ChessPre[chessK, chessL].SetActive(false);
        }
        ChessExistCheck(i, j, out int chessI, out int chessJ);
        move_Chess = chessBoardManager.ChessPre[chessI, chessJ].GetComponent<Transform>();
        target_Pos = chessBoardManager.GetChessVector(chessBoardManager.miniBoard[k, l]);
//        chessBoardManager.ChessPre[chessI, chessJ].GetComponent<Transform>().position = chessBoardManager.GetChessVector(chessBoardManager.miniBoard[k, l]);
    }

    /// <summary>
    /// 判断棋子是哪个类型
    /// </summary>
    /// <param name="i">chessI，棋子初始化时的数组首位值，参考</param>
    /// <param name="j">chessJ</param>
    /// <returns>返回首位为0->白;1->黑
    /// 返回末位0->兵;1->王;2->后;3->车;4->象;5->马</returns>
    public int[] ChessClassCheck(int i, int j)
    {
        int[] type = new int[2];
        if (i == 0 || i == 1)
        {
            type[0] = i;
        }
        else {
            type[0] = 2;// 传参错误，i反2
        }
        if (j > 7 && j < 16)
        {
            type[1] = 0;
        }
        else if (j == 4)
        {
            type[1] = 1;
        }
        else if (j == 3)
        {
            type[1] = 2;
        }
        else if (j == 0 || j == 7)
        {
            type[1] = 3;
        }
        else if (j == 1 || j == 6)
        {
            type[1] = 5;
        }
        else if (j == 2 || j == 5)
        {
            type[1] = 4;
        }
        else {
            type[1] = 6;// 传参错误，j反6
        }
        return type;
    }

    /// <summary>
    /// 通过获取棋子种类和棋子位置
    /// 返回下一步可去的棋盘位置
    /// </summary>
    /// <param name="chessClass">棋子种类，参考ChessClassCheck</param>
    /// <param name="chessPos">棋子位置，参考ChessBoardManager的miniBoard</param>
    /// <param name="nextCouldStep">返回棋子之后可走的位置、是否为吃子</param>
public void NextStepGuider(int[] chessClass, int[] chessPos, out List<int[]> nextCouldStep)
    {
        nextCouldStep = new List<int[]>();
        Debug.Log(Flag + "chessClass[1]:" + chessClass[1]);
        if (chessClass[1] == 0) // 兵
        {
            switch (chessClass[0])
            {
                case 0:
                    // 兵正常可前进移动
                    if (chessPos[0] != 7)
                        if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1], chessClass[0], true, out var couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            // 兵未移动时，能前进两格
                            if (!PawnMovedCheck(chessPos[0], chessPos[1], chessClass[0], chessClass[1]))
                                if (ChessCouldMoveAdd(chessPos[0] + 2, chessPos[1], chessClass[0], true, out var couldStep1))
                                    nextCouldStep.Add(couldStep1);
                        }

                    // 兵的斜吃子
                    if (chessPos[0] < 7)
                    {
                        if (chessPos[1] > 0 && ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] - 1, chessClass[0], false,
                                out var couldStep1))
                            nextCouldStep.Add(couldStep1);
                        if (chessPos[1] < 7 && ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] + 1, chessClass[0], false,
                                out var couldStep2))
                            nextCouldStep.Add(couldStep2);
                    }
                    break;
                case 1:
                    // 兵正常可前进移动
                    if (chessPos[0] != 0)
                        if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1], chessClass[0], true, out var couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            // 兵未移动时，能前进两格
                            if (!PawnMovedCheck(chessPos[0], chessPos[1], chessClass[0], chessClass[1]))
                                if (ChessCouldMoveAdd(chessPos[0] - 2, chessPos[1], chessClass[0], true, out var couldStep1))
                                    nextCouldStep.Add(couldStep1);
                        }

                    // 兵的斜吃子
                    if (chessPos[0] > 0)
                    {
                        if (chessPos[1] > 0 && ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] - 1, chessClass[0], false,
                                out int[] couldStep1))
                            nextCouldStep.Add(couldStep1);
                        if (chessPos[1] < 7 && ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] + 1, chessClass[0], false,
                                out int[] couldStep2))
                            nextCouldStep.Add(couldStep2);
                    }
                    break;
            }
        }
        else if (chessClass[1] == 1) // 王,暂不用判断哪一边
        {
            if (chessPos[0] - 1 >= 0) // 下
            {
                if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1], chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[0] + 1 <= 7) // 上
            {
                if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1], chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[1] - 1 >= 0) // 左
            {
                if (ChessCouldMoveAdd(chessPos[0], chessPos[1] - 1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[1] + 1 <= 7) // 右
            {
                if (ChessCouldMoveAdd(chessPos[0], chessPos[1] + 1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[0] - 1 >= 0 && chessPos[1] - 1 >= 0) //左下
            {
                if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] -1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[0] - 1 >= 0 && chessPos[1] + 1 <= 7) //右下
            {
                if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] + 1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[0] + 1 <= 7 && chessPos[1] - 1 >= 0) //左上
            {
                if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] - 1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            if (chessPos[0] + 1 <= 7 && chessPos[1] + 1 <= 7) //右上
            {
                if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] + 1, chessClass[0], out int[] couldStep))
                {
                    nextCouldStep.Add(couldStep);
                }
            }
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 2) // 后,暂不用确定属于哪一边
        {
            if (chessPos[0] != 7)
            {
                if (chessPos[1] != 0) // 左上
                {
                    for (int i = 1; chessPos[0] + i <= 7 && chessPos[1] - i >= 0; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1] - i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
                if (chessPos[1] != 7) // 右上
                {
                    for (int i = 1; chessPos[0] + i <= 7 && chessPos[1] + i <= 7; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1] + i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (chessPos[0] != 0)
            {
                if (chessPos[1] != 0) // 左下
                {
                    for (int i = 1; chessPos[0] - i >= 0 && chessPos[1] - i >= 0; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1] - i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (chessPos[1] != 7) // 右下
                {
                    for (int i = 1; chessPos[0] - i >= 0 && chessPos[1] + i <= 7; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1] + i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (chessPos[1] != 0) // 左
            {
                for (int i = 1; chessPos[1] - i >= 0; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0], chessPos[1] - i, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else {
                        break;
                    }
                }
            }
            if (chessPos[1] != 7) // 右
            {
                for (int i = 1; chessPos[1] + i <= 7; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0], chessPos[1] + i, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (chessPos[0] != 0) // 下
            {
                for (int i = 1; chessPos[0] - i >= 0; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1], chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (chessPos[0] != 7) // 上
            {
                for (int i = 1; chessPos[0] + i <= 7; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1], chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 3) // 车
        {
            if (chessPos[1] != 0) // 左
            {
                for (int i = 1; chessPos[1] - i >= 0; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0], chessPos[1] - i, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (chessPos[1] != 7) // 右
            {
                for (int i = 1; chessPos[1] + i <= 7; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0], chessPos[1] + i, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (chessPos[0] != 0) // 下
            {
                for (int i = 1; chessPos[0] - i >= 0; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1], chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (chessPos[0] != 7) // 上
            {
                for (int i = 1; chessPos[0] + i <= 7; i++)
                {
                    if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1], chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                        if (couldStep[2] == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 4) // 象
        {
            if (chessPos[0] != 7)
            {
                if (chessPos[1] != 0) // 左上
                {
                    for (int i = 1; chessPos[0] + i <= 7 && chessPos[1] - i >= 0; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1] - i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (chessPos[1] != 7) // 右上
                {
                    for (int i = 1; chessPos[0] + i <= 7 && chessPos[1] + i <= 7; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + i, chessPos[1] + i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (chessPos[0] != 0)
            {
                if (chessPos[1] != 0) // 左下
                {
                    for (int i = 1; chessPos[0] - i >= 0 && chessPos[1] - i >= 0; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1] - i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (chessPos[1] != 7) // 右下
                {
                    for (int i = 1; chessPos[0] - i >= 0 && chessPos[1] + i <= 7; i++)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - i, chessPos[1] + i, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                            if (couldStep[2] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 5) // 马
        {
            if (chessPos[0] - 1 >= 0)
            {
                if (chessPos[1] - 2 >= 0)
                {
                    if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] - 2, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                    }
                }
                if (chessPos[1] + 2 <= 7)
                {
                    if (ChessCouldMoveAdd(chessPos[0] - 1, chessPos[1] + 2, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                    }
                }
                if (chessPos[0] - 2 >= 0)
                {
                    if (chessPos[1] - 1 >= 0)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - 2, chessPos[1] - 1, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                        }
                    }
                    if (chessPos[1] + 1 <= 7)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] - 2, chessPos[1] + 1, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                        }
                    }
                }
            }
            if (chessPos[0] + 1 <= 7)
            {
                if (chessPos[1] - 2 >= 0)
                {
                    if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] - 2, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                    }
                }
                if (chessPos[1] + 2 <= 7)
                {
                    if (ChessCouldMoveAdd(chessPos[0] + 1, chessPos[1] + 2, chessClass[0], out int[] couldStep))
                    {
                        nextCouldStep.Add(couldStep);
                    }
                }
                if (chessPos[0] + 2 <= 7)
                {
                    if (chessPos[1] - 1 >= 0)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + 2, chessPos[1] - 1, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                        }
                    }
                    if (chessPos[1] + 1 <= 7)
                    {
                        if (ChessCouldMoveAdd(chessPos[0] + 2, chessPos[1] + 1, chessClass[0], out int[] couldStep))
                        {
                            nextCouldStep.Add(couldStep);
                        }
                    }
                }
            }
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        Debug.Log(Flag + "nextCouldTimes:" + nextCouldStep.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (move_Chess != null && move_Chess.position != target_Pos)
        {
            animing = true;
            if (Mathf.Abs(Vector3.Distance(move_Chess.position, target_Pos)) <= 0.2f)
            {
                move_Chess.position = target_Pos;
            }
            move_Chess.position = Vector3.Lerp(move_Chess.position, target_Pos, Time.deltaTime * 4);
        }
        else
        {
            animing = false;
        }
    }
}
