using System;
using System.Collections.Generic;
using FMLib.UI.OnOff;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.Debugger
{
    [Serializable]
    public class NodeTypeSprite
    {
        public NodeType Type;
        [PreviewField]
        public Sprite Sprite;

        public string Text;
    }

    public class NodeIcon2 : MonoBehaviour
    {
        public Button Button => button;

        [TableList] [SerializeField] private List<NodeTypeSprite> typeToSprites;
        [SerializeField] [Required] private Image image;
        [SerializeField] [Required] private TextMeshProUGUI text;
        [SerializeField] [Required] private Button button;
        [SerializeField] [Required] private OnOffBehaviour disableOnOff;
        [SerializeField] [Required] private OnOffBehaviour checkOnOff;

        [SerializeField] private GameObject[] selectionObjs;

        [Button]
        public void Set(NodeType nodeType)
        {
            var find = typeToSprites.Find(x => x.Type == nodeType);
            image.sprite = find.Sprite;
            text.text = find.Text;
        }

        public void SetButtonActive(bool active)
        {
            button.enabled = active;
        }

        public void SetEnable(bool enable)
        {
            disableOnOff.Set(!enable);
        }

        public void SetCheck(bool enable)
        {
            checkOnOff.Set(enable);
        }

        public void ClearSelection()
        {
            foreach (var selectionObj in selectionObjs)
            {
                selectionObj.SetActive(false);
            }
        }

        public void AddSelection()
        {
            foreach (var obj in selectionObjs)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    break;
                }
            }
        }
    }
}