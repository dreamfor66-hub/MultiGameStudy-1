using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Bolt;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;

namespace Rogue
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class DummySpawner : GlobalEventListener
    {
        [SerializeField] GameObject DummyPrefab;
        CharacterBehaviour character;
        private CharacterData dummyCharacterData;
        ParticleSystem particleSystem;

        void Start()
        {
            GetParticleData();
            GetDummyData();
            SpawnDummy();
            StartCoroutine(FindDummyCoroutine(2f));
        }

        void Update()
        {

        }

        private void GetParticleData()
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void GetDummyData()
        {
            if (DummyPrefab != null)
            {
                dummyCharacterData = DummyPrefab.GetComponent<CharacterBehaviour>().characterData;
            }
        }

        private void SpawnDummy()
        {
            if (BoltNetwork.IsServer)
            {
                if (DummyPrefab != null)
                {
                    BoltNetwork.Instantiate(DummyPrefab, transform.position, Quaternion.identity);
                    particleSystem.Play();
                }
            }
        }

        private CharacterBehaviour GetDummyFromEntity()
        {
            return EntityTable.FindByType<CharacterBehaviour>().FirstOrDefault(x => x.characterData == dummyCharacterData);
        }

        private void FindDummy()
        {
            var DummyEntity = GetDummyFromEntity();
            if (DummyEntity == null)
            {
                character = null;
                SpawnDummy();
            }
            else
            {
                character = DummyEntity;
            }
        }

        IEnumerator FindDummyCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            FindDummy();
            StartCoroutine(FindDummyCoroutine(delay));
        }

        private void OnDestroy()
        {
            if (character != null)
                BoltNetwork.Destroy(character.gameObject);
        }
    }
}
