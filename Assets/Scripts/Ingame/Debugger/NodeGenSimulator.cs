using Rogue.Ingame.Data;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Debugger
{
    public class NodeGenSimulator : MonoBehaviour
    {
        public DungeonData DungeonData;
        public NodeMapDrawer Drawer;

        public bool SeedFixed;

        [EnableIf(nameof(SeedFixed))]
        public int Seed;

        [Button]
        public void Generate()
        {
#if UNITY_EDITOR
            if (!SeedFixed)
                Seed = UnityEditor.GUID.Generate().GetHashCode();
#endif
            var generator = new NodeGenerator(DungeonData.NodeGenData, Seed);
            var result = generator.Generate();
            Drawer.Init(result);
        }

    }
}
