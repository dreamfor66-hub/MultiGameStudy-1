using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Rogue.Tool.BuffFinder
{
    public class BuffFinderWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Buff/Buff Finder")]
        public static void ShowWindow()
        {
            GetWindow<BuffFinderWindow>().Show();
        }
        public List<BuffData> All;
        public List<BuffData> Finds;

        [Button]
        public void FindAll()
        {
            var guids = AssetDatabase.FindAssets($"t:BuffData");
            All = guids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<BuffData>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }

        [Button]
        public void FindByConditionType(BuffConditionType conditionType)
        {
            Find(buff => buff.Buffs.Any(x => x.Conditions.Any(c => c.ConditionType == conditionType)));
        }

        [Button]
        public void FindBySimpleValueType(BuffSimpleValueType simpleValueType)
        {
            Find(buff => buff.Buffs.Any(x => x.SimpleEffects.Any(s => s.ValueType == simpleValueType)));
        }

        [Button]
        public void FindByTriggerType(BuffTriggerType triggerType)
        {
            Find(buff => buff.Buffs.Any(x => x.TriggerActiveEffects.Any(t => t.TriggerData.TriggerType == triggerType)));
        }

        [Button]
        public void FindByActiveType(BuffActiveType activeType)
        {
            Find(buff => buff.Buffs.Any(x => x.TriggerActiveEffects.Any(t => t.ActiveDataList.Any(t => t.ActiveType == activeType))));
        }


        private void Find(Func<BuffData, bool> checker)
        {
            Finds.Clear();
            foreach (var buff in All)
            {
                if (checker(buff))
                    Finds.Add(buff);
            }


        }
    }
}
