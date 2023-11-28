using UnityEngine;

namespace MasyoLab.Game.DevelopSample
{
    public class BlendShapesSetting : MonoBehaviour
    {
        [SerializeField]
        private BlendShapesSettingData _blendShapesSettingData = null;
        public BlendShapesSettingData BlendShapesSettingData
        {
            get
            {
                return _blendShapesSettingData;
            }
            set
            {
                _blendShapesSettingData = value;
            }
        }

        private void Awake()
        {

        }
    }
}
