using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BeatEmUp
{[System.Serializable]
    public class Combo
    {
        public string name;
        public List<ComboInput> inputs;
        public Attack comboAttack;
        public UnityEvent onInput;
        int currentInput = 0;

        public bool ContinueCombo(ComboInput i)
        {
            if (inputs[currentInput].IsSameAs(i))
            {
                currentInput++;
                if (currentInput >= inputs.Count)
                {
                    onInput.Invoke();
                    ResetCombo();
                }
                return true;
            }
            else
            {
                ResetCombo();
                return false;
            }
        }

        public ComboInput CurrentComboInput()
        {
            if (currentInput >= inputs.Count) return null;
            return inputs[currentInput];
        }

        public void ResetCombo()
        {
            currentInput = 0;
        }
    }
}
