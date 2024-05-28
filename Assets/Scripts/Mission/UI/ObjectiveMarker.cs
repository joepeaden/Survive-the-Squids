using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Tracks a given object with a UI element.
/// </summary>
public class ObjectiveMarker : MonoBehaviour
{
    /// <summary>
    /// How far away from edges of screen should this marker be?
    /// </summary>
    public int padding;
    /// <summary>
    /// How high above the objective should the marker sit?
    /// </summary>
    public int heightOffset;

    [SerializeField] protected TMP_Text label;
    [SerializeField] protected Image dotImage;
    public Transform objectiveTransform;
    private RectTransform rectTrans;

    Camera mainCamera;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void SetData(Transform objectToMark, string newLabel)
    {
        objectiveTransform = objectToMark;

        label.text = newLabel;
    }

    public void Update()
    {
        if (objectiveTransform != null)
        {
            Vector3 objScreenPos = mainCamera.WorldToScreenPoint(objectiveTransform.position);
            objScreenPos.y += heightOffset;

           
            // make sure we're not off-screen
            objScreenPos = new Vector3(Mathf.Clamp(objScreenPos.x, padding, Screen.width - padding), Mathf.Clamp(objScreenPos.y, padding, (Screen.height - padding)), objScreenPos.z);
           
            rectTrans.position = objScreenPos;
        }
        else
        {
            // whenever the objectiveTransform is destroyed, destroy this marker.
            Destroy(gameObject);
        }
    }
}
