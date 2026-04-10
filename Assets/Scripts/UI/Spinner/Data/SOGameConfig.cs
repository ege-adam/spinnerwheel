using UnityEngine;

namespace Scripts.UI.Spinner.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/UI/Spinner/GameConfig", order = 0)]
    public class SOGameConfig : ScriptableObject
    {
        [Header("Revive")]
        [SerializeField] private int reviveCost = 25;

        public int ReviveCost => reviveCost;
    }
}
