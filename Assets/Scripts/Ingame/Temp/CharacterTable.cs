using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{
    public class CharacterTable : MonoBehaviour
    {
        public static CharacterTable Instance { get; private set; }

        public List<GameObject> CharacterList;
        [HideInInspector] public GameObject CharacterPrefab => CharacterList[0];

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
