using UnityEngine;
using UnityEngine.UI;

namespace work.ctrl3d
{
    public class ImageContainer : UIContainer
    {
        [SerializeField] private Image image;
        public bool preserveAspect = true;

        protected override void Awake()
        {
            base.Awake();
            image.color = new Color(1, 1, 1, 0);
        }
    
        public void SetImage(Sprite sprite)
        {
            image.color = new Color(1, 1, 1, 0);
            image.sprite = sprite;
            image.color = new Color(1, 1, 1, 1);
        
            image.preserveAspect = preserveAspect;
        }
    }
}