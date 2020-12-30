using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Button))]
    public class AreaButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Image areaImage;

        private const string lockedText = "???";
        private Button button;
        private Image buttonImage;

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
        }

        /// <summary>
        /// Set this area button to display as locked.
        /// </summary>
        public void SetLocked()
        {
            buttonText.text = lockedText;
            areaImage.color = Color.black;
            button.enabled = false;
            buttonImage.enabled = false;
        }
    }
}