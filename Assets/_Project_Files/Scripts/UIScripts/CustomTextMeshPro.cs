using UnityEngine.EventSystems;

public class CustomTextMeshPro : TMPro.TMP_InputField
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.clickCount == 2)
        {
            SelectAll();
            ForceLabelUpdate();
        }
    }
}
