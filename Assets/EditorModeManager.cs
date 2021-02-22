using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BeatEmUp

{ public class EditorModeManager : MonoBehaviour
    {
        [SerializeField] private GameObject EdiorUI;
        private bool isActive = false;
        [SerializeField] Dropdown enemiesDropDown;
        [SerializeField] List<AI_RuleEngine> enemies;
        private int index;

        void Update()
        {
            index = enemiesDropDown.value;
            if (Input.GetKeyDown(KeyCode.F12) && isActive)
            {
                EdiorUI.SetActive(false);
                isActive = false;
            }
            else if (Input.GetKeyDown(KeyCode.F12) && !isActive)
            {
                EdiorUI.SetActive(true);
                isActive = true;
            }
        }

        public void SpawnEnemy(GameObject enemy)
        {
            Instantiate(enemies[index]);
        }
        public void RemoveAllEnemies()
        {
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in allEnemies)
            {
                Destroy(enemy);
            }
        }
        public void FillHealth(PlayerCombos player)
        {
            player.kyokoCurrentHealth = player.kyokoMaxHealth;
        }
        public void ResetScene()
        {
            Scene tempScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(tempScene.name);
        }
    }
}
