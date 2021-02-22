using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    public enum AI_Actions
    {
        Attack,
        Defend,
        Jump,
        Chase,
        Leap,
        Shoot,
        Rest,
    }
    [System.Serializable]
    public class AI_Behaviour
    {
        public AI_Actions ai_Action;
        [HideInInspector]
        public bool isActionDone;
    }

}
