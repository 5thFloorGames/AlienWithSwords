#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
    public static class SabreMouse
    {
        static MouseCursor activeCursor = MouseCursor.Arrow;

        public static MouseCursor ActiveCursor
        {
            get { return activeCursor; }
        }

        public static void ResetCursor()
        {
            activeCursor = MouseCursor.Arrow;
        }

        public static void SetCursor(MouseCursor mouseCursor)
        {
            activeCursor = mouseCursor;
        }

        public static void SetCursorFromVector3(Vector2 currentPosition, Vector2 lastPosition)
        {
            Vector3 delta = currentPosition - lastPosition;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);

            while (angle < 0)
            {
                angle += 180;
            }

            while (angle > 180)
            {
                angle -= 180;
            }

            if (angle >= 67.5f && angle < 112.5f)
            {
                activeCursor = MouseCursor.ResizeVertical;
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                activeCursor = MouseCursor.ResizeUpLeft;
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                activeCursor = MouseCursor.ResizeUpRight;
            }
            else
            {
                activeCursor = MouseCursor.ResizeHorizontal;
            }
        }
    }
}
#endif