using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigCallbackAttribute : Attribute
    {

        public delegate void OnValueChangedDelegate(object originalValue, object newValue);

        public OnValueChangedDelegate OnValueChanged;

        public ConfigCallbackAttribute(OnValueChangedDelegate del)
        {
            OnValueChanged = del;
        }

        
    }

}
