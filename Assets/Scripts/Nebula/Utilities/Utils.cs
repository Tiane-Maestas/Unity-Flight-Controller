using UnityEngine;
using TMPro;

namespace Nebula
{
    public static class Utils
    {
        // Needs testing in 3D.
        public static void DisplayInfo(Transform transform, string message)
        {
            // Create a canvas to put text onto at the transform passed in.
            GameObject canvasObject = new GameObject("Custom Canvas: " + message);
            canvasObject.transform.SetParent(transform);

            Canvas textCanvas = canvasObject.AddComponent<Canvas>();
            textCanvas.GetComponent<RectTransform>().localPosition = Vector3.zero;
            textCanvas.renderMode = RenderMode.WorldSpace;
            textCanvas.worldCamera = Camera.main;

            // Create text and place it in the canvas.
            GameObject textObject = new GameObject("Custom Text: " + message);
            textObject.transform.SetParent(canvasObject.transform);

            TextMeshPro text = textObject.AddComponent<TextMeshPro>();
            text.GetComponent<RectTransform>().localPosition = Vector3.zero;
            text.autoSizeTextContainer = true;
            text.text = message;
            text.fontSize = 4;
            text.sortingOrder = 1;

            // Destroy it after 1 physics call.
            GameObject.Destroy(canvasObject, Time.fixedDeltaTime);
        }

        public static Vector3 RotateVector3ByDegInWorldCoordinates(Vector3 vector, Vector3 angles)
        {
            return Quaternion.Euler(angles.x, angles.y, angles.z) * vector;
        }
    }
}