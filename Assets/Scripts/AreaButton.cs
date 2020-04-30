using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class AreaButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Image areaImage;

        private const string lockedText = "???";

        /// <summary>
        /// Set this area button to display as locked.
        /// </summary>
        public void SetLocked()
        {
            buttonText.text = lockedText;
            areaImage.color = Color.black;
        }
    }
}