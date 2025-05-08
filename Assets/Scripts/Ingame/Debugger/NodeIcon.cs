using System;
using System.Collections.Generic;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Debugger
{
    [Serializable]
    public class NodeTypeToObj
    {
        public NodeType Type;
        public GameObject GameObject;
    }
    public class NodeIcon : MonoBehaviour
    {
        [TableList]
        public List<NodeTypeToObj> TypeToObjs;

        public void Set(NodeType nodeType)
        {
            foreach (var typeObj in TypeToObjs)
            {
                typeObj.GameObject.SetActive(typeObj.Type == nodeType);
            }
        }
    }
}