using System.Collections.Generic;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Core
{
    public static class HealCalculator
    {
        private static readonly List<HealResultInfo> resultBuffer = new List<HealResultInfo>();

        static HealCalculator()
        {
            GameCommandHeal.Listen(OnHeal);
            GameCommandShield.Listen(OnShield);
            GameCommandRevive.Listen(OnRevive);
        }

        public static void GetResult(ref List<HealResultInfo> results)
        {
            results.Clear();
            results.AddRange(resultBuffer);
            resultBuffer.Clear();
        }

        private static void OnHeal(GameCommandHeal cmd)
        {
            var healInfo = cmd.HealInfo;
            if (healInfo.Target is CharacterBehaviour character)
            {
                var finalAmount = CalculateBuff(healInfo);
                var realAmount = character.HpModule.Heal(finalAmount);
                if (realAmount > 0)
                {
                    resultBuffer.Add(new HealResultInfo(healInfo.Target, realAmount));
                }
            }
        }

        public static void OnShield(GameCommandShield cmd)
        {
            var shieldInfo = cmd.ShieldInfo;
            if (shieldInfo.Entity is CharacterBehaviour character)
            {
                character.HpModule.Shield(shieldInfo.Amount, shieldInfo.Frame);
            }
        }

        public static void OnRevive(GameCommandRevive cmd)
        {
            if (cmd.Target is CharacterBehaviour character)
            {
                character.HpModule.Revive(cmd.HpPercent);
            }
        }

        private static int CalculateBuff(HealInfo healInfo)
        {
            var character = healInfo.Target as CharacterBehaviour;
            var basicAmount = healInfo.Amount;
            var finalAmount = healInfo.Amount;
            if (character != null)
            {
                var percentBuff = character.BuffValues.GetValue(BuffSimpleValueType.HealAmountPercent);
                finalAmount += basicAmount * percentBuff / 100;
            }

            return Mathf.Clamp(finalAmount, 0, int.MaxValue);
        }
    }
}