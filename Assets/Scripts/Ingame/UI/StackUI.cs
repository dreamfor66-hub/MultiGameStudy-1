using System.Collections.Generic;
using Rogue.Ingame.Character;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class StackUI : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour character;
        [SerializeField] private GameObject prefab;

        private readonly List<GameObject> stacks = new List<GameObject>();

        private void Start()
        {
            for (var i = 0; i < character.CharacterData.Stack.BasicMaxStack; i++)
            {
                var obj = Instantiate(prefab, prefab.transform.parent);
                stacks.Add(obj.transform.GetChild(0).gameObject);
            }
            prefab.SetActive(false);
        }

        private void Update()
        {
            for (var i = 0; i < stacks.Count; i++)
            {
                stacks[i].SetActive(i < character.StackModule.Info.CurStack);
            }
        }
    }
}
