using UnityEngine;
namespace DroneController
{
    namespace CustomGUI
    {
        public class DrawGUI : MonoBehaviour
        {
            public static void DrawButton(float x, float y, float w, float h, string txt)
            {
                GUI.Button(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), txt);
            }

            public static void DrawButton(float x, float y, float w, float h, string txt, GUIStyle style)
            {
                GUI.Button(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), txt, style);
            }

            public static void DrawButton(float x, float y, float w, float h, string txt, string hoverTxt, GUIStyle style)
            {
                GUI.Button(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), new GUIContent(txt, hoverTxt), style);
            }
            public static bool DrawButtonReturn(float x, float y, float w, float h, string txt)
            {
                return GUI.RepeatButton(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), txt);
            }
            public static bool DrawButtonReturn(float x, float y, float w, float h, string txt, string hoverTxt, GUIStyle style)
            {
                return GUI.Button(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), new GUIContent(txt, hoverTxt), style);
            }

            public static void DrawTexture(float x, float y, float w, float h, Texture texture)
            {
                GUI.DrawTexture(new Rect(position_x(x), position_y(y), size_x(w), size_y(h)), texture);
            }

            public static Rect Percentages(Rect rect)
            {
                return new Rect(position_x(rect.x), position_y(rect.y), size_x(rect.width), size_y(rect.height));
            }
            public static Vector2 Percentages(Vector2 rect)
            {
                return new Vector2((rect.x / Screen.width * 100), (rect.y / Screen.height * 100));
            }

            //#####		RETURN THE SIZE AND POSITION for GUI images ##################
            private static float position_x(float var)
            {
                return Screen.width * var / 100;
            }
            private static float position_y(float var)
            {
                return Screen.height * var / 100;
            }
            private static float size_x(float var)
            {
                return Screen.width * var / 100;
            }
            private static float size_y(float var)
            {
                return Screen.height * var / 100;
            }
            //#####################################################
        }
    }
}