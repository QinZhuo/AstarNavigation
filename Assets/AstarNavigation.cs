using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace AstarNavigation
{
    /// <summary>
    /// 寻路信息
    /// </summary>
    public class NavPath
    {
        public PathNode start;
        /// <summary>
        /// 实现迭代器 用于foreach
        /// </summary>
        /// <returns>路径节点</returns>
        public IEnumerator<PathNode> GetEnumerator()
        {
            var current = start;
            while (current!=null)
            {
                yield return current;
                current = current.next;
            }
        }

    }
    /// <summary>
    /// 路径节点
    /// </summary>
    public class PathNode
    {
        public int x;
        public int y;
        public PathNode next;
    }
    /// <summary>
    /// 寻路网格
    /// </summary>
    public class AstarNavMesh
    {
        /// <summary>
        /// 开启列表 即将搜索的节点
        /// </summary>
        public List<AstarNode> openList = new List<AstarNode>();
        /// <summary>
        /// 关闭列表 不再搜索的节点
        /// </summary>
        public List<AstarNode> closeList = new List<AstarNode>();
        /// <summary>
        /// 寻路网格信息
        /// </summary>
        AstarNode[,] map;
        /// <summary>
        /// 获取节点信息 出界会返回null
        /// </summary>
        public AstarNode this[int x, int y]
        {
            get
            {
                if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1))
                {
                    return map[x, y];
                }
                return null;
            }
        }
        /// <summary>
        /// 初始化寻路信息
        /// </summary>
        /// <param name="newMap">0代表可通过1代表障碍物</param>
        public void Init(int[,] newMap)
        {
            map = new AstarNode[newMap.GetLength(0), newMap.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new AstarNode(i, j);
                    map[i, j].status = newMap[i, j];
                }
            }
        }
        /// <summary>
        /// 寻找通路
        /// </summary>
        public bool FindPath(AstarNode start, AstarNode end)
        {
            if (start == null || end == null) return false;
            openList.Clear();
            closeList.Clear();
            start.Clear();
            openList.Add(start);
            AstarNode current = null;
            while (openList.Count > 0)
            {
                current = MinCost(openList);
                openList.Remove(current);
                closeList.Add(current);
                if (current == end)
                {
                    break;
                }
                else
                {
                    foreach (var node in CanArriveNode(current))
                    {
                        if (closeList.Contains(node))
                        {
                            continue;
                        }
                        else if (openList.Contains(node))
                        {
                            if (current.g + 1 < node.g)
                            {
                                node.g = current.g + 1;
                                node.lastNode = current;
                            }
                        }
                        else
                        {
                            openList.Add(node);
                            node.h=node.Distance(end);
                            node.g = current.g + 1;
                            node.lastNode = current;
                        }
                    }
                }

            }
            if (current == end)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获得寻路路径
        /// </summary>
        public NavPath GetPath(int startX,int startY,int endX,int endY)
        {
            if (FindPath(this[startX, startY], this[endX, endY]))
            {
                var currentNode = this[endX, endX];
                PathNode currentPath =null;
                PathNode lastPath = null;
                while (currentNode != this[startX, startY])
                {
                    if (currentNode.lastNode == null)
                    {
                        return null;
                    }

                    currentPath = new PathNode()
                    {
                        x = currentNode.x,
                        y = currentNode.y,
                        next = lastPath,
                    };
                    currentNode = currentNode.lastNode;
                    lastPath = currentPath;
                }
                return new NavPath() {start=currentPath};
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有可走节点
        /// </summary>
        public List<AstarNode> CanArriveNode(AstarNode center)
        {
            var arrive = new List<AstarNode>();
            arrive.Add(this[center.x + 1, center.y]);
            arrive.Add(this[center.x - 1, center.y]);
            arrive.Add(this[center.x, center.y + 1]);
            arrive.Add(this[center.x, center.y - 1]);
            return ClearNull(arrive);
        }
        /// <summary>
        /// 清除不合法节点与不可走节点
        /// </summary>
        public List<AstarNode> ClearNull(List<AstarNode> list)
        {
            for (int i = 0; i < list.Count;)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
                else
                {
                    if (list[i].status != 0)
                    {
                        list.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }

                }
            }
            return list;
        }
        /// <summary>
        /// 获取最小距离花费节点
        /// </summary>
        /// <param name="list">可走节点列表</param>
        public AstarNode MinCost(List<AstarNode> list)
        {
            AstarNode min = null;
            foreach (var node in list)
            {
                if (min == null) { min = node; continue; }
                if (min > node)
                {
                    min = node;
                }
            }
            return min;
        }
    }
    /// <summary>
    /// A*寻路节点
    /// </summary>
    public class AstarNode
    {
        public int x;
        public int y;
        /// <summary>
        /// 状态值 0代表可通过 1代表障碍物
        /// </summary>
        public int status;
        /// <summary>
        /// 当前节点距离起点的真实值
        /// </summary>
        public int g;
        /// <summary>
        /// 当前节点距离终点的预估值
        /// </summary>
        public int h;
        /// <summary>
        /// 经过当前节点时起点到终点的距离预估值
        /// </summary>
        public int f
        {
            get
            {
                return g + h;
            }
        }
        /// <summary>
        /// 上一个节点
        /// </summary>
        public AstarNode lastNode;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public AstarNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
       
        public int Distance(AstarNode other)
        {
            return Mathf.Abs(other.x - x) + Mathf.Abs(other.y - y);
        }
        public void Clear()
        {
            g = 0;
            h = 0;
            lastNode = null;
        }
        /// <summary>
        /// 重载>操作符 判断总距离花费
        /// </summary>
        public static bool operator >(AstarNode a, AstarNode b)
        {
            if (a.f > b.f)
            {
                return true;
            }
            else if (a.f < b.f) return false;
            else
            {
                if (a.g > b.g) return true;
                else if (a.g == b.g)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool operator <(AstarNode a, AstarNode b)
        {
            return b > a;
        }
    }

}