using UnityEngine;
namespace Shine.Scroller
{
    public class BaseReuseScrollerItem<T> : MonoBehaviour
    {
        public int idx;
        public T data;
        public RectTransform rect;

        public float Top
        {
            get
            {
                return rect.offsetMax.y;
            }
        }

        public float Bottom
        {
            get
            {
                return rect.offsetMin.y;
            }
        }
        public float AnchoredPositionY
        {
            set
            {
                Vector3 anchoredPosition3D = rect.anchoredPosition3D;
                anchoredPosition3D.y = value;
                rect.anchoredPosition3D = anchoredPosition3D;
            }
        }

        public float AnchoredPositionX
        {
            set
            {
                Vector3 anchoredPosition3D = rect.anchoredPosition3D;
                anchoredPosition3D.x = value;
                rect.anchoredPosition3D = anchoredPosition3D;
            }
        }
        public Vector3 AnchoredPosition
        {
            set
            {
                rect.anchoredPosition3D = value;
            }
        }

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public virtual void UpdateData(T data)
        {
            this.data = data;
        }
    }
}