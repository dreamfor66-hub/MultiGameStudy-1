using Photon.Bolt;
using Rogue.Ingame.Stage;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class BoltPlayerObject
    {
        public BoltPlayerController Character;
        public BoltConnection Conneciton;

        public bool IsServer => Conneciton == null;
        public bool IsClient => Conneciton != null;

        public void Spawn(GameObject characterPrefab = null)
        {
            if (!Character)
            {
                if (characterPrefab == null)
                {
                    characterPrefab = CharacterTable.Instance != null ? CharacterTable.Instance.CharacterPrefab : null;
                }

                var startPositions = Object.FindObjectOfType<StartPositions>();
                var pos = Vector3.zero;
                var rot = Quaternion.identity;
                if (startPositions != null)
                {
                    var tm = startPositions.GetTransform();
                    pos = tm.position;
                    rot = tm.rotation;
                }

                if (characterPrefab != null)
                {
                    Character = BoltNetwork.Instantiate(characterPrefab, pos, rot).GetComponent<BoltPlayerController>();
                }
                else
                {
                    Character = BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_Player_Kim, pos, rot).GetComponent<BoltPlayerController>();
                }


                if (IsServer)
                    Character.entity.TakeControl();
                else
                    Character.entity.AssignControl(Conneciton);
            }
        }

        public void ChangeCharacter(int characterNum)
        {
            if (CharacterTable.Instance.CharacterList.Count <= characterNum)
                return;

            Destroy();
            Spawn(CharacterTable.Instance.CharacterList[characterNum]);
        }
        public void Destroy()
        {
            if (Character)
            {
                BoltNetwork.Destroy(Character.gameObject);
                Character = null;
            }
        }
    }



}
