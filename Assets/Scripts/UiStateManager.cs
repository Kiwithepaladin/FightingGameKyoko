using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUp
{
    public class UiStateManager : MonoBehaviour
    {
        [SerializeField] private PlayerControllerScript player;
        [SerializeField] private Text previousStateText, earliestStateText;
        private void Update()
        {
            previousStateText.text = player.previousStateEnum.ToString();
            earliestStateText.text = player.earliestStateEnum.ToString();
        }
    }
}
