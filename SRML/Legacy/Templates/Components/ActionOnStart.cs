using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.Templates.Components
{
    public class ActionOnStart : MonoBehaviour
    {
        public List<string> actions;

        public void Start()
        {
            foreach (string action in actions)
                TemplateActions.RunAction(action, gameObject);
        }
    }
}
