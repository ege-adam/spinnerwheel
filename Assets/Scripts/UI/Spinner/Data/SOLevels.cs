using System.Linq;
using TriInspector;
using UnityEngine;

namespace Scripts.UI.Spinner.Data
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/UI/Spinner/LevelsConfig", order = 0)]
    public class SOLevels : ScriptableObject
    {

        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private RuleDefinition[] rules;

        public LevelConfig[] Levels { get => levels; }

        void OnValidate()
        {
            if (levels == null || levels.Length == 0)
                return;

            var levelCount = levels.Length;

            foreach (var level in levels)
            {
                level.IsWritable = true;
            }

            var sorted = rules.OrderBy(r => r.EveryXthLevel);

            foreach (var rule in sorted)
            {
                if (rule.EveryXthLevel > levelCount || rule.EveryXthLevel <= 0)
                {
                    continue;
                }

                for (int i = rule.EveryXthLevel - 1; i < levelCount; i += rule.EveryXthLevel)
                {
                    if (levels[i].IsWritable || levels[i].Spinner != rule.Spinner)
                    {
                        levels[i].Spinner = rule.Spinner;
                        levels[i].IsWritable = false;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class LevelConfig
    {
        [HideInInspector] public bool IsWritable = true;
        [EnableIf(nameof(IsWritable))] public SOSpinner Spinner;
    }

    [System.Serializable]
    public class RuleDefinition
    {
        [SerializeField] private int everyXthLevel;
        [SerializeField] private SOSpinner spinner;

        public int EveryXthLevel { get => everyXthLevel; }
        public SOSpinner Spinner { get => spinner; }
    }
}