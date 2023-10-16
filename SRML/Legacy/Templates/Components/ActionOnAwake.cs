using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.Templates.Components
{
    public class ActionOnAwake : MonoBehaviour
    {
        public List<string> actions;

        public void Awake()
        {
            foreach (string action in actions)
                TemplateActions.RunAction(action, gameObject);
        }
    }
}
