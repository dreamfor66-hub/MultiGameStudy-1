namespace Rogue.Tool
{
    public static class EditorPaths
    {
        public static string AnimatorControllerPath(string charName) =>
            $"Assets/Animator/AC_{charName}.controller";

        public static string ActionDataPath(string charName, string actionName) =>
            $"Assets/Data/Character/{charName}/Action/Action_{charName}_{actionName}.asset";

        public static string TimelineSceneRootPath =>
            "Assets/Timeline";

        public static string TimelineTemplateScenePath =>
            $"{TimelineSceneRootPath}/TL_Template.unity";

        public static string TimelineScenePath(string charName, string actionName) =>
            $"{TimelineSceneRootPath}/{charName}/TL_{charName}_{actionName}.unity";

        public static string TimelineAssetPath(string charName, string actionName) =>
            $"{TimelineSceneRootPath}/{charName}/TL_{charName}_{actionName}.playable";

        public static string CharacterModelPrefabPath(string charName) =>
            $"Assets/Prefabs/Characters/Model/PF_{charName}.prefab";

        public static string BoltPlayerCharacterPrefabPath(string charName) =>
            $"Assets/Prefabs/Bolt/Player/PF_Bolt_Player_{charName}.prefab";

        public static string BoltMonsterCharacterPrefabPath(string charName) =>
            $"Assets/Prefabs/Bolt/Monster/PF_Bolt_Monster_{charName}.prefab";

        public static string CharacterDataPath(string charName) =>
            $"Assets/Data/Character/{charName}/Data_Character_{charName}.asset";

        public static string BotPatternDataPath(string charName) =>
            $"Assets/Data/BotPattern/Data_BotPattern_{charName}.asset";

        public static string BotPhaseDataPath(string charName) =>
            $"Assets/Data/Character/{charName}/Pattern/Data_BotPhase_{charName}.asset";
    }
}