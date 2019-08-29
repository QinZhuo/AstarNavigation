using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstarNavigation;
public class NavigationTest : MonoBehaviour
{
    AstarNavMesh nav;
    public int[,] map;
    public NavPath path;
    [Range(0, 4)]
    public int startX = 0;
    [Range(0, 4)]
    public int startY = 0;
    [Range(0, 4)]
    public int endX = 3;
    [Range(0, 4)]
    public int endY = 3;
    /// <summary>
    /// 测试寻路
    /// </summary>
    [ContextMenu("Test")]
    void Test()
    {
        map = new int[5, 5]
        {
            {0,0,0,0,0 },
            {0,1,1,1,0 },
            {0,1,0,0,0 },
            {0,1,1,0,0 },
            {0,0,0,0,0 }
        };
        nav = new AstarNavMesh();
        nav.Init(map);
        path = nav.GetPath(startX, startY, endX, endY);
    }
    /// <summary>
    /// 绘制Debug画面
    /// </summary>
    private void OnDrawGizmos()
    {
       
        if (nav != null && map != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(startX, 0, startY), 0.4f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(endX, 0, endY), 0.4f);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (nav[i, j].status == 1)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(new Vector3(i, 0, j), Vector3.one);
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(new Vector3(i, 0, j), Vector3.one);
                    }
                    if (nav.openList.Contains(nav[i, j]))
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(new Vector3(i, 0, j), 0.42f);
                    }
                    if (nav.closeList.Contains(nav[i, j]))
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireSphere(new Vector3(i, 0,j), 0.44f);
                    }
                }
            }
            if (path!=null)
            {
                Gizmos.color = Color.green;
                foreach (PathNode node in path)
                {
                    Gizmos.DrawWireCube(new Vector3(node.x, 0, node.y), Vector3.one * 0.9f);
                }
            }
        }
        
    }
}
