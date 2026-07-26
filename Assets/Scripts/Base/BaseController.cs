using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// The eagle. When its Destructible dies (only enemy bullets can hit it —
    /// see LayerConfig), the game is lost.
    /// </summary>
    [RequireComponent(typeof(Destructible))]
    public class BaseController : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Destructible>().Destroyed += _ =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.TriggerLose();
            };
        }
    }
}
