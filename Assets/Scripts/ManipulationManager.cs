using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 枚举Shader4种状态类型
/// </summary>
public enum RenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent,
}

/// <summary>
/// Manipulation manager.
/// 用于管理操作
/// </summary>
public class ManipulationManager : MonoBehaviour
{
    private ChessBoardManager chessBoardManager;
    private PiecesLogicManager piecesLogicManager;
    private CameraManager cameraManager;

    private Transform m_Transform;
    private readonly string Flag = "ManipulationManager-";

    Material originMaterial;
    Material selectedMaterial;
    Material killMaterial;
    GameObject selectedBoard;

    private IEnumerator coroutine;

    private bool catching; // 是否释放，用于阻止同时选中多个棋子
    private bool selecting; // 是否选中，用于进行选择-下棋操作
    private bool isShow = false; // 是否正在展示下一步，用于显示/恢复可走格子
    private float offset = 40; // 棋盘偏移值
    private int[] selectedPos = new int[2]; // 移动格存储
    private bool selectedClass = false; // 记录黑白棋子，白棋为false黑棋为true
    private List<int[]> storedNextCouldStep; // 暂存的下一步可走格，用于下棋后重置棋盘颜色

    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        piecesLogicManager = gameObject.GetComponent<PiecesLogicManager>();
        cameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        m_Transform = gameObject.GetComponent<Transform>();
        selectedMaterial = Resources.Load<Material>("Materials/ChessBoard/Board_Selecting_Standard");
        killMaterial = Resources.Load<Material>("Materials/ChessBoard/Board_Kill_Standard");
    }

    // Update is called once per frame
    void Update()
    {
        GetSelectChess();
    }

    /// <summary>
    /// 获取选中的棋子
    /// </summary>
    void GetSelectChess()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 屏幕坐标转射线
            bool isHit = Physics.Raycast(ray, out RaycastHit hit);
            if (isHit)
            {
                Debug.DrawLine(Camera.main.GetComponent<Transform>().position, 
                    hit.transform.position);
                // Debug.Log(Flag + "Notice:hitpoint=" + hit.point);
                Vector3 targetPoint;
                targetPoint = hit.point;

                if(Mathf.Abs(targetPoint.x) <= offset && Mathf.Abs(targetPoint.z) <= offset && catching == false && piecesLogicManager.animing == false)
                {
                    if (selecting)
                    {
                        // 选中态，进行移动或取消
                        int i = ((Mathf.RoundToInt(targetPoint.z) + 40) / 10);
                        int j = ((Mathf.RoundToInt(targetPoint.x) + 40) / 10);
                        // Debug.Log(Flag + "get i =" + i + " j = " + j);
                        if (selectedPos[0] == i && selectedPos[1] == j)
                        {
                            Debug.Log(Flag + "Select Same Position");
                            selectedBoard.GetComponent<Renderer>().material = originMaterial;
                            if (isShow)
                            {
                                DoShowCouldStep(storedNextCouldStep, isShow);
                            }
                            selecting = false;
                        }
                        else
                        {
                            Debug.Log(Flag + "Select Different Position");
                            catching = true;
                            GameObject firstSelBoard = selectedBoard;
                            selectedBoard = chessBoardManager.miniBoard[i, j];
                            selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                            if (isShow)
                            {
                                DoShowCouldStep(storedNextCouldStep, isShow);
                                int[][] index = storedNextCouldStep.ToArray();
                                for (int a = 0; a < index.Length; a++)
                                {
                                    if (Enumerable.SequenceEqual(index[a], new int[] { i, j, 0 }))
                                    {
                                        Debug.Log(Flag + "Move Success");
                                        int[] firBoardPos = chessBoardManager.GetBoardPos(firstSelBoard);
                                        int[] secBoardPos = chessBoardManager.GetBoardPos(selectedBoard);
                                        piecesLogicManager.ChessMove(firBoardPos[0], firBoardPos[1], secBoardPos[0], secBoardPos[1], false);
                                        selectedClass = !selectedClass;
                                        cameraManager.SwitchPos(selectedClass);
                                        break;
                                    }
                                    if (Enumerable.SequenceEqual(index[a], new int[] { i, j, 1 }))
                                    {
                                        Debug.Log(Flag + "Move Success, kill opposite");
                                        int[] firBoardPos = chessBoardManager.GetBoardPos(firstSelBoard);
                                        int[] secBoardPos = chessBoardManager.GetBoardPos(selectedBoard);
                                        piecesLogicManager.ChessMove(firBoardPos[0], firBoardPos[1], secBoardPos[0], secBoardPos[1], true);
                                        selectedClass = !selectedClass;
                                        cameraManager.SwitchPos(selectedClass);
                                        break;
                                    }
                                }
                            }
                            AfterMoving(firstSelBoard, selectedBoard);
                            selecting = false;
                        }
                    } else
                    {
                        // 未选中态，变为选中
                        int i = ((Mathf.RoundToInt(targetPoint.z) + 40) / 10);
                        int j = ((Mathf.RoundToInt(targetPoint.x) + 40) / 10);
                        int chessI = 0;
                        int chessJ = 0;
                        selecting = piecesLogicManager.ChessExistCheck(i, j, out chessI,out chessJ) ? true : false;
                        if (selecting == true)
                        {
                            if (!((chessI == 0 && selectedClass == false) || (chessI == 1 && selectedClass == true)))
                            {
                                selecting = false;
                            }
                        }
                        // Debug.Log(Flag + "get i =" + i + " j = " + j);
                        selectedBoard = chessBoardManager.miniBoard[i, j];
                        originMaterial = selectedBoard.GetComponent<Renderer>().material;
                        if (selecting)
                        {
                            int[] chessClass = piecesLogicManager.ChessClassCheck(chessI, chessJ);
                            selectedPos[0] = i;
                            selectedPos[1] = j;
                            selectedBoard = chessBoardManager.miniBoard[selectedPos[0], selectedPos[1]];
                            selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                            Debug.Log(Flag + "Select Success, Selected Pos=" + selectedPos[0] + ", " + selectedPos[1]);
                            piecesLogicManager.NextStepGuider(chessClass, selectedPos, out List<int[]> nextCouldStep);
                            if (nextCouldStep.Count > 0)
                            {
                                DoShowCouldStep(nextCouldStep, isShow);
                            }
                        } else
                        {
                            // 进行单步取消操作
                            catching = true;
                            selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                            coroutine = CancelOneStep(selectedBoard);
                            StartCoroutine(coroutine);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置透明色
    /// </summary>
    /// <param name="material"></param>
    /// <param name="renderingMode"></param>
    void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    /// <summary>
    /// 完成移动后，重置棋盘颜色
    /// </summary>
    /// <param name="firBoard">首次点击的棋盘</param>
    /// <param name="secBoard">第二次点击的棋盘</param>
    void AfterMoving(GameObject firBoard, GameObject secBoard)
    {
        chessBoardManager.ResetBoard(firBoard);
        chessBoardManager.ResetBoard(secBoard);
        catching = false;
    }

    /// <summary>
    /// 首次点击失败，重置棋盘颜色
    /// </summary>
    /// <param name="firBoard">首次点击棋盘</param>
    /// <returns></returns>
    IEnumerator CancelOneStep(GameObject firBoard)
    {
        yield return new WaitForSeconds(0.5f);
        chessBoardManager.ResetBoard(firBoard);
        catching = false;
    }

    /// <summary>
    /// 对预览格格子进行数据存储操作
    /// </summary>
    /// <param name="nextCouldStep">下一步可走格子</param>
    /// <param name="showed">是否正在展示下一步</param>
    void DoShowCouldStep(List<int[]> nextCouldStep, bool showed)
    {
        List<int> killBoard = new List<int>();
        // 未展示下一步，进行存储
        if (!showed)
        {
            storedNextCouldStep = nextCouldStep;
        }
        int[][] index = nextCouldStep.ToArray();
        GameObject[] couldBoard = new GameObject[index.Length];
        for (int i = 0; i < index.Length; i++)
        {
            couldBoard[i] = chessBoardManager.miniBoard[index[i][0], index[i][1]];
            if (index[i][2] == 1)
            {
                killBoard.Add(i);
            }
        }
        ShowCouldStep(couldBoard, showed, killBoard);
        isShow = !isShow;
    }

    /// <summary>
    /// 显示或者恢复可以走的格子
    /// </summary>
    /// <param name="couldBoard">可以走到的格子</param>
    /// <param name="showed">显示or恢复格子</param>
    /// <param name="killBoard">吃子的格子序号</param>>
    void ShowCouldStep(GameObject[] couldBoard, bool showed, List<int> killBoard)
    {
        if (!showed)
        {
            Material[] couldMaterials = new Material[couldBoard.Length];
            for (int i = 0; i < couldBoard.Length; i++)
            {
                couldMaterials[i] = couldBoard[i].GetComponent<Renderer>().material;
                if (killBoard.Contains(i))
                {
                    couldBoard[i].GetComponent<Renderer>().material = killMaterial;
                }
                else {
                    couldBoard[i].GetComponent<Renderer>().material = selectedMaterial;
                }
                Material material = couldBoard[i].GetComponent<Renderer>().material;
                couldBoard[i].GetComponent<Renderer>().material.color = new Color(material.color.r, material.color.g, material.color.b, 0.6f);
                SetMaterialRenderingMode(couldBoard[i].GetComponent<Renderer>().material, RenderingMode.Transparent);
            }
        }
        else {
            for (int i = 0; i < couldBoard.Length; i++)
            {
                chessBoardManager.ResetBoard(couldBoard[i]);
            }
        }
    }
}