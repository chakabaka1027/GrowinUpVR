using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent (typeof (RectTransform), typeof (Collider2D))]
public class ColliderDetectMouseover : MonoBehaviour  {
        Collider2D myCollider;
       
        void Awake ()  {
                myCollider = GetComponent<Collider2D>();
        }
 
        public bool isMouseOver() {
                return myCollider.OverlapPoint (new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
}
