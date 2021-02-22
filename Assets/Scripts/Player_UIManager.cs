using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeatEmUp
{
    public class Player_UIManager : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxEnergy;
        [SerializeField] private float currentEnergy;
        private PlayerCombos kyoko;
        [SerializeField] private Image[] heartPosition;
        [SerializeField] private Image[] energyPosition;
        [Header("Full / Empty Sprites")]
        [SerializeField] private  Sprite fullHeart;
        [SerializeField] private  Sprite emptyHeart;
        [SerializeField] private  Sprite fullEnergy;
        [SerializeField] private  Sprite emptyEnergy;

        private void Start()
        {
            kyoko = GameObject.Find("Kyoko").GetComponent<PlayerCombos>();
        }

        private void Update()
        {
            ShowHearts();
            ShowEnergy();
        }

        private void ShowHearts()
        {
            currentHealth = kyoko.kyokoCurrentHealth / 5;
            maxHealth = kyoko.kyokoMaxHealth / 5;

            for (int i = 0; i < heartPosition.Length; i++)
            {
                if (i < currentHealth)
                {
                    heartPosition[i].sprite = fullHeart;
                }
                else if (i >= currentHealth)
                {
                    heartPosition[i].sprite = emptyHeart;
                }
                if (i < maxHealth)
                {
                    heartPosition[i].enabled = true;
                }
                else
                {
                    heartPosition[i].enabled = false;
                }
            }
        }
        private void ShowEnergy()
        {
            currentEnergy = kyoko.kyoko_CurrentEnergy / 50;
            maxEnergy = kyoko.kyoko_MaxEnergy / 50;

            for (int i = 0; i < energyPosition.Length; i++)
            {
                if (i < currentEnergy)
                {
                    energyPosition[i].sprite = fullEnergy;
                }
                else if (i >= currentEnergy)
                {
                    energyPosition[i].sprite = emptyEnergy;
                }
                if (i < maxEnergy)
                {
                    energyPosition[i].enabled = true;
                }
                else
                {
                    energyPosition[i].enabled = false;
                }
            }
        }
    }
}
