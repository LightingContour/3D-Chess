using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manipulation manager.
/// 用于管理操作
/// </summary>
public class ManipulationManager : MonoBehaviour
{
    private ChessBoardManager chessBoardManager;

    private Transform m_Transform;
    private readonly string Flag = "ManipulationManager-";

    Material originMaterial;
    Material selectedMaterial;
    GameObject selectedBoard;

    private IEnumerator coroutine;

    private bool selecting; // 是否选中
    private float offset = 40; // 棋盘偏移值
    private int[] selectedPos = new int[2]; // 移动格存储

    // Start is called before the first frame update
    void Start()
    {
        chessBoardManager = GameObject.Find("BoardPlane").GetComponent<ChessBoardManager>();
        selectedMaterial = Resources.Load<Material>("Materials/ChessBoard/Board_Selectiong_Standard");
        m_Transform = gameObject.GetComponent<Transform>();
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
                Debug.Log(Flag + "Notice:hitpoint=" + hit.point);
                Vector3 targetPoint;
                targetPoint = hit.point;

                if(Mathf.Abs(targetPoint.x) <= offset && Mathf.Abs(targetPoint.z) <= offset)
                {
                    if (selecting)
                    {
                        // 选中态，进行移动或取消
                        int i = ((Mathf.RoundToInt(targetPoint.z) + 40) / 10);
                        int j = ((Mathf.RoundToInt(targetPoint.x) + 40) / 10);
                        Debug.Log(Flag + "get i =" + i + " j = " + j);
                        if (selectedPos[0] == i && selectedPos[1] == j)
                        {
                            Debug.Log(Flag + "Select Same Position");
                            selectedBoard.GetComponent<Renderer>().material = originMaterial;
                            selecting = false;
                        }
                        else
                        {
                            Debug.Log(Flag + "Select Different Position");
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
                        selecting = true;
                        int i = ((Mathf.RoundToInt(targetPoint.z) + 40) / 10);
                        int j = ((Mathf.RoundToInt(targetPoint.x) + 40) / 10);
                        Debug.Log(Flag + "get i =" + i + " j = " + j);
                        selectedPos[0] = i;
                        selectedPos[1] = j;
                        selectedBoard = chessBoardManager.miniBoard[i, j];
                        originMaterial = selectedBoard.GetComponent<Renderer>().material;
                        selectedBoard.GetComponent<Renderer>().material = selectedMaterial;
                        Debug.Log(Flag + "Select Success");
                    }
                }
            }
        }
    }

    // 完成移动后，重置棋盘颜色
    IEnumerator AfterMoving(GameObject firBoard, Material firMaterial, GameObject secBoard, Material secMaterial)
    {
        yield return new WaitForSeconds(2.0f);
        firBoard.GetComponent<Renderer>().material = firMaterial;
        secBoard.GetComponent<Renderer>().material = secMaterial;
    }
}
