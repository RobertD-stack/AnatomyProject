using UnityEngine.EventSystems;
using UnityEngine.UI;
/* *** SUMMARY *** 

*/

// All we do here is make sure that this scrollRect cannot be scrolled using mouse button dragging since we are dragging objects off the menus
public class NonDraggableScrollRect : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
}
