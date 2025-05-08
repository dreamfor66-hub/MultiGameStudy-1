using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rogue.Ingame.Data
{
    [Flags]
    public enum DirectionMask
    {
        Neutral = 1,
        Direction = 2,
        All = 3
    }
    [Serializable]
    public class CommandToAction
    {
        public string StateKey;
        public bool FromIdle = true;
        public bool FromGetup = false;
        public DirectionMask DirectionMask = DirectionMask.All;
        public CharacterCommandType Command;
        public ActionData Action;
    }

    [Serializable]
    [Title("Hp")]
    [HideLabel]
    public class HpData
    {
        public int BasicMaxHp = 100;
        public bool ImmuneInstantDeath = false;
    }

    [Serializable]
    [Title("Stamina")]
    [HideLabel]
    public class StaminaData
    {
        public int BasicMaxStamina = 2;
        public int ChargingFramePerStamina = 180;
    }


    [Serializable]
    [Title("Buff")]
    [HideLabel]
    public class CharacterBuffData
    {
        [ValidateInput(nameof(Validate), "Buff Table 에 포함되지 않은 에셋이 존재합니다. 테이블을 업데이트 해주세요.")]
        public List<BuffData> StartBuffs = new List<BuffData>();
        private bool Validate() => StartBuffs.All(TableChecker.IsInTable);
    }


    [Serializable]
    [Title("Stack")]
    [HideLabel]
    public class CharacterStackData
    {
        public int BasicMaxStack = 0;
        public int InitStack = 0;
    }

    [CreateAssetMenu(fileName = "new CharacterData", menuName = "Data/Character")]
    public class CharacterData : ScriptableObject
    {
        public float AccTime = 0.1f;

        public float RotateSpeed = 1080f;
        public float Speed = 6.5f;
        public float WalkSpeed = 2f;
        public ActionData AppearAction;

        [TableList]
        public List<CommandToAction> CommandToActions;
        public List<ActionData> PossibleActions;

        [Title("Animation")]
        public int KnockbackMidGetUpFrame;
        public bool KnockbackMidUseHeight;
        public int KnockbackHighGetUpFrame;
        public bool KnockbackHighUseHeight;

        public bool ExtraRunAnimation;
        public bool ExtraWalkAnimation;
        public float WalkStartFrame;
        public float WalkRotateSpeed = 60;

        public HpData Hp = new HpData();
        public StaminaData Stamina = new StaminaData();
        public CharacterStackData Stack = new CharacterStackData();
        public CharacterBuffData Buff = new CharacterBuffData();


        [Title("Etc")]
        public bool IsBoss;
        public bool DefaultSuperArmor;
    }
}
