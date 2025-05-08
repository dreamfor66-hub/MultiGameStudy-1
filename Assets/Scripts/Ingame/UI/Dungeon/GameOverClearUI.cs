using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Dungeon
{
    public class GameOverClearUI : MonoBehaviour
    {
        public static GameOverClearUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        [SerializeField] [Required] private GameObject gameOver;
        [SerializeField] [Required] private Text gameOverText;
        [SerializeField] [Required] private GameObject gameClear;
        [SerializeField] [Required] private Text gameClearText;

        public void ShowGameOver(int count)
        {
            gameOver.SetActive(true);
            gameOverText.text = $"{count} 번 째 지 구 멸 망";
        }

        public void ShowGameClear(int count)
        {
            gameClearText.text = $"{count}번 시도로 클리어!!";
            gameClear.SetActive(true);
        }

        public void HideAll()
        {
            gameOver.SetActive(false);
            gameClear.SetActive(false);
        }
    }
}
