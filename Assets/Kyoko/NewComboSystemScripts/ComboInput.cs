using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    [System.Serializable]
    public class ComboInput
    {
        public KyokoStates type;

        public ComboInput(KyokoStates t)
        {
            type = t;
        }
        public bool IsSameAs(ComboInput input)
        {
            return (type == input.type);
        }
    }
}
