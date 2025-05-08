using Rogue.Ingame.Dungeon;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Audio
{
    public class BGMPlayer : MonoBehaviour
    {
        [SerializeField] [Required] private AudioSource defaultSource;
        [SerializeField] [Required] private AudioSource bossSource;

        private bool isBoss = false;

        private void Start()
        {
            isBoss = false;
            defaultSource.Play();
            bossSource.Stop();
        }
        private void Update()
        {
            var curBoss = false;
            if (DungeonModel.Instance.IsInit)
            {
                curBoss = DungeonModel.Instance.NodeMapModel.CurNode.Type == NodeType.Boss;
            }

            if (!isBoss && curBoss)
            {
                bossSource.Play();
                defaultSource.Stop();
            }

            if (isBoss && !curBoss)
            {
                bossSource.Stop();
                defaultSource.Play();
            }

            isBoss = curBoss;
        }
    }
}
