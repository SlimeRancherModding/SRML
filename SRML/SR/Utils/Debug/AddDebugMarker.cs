using UnityEngine;

namespace SRML.SR.Utils.Debug
{
    public class AddDebugMarker : MonoBehaviour
    {
        public MarkerType type;
        public float scaleMult;

        public void Start()
        {
            gameObject.SetDebugMarker(type, scaleMult);
            Destroy(this);
        }
    }
}
