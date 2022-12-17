using System.IO;
using UnityEngine;

namespace Nebula
{
    public static class Utils2D
    {
        // This method will return a Vector2 increased/decreased by a factor specified to the
        // vector component specified.
        public static Vector2 SmoothVectorTo(Vector2 toChange, float factor, string component = "xy")
        {
            if (component == "x")
            {
                return new Vector2(toChange.x * factor, toChange.y);
            }
            else if (component == "y")
            {
                return new Vector2(toChange.x, toChange.y * factor);
            }
            else
            {
                return new Vector2(toChange.x * factor, toChange.y * factor);
            }
        }

        public static Vector2 RotateVector2ByRad(Vector2 vector, float angle)
        {
            return new Vector2((vector.x * Mathf.Cos(angle)) - (Mathf.Sin(angle) * vector.y),
                               (vector.x * Mathf.Sin(angle)) + (Mathf.Cos(angle) * vector.y));
        }

        public static Vector2 RotateVector2ByDeg(Vector2 vector, float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2((vector.x * Mathf.Cos(angle)) - (Mathf.Sin(angle) * vector.y),
                               (vector.x * Mathf.Sin(angle)) + (Mathf.Cos(angle) * vector.y));
        }

        public static Texture2D LoadTextureFromFile(string filePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D. Null if fails.
            Texture2D texture;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                texture = new Texture2D(0, 0);
                // Set texture if readable. Sized Automatically.
                if (texture.LoadImage(fileData))
                    return texture;
            }
            return null;
        }
    }
}