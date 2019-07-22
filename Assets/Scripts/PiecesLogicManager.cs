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

    private readonly string Flag = "PiecesLogicManager-";

    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        manipulationManager = GameObject.Find("BoardPlane").GetComponent<ManipulationManager>();
    }

    /// <summary>
    /// 检测选中的棋盘格中是否存在棋子
    /// 存在的话返回棋子ij
    /// </summary>
    /// <param name="i">选中棋盘的i</param>
    /// <param name="j">选中棋盘的j</param>
    /// <param name="chessI">返回棋子i</param>
    /// <param name="chessJ">返回棋子j</param>
    /// <returns>有棋子返回true，无返回false</returns>
    public bool ChessExistCheck(int i, int j, out int chessI, out int chessJ)
    {
        chessI = 0;
        chessJ = 0;
        float EPSILON = 0.2f;
        board_Transform = chessBoardManager.miniBoard[i, j].GetComponent<Transform>();
        for (int n = 0; n < 2; n++)
        {
            for (int m = 0; m < 16; m++)
            {
                Vector3 position = chessBoardManager.ChessPre[n, m].GetComponent<Transform>().position;
                if (System.Math.Abs(position.x - board_Transform.position.x) < EPSILON && System.Math.Abs(position.z - board_Transform.position.z) < EPSILON)
                {
                    chessI = n;
                    chessJ = m;
                    break;
                }
            }
        }
        bool flag = (chessI == 0 && chessJ == 0) ? false : true;
        return flag;
    }

    /// <summary>
    /// 检测选中的棋盘格中是否存在棋子
    /// </summary>
    /// <param name="i">选中棋盘的i</param>
    /// <param name="j">选中棋盘的j</param>
    /// <returns>有棋子返回true，无返回false</returns>
    public bool ChessExistCheck(int i, int j)
    {
        int chessI = 0;
        int chessJ = 0;
        float EPSILON = 0.2f;
        board_Transform = chessBoardManager.miniBoard[i, j].GetComponent<Transform>();
        for (int n = 0; n < 2; n++)
        {
            for (int m = 0; m < 16; m++)
            {
                Vector3 position = chessBoardManager.ChessPre[n, m].GetComponent<Transform>().position;
                if (System.Math.Abs(position.x - board_Transform.position.x) < EPSILON && System.Math.Abs(position.z - board_Transform.position.z) < EPSILON)
                {
                    chessI = n;
                    chessJ = m;
                    break;
                }
            }
        }
        bool flag = (chessI == 0 && chessJ == 0) ? false : true;
        return flag;
    }

    /// <summary>
    /// 判断棋子是哪个类型
    /// </summary>
    /// <param name="i">chessI</param>
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
            type[1] = 4;
        }
        else if (j == 2 || j == 5)
        {
            type[1] = 5;
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
    /// <param name="nextCouldStep">返回棋子之后可走的位置</param>
    public void NextStepGuider(int[] chessClass, int[] chessPos, out int nextCouldTimes, out List<int[]> nextCouldStep)
    {
        nextCouldStep = new List<int[]>();
        nextCouldTimes = 0;
        if (chessClass[1] == 0)
        {
            switch (chessClass[0])
            {
                case 0:
                    // 默认情况可走前、左、右
                    if (chessPos[0] != 7)
                    {
                        if (!ChessExistCheck(chessPos[0] + 1, chessPos[1]))
                        {
                            nextCouldStep.Add(new int[2] {chessPos[0] + 1, chessPos[1]});
                            nextCouldTimes++;
                        }
                        if (chessPos[1] != 0 && chessPos[1] != 7)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else if (chessPos[1] == 0)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                        }
                    }
                    else {
                        if (chessPos[1] != 0 && chessPos[1] != 7)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else if (chessPos[1] == 0)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                        }
                    }
                    Debug.Log(Flag + "nextCouldTimes:" + nextCouldTimes);
                    break;
                case 1:
                    // 默认可走后、左、右
                    if (chessPos[0] != 0)
                    {
                        if (!ChessExistCheck(chessPos[0] - 1, chessPos[1]))
                        {
                            nextCouldStep.Add(new int[2] { chessPos[0] - 1, chessPos[1]});
                            nextCouldTimes++;
                        }
                        if (chessPos[1] != 0 && chessPos[1] != 7)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else if (chessPos[1] == 0)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                        }
                    }
                    else
                    {
                        if (chessPos[1] != 0 && chessPos[1] != 7)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else if (chessPos[1] == 0)
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] + 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] + 1 });
                                nextCouldTimes++;
                            }
                        }
                        else
                        {
                            if (!ChessExistCheck(chessPos[0], chessPos[1] - 1))
                            {
                                nextCouldStep.Add(new int[2] { chessPos[0], chessPos[1] - 1 });
                                nextCouldTimes++;
                            }
                        }
                    }
                    Debug.Log(Flag + "nextCouldTimes:" + nextCouldTimes);
                    break;
            }
        }
        else if (chessClass[1] == 1)
        {
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 2)
        {
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 3)
        {
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 4)
        {
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
        else if (chessClass[1] == 5)
        {
            switch (chessClass[0])
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
