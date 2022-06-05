using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;
using NaughtyAttributes;
using TMPro;
using UnityEngine.EventSystems;

namespace GeneralCore
{
    /// <summary>
    /// Managing Game states
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            BeforeGameplay,
            Gameplay,
            LevelCompleted,
            GameOver
        }
        private static GameManager instance;
        public static GameManager Instance => instance ??= FindObjectOfType<GameManager>();

        [BoxGroup("SETTINGS"), SerializeField] private int firstLevelAfterLoop;
        [BoxGroup("GAME STATE UI"), ReadOnly] public GameState CurrentGameState;
        [BoxGroup("GAME STATE UI"), SerializeField] private GameObject beforeGameplayUI;
        [BoxGroup("GAME STATE UI"), SerializeField] private GameObject gameplayUI;
        [BoxGroup("GAME STATE UI"), SerializeField] private GameObject levelCompletedUI;
        [BoxGroup("GAME STATE UI"), SerializeField] private GameObject gameOverUI;

        [BoxGroup("TEXT SETUP"), SerializeField] private TextMeshProUGUI levelText;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Application.targetFrameRate = 60;
            SingleEventSystem();
            DontDestroyOnLoad(this.gameObject);
            LoadReachedLevel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LevelCompleted();
            }

            if (Input.anyKeyDown && CurrentGameState == GameState.BeforeGameplay)
            {
                beforeGameplayUI.SetActive(false);
            }
        }

        public void LoadReachedLevel()
        {
            CurrentGameState = GameState.BeforeGameplay;
            MMVibrationManager.TransientHaptic(1, 0.1f, true, this);
            SceneManager.LoadScene(PlayerPrefs.GetInt("reachedLevel", 1));
            levelText.text = "Level " + PlayerPrefs.GetInt("fakeLevelNumber", 1).ToString();
            levelCompletedUI.SetActive(false);
            gameOverUI.SetActive(false);
            beforeGameplayUI.SetActive(true);
            gameplayUI.SetActive(true);
            levelText.transform.parent.gameObject.SetActive(true);
        }

        public void LevelCompleted()
        {
            CurrentGameState = GameState.LevelCompleted;
            MMVibrationManager.TransientHaptic(1, 0.1f, true, this);
            PlayerPrefs.SetInt("fakeLevelNumber", PlayerPrefs.GetInt("fakeLevelNumber", 1) + 1);
            StartCoroutine(SetUIMenu(levelCompletedUI, 1f, true));
            StartCoroutine(SetUIMenu(gameplayUI, 1f, false));
            StartCoroutine(SetUIMenu(beforeGameplayUI, 1f, false));
            StartCoroutine(SetUIMenu(gameOverUI, 1f, false));

            if (SceneManager.sceneCountInBuildSettings > PlayerPrefs.GetInt("reachedLevel", 2) + 1)
            {
                PlayerPrefs.SetInt("reachedLevel", PlayerPrefs.GetInt("reachedLevel", 2) + 1);
            }
            else
            {
                if (firstLevelAfterLoop <= 0)
                {
                    firstLevelAfterLoop = 1;
                }

                if (SceneManager.sceneCountInBuildSettings <= firstLevelAfterLoop + 1)
                {
                    PlayerPrefs.SetInt("reachedLevel", 2);
                }
                else
                {
                    PlayerPrefs.SetInt("reachedLevel", firstLevelAfterLoop + 1);
                }
            }
        }

        public void GameOver()
        {
            CurrentGameState = GameState.GameOver;
            MMVibrationManager.TransientHaptic(1, 0.1f, true, this);
            StartCoroutine(SetUIMenu(gameOverUI, 3f, true));
            StartCoroutine(SetUIMenu(gameplayUI, 3f, false));
            StartCoroutine(SetUIMenu(beforeGameplayUI, 3f, false));
            StartCoroutine(SetUIMenu(levelCompletedUI, 3f, false));
        }

        public void Gameplay()
        {
            CurrentGameState = GameState.Gameplay;
            levelText.transform.parent.gameObject.SetActive(false);
            gameplayUI.SetActive(true);
            beforeGameplayUI.SetActive(false);
            levelCompletedUI.SetActive(false);
            gameOverUI.SetActive(false);
        }
        private void SingleEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystem.transform.SetParent(gameObject.transform);
            }
        }

        private IEnumerator SetUIMenu(GameObject menu, float time, bool trueOrFalse)
        {
            yield return new WaitForSeconds(time);
            menu.SetActive(trueOrFalse);
        }
    }
}
    