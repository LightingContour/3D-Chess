using System.Collections;
using System.Collections.Generic;
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

    private Transform m_Transform;
    private readonly string Flag = "ManipulationManager-";

    Material originMaterial;
    Material selectedMaterial;
    GameObject selectedBoard;

    private IEnumerator coroutine;

    private bool catching; // 是否释放
    private bool selecting; // 是否选中
    private float offset = 40; // 棋盘偏移值
    private int[] selectedPos = new int[2]; // 移动格存储

    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        piecesLogicManager = gameObject.GetComponent<PiecesLogicManager>();
        m_Transform = gameObject.GetComponent<Transform>();
        selectedMaterial = Resources.Load<Material>("Materials/ChessBoard/Board_Selecting_Standard");
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

                if(Mathf.Abs(targetPoint.x) <= offset && Mathf.Abs(targetPoint.z) <= offset && catching == false)
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
                            selecting = false;
                        }
                        else
                        {
                            Debug.Log(Flag + "Select Different Position");
                            catching = true;
                            GameObject firstSelBoard = selectedBoard;
                            selectedBoard = chessBoardManager.miniBoard[i, j];
                            Material secondSelMaterial = selectedBoard.GetComponent<Renderer>().material;
                            selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                            coroutine = AfterMoving(firstSelBoard, originMaterial, selectedBoard, secondSelMaterial);
                            StartCoroutine(coroutine);
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
                            Debug.Log(Flag + "Select Success");
                            piecesLogicManager.NextStepGuider(chessClass, selectedPos, out int nextCouldTimes, out List<int[]> nextCouldStep);
                            if (nextCouldTimes > 0)
                            {
                                DoShowCouldStep(nextCouldStep);
                            }
                        } else
                        {
                            // 进行单步取消操作
                            catching = true;
                            selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                            coroutine = CancelOneStep(selectedBoard, originMaterial);
                            StartCoroutine(coroutine);
                        }
                    }
                }
            }
        }
    }

    void DoShowCouldStep(List<int[]> nextCouldStep)
    {
        int[][] index = nextCouldStep.ToArray();
        GameObject[] couldBoard = new GameObject[index.Length];
        for (int i = 0; i < index.Length; i++)
        {
            couldBoard[i] = chessBoardManager.miniBoard[index[i][0], index[i][1]];
        }
        coroutine = ShowCouldStep(couldBoard);
        StartCoroutine(coroutine);
    }

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
    /// <param name="firMaterial">首次点击棋盘的Material</param>
    /// <param name="secBoard">第二次点击的棋盘</param>
    /// <param name="secMaterial">第二次点击棋盘的Material</param>
    /// <returns></returns>
    IEnumerator AfterMoving(GameObject firBoard, Material firMaterial, GameObject secBoard, Material secMaterial)
    {
        yield return new WaitForSeconds(2.0f);
        firBoard.GetComponent<Renderer>().material = firMaterial;
        secBoard.GetComponent<Renderer>().material = secMaterial;
        catching = false;
    }

    /// <summary>
    /// 首次点击失败，重置棋盘颜色
    /// </summary>
    /// <param name="firBoard">首次点击棋盘</param>
    /// <param name="firMaterial">首次点击棋盘的Material</param>
    /// <returns></returns>
    IEnumerator CancelOneStep(GameObject firBoard, Material firMaterial)
    {
        yield return new WaitForSeconds(1.5f);
        firBoard.GetComponent<Renderer>().material = firMaterial;
        catching = false;
    }

    /// <summary>
    /// 显示可以走的格子，显示三秒后消失
    /// </summary>
    /// <param name="couldBoard"></param>
    /// <returns></returns>
    IEnumerator ShowCouldStep(GameObject[] couldBoard)
    {
        Material[] couldMaterial = new Material[couldBoard.Length];
        for (int i = 0; i < couldBoard.Length; i++)
        {
            couldMaterial[i] = couldBoard[i].GetComponent<Renderer>().material;
            couldBoard[i].GetComponent<Renderer>().material = selectedMaterial;
            Material material = couldBoard[i].GetComponent<Renderer>().material;
            couldBoard[i].GetComponent<Renderer>().material.color = new Color(material.color.r, material.color.g, material.color.b, 0.6f);
            SetMaterialRenderingMode(couldBoard[i].GetComponent<Renderer>().material, RenderingMode.Transparent);
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < couldBoard.Length; i++)
        {
            couldBoard[i].GetComponent<Renderer>().material = couldMaterial[i];
        }
    }
}
