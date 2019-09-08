using UnityEngine;

namespace Unibill.Demo
{
    public class ComboBox
    {
        private static bool s_ForceToUnShow;
        private static int s_UseControlId = -1;
        private bool m_IsClickedComboButton;
        private int m_SelectedItemIndex;

        public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle listStyle)
        {
            return List(rect, new GUIContent(buttonText), listContent, "button", "box", listStyle);
        }

        public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
        {
            return List(rect, buttonContent, listContent, "button", "box", listStyle);
        }

        public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle,
            GUIStyle listStyle)
        {
            return List(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle, listStyle);
        }

        public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent,
            GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
        {
            if (s_ForceToUnShow)
            {
                s_ForceToUnShow = false;
                m_IsClickedComboButton = false;
            }

            bool done = false;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.MouseUp:
                {
                    if (m_IsClickedComboButton)
                        done = true;
                }
                    break;
            }

            if (GUI.Button(rect, buttonContent, buttonStyle))
            {
                if (s_UseControlId == -1)
                {
                    s_UseControlId = controlId;
                    m_IsClickedComboButton = false;
                }

                if (s_UseControlId != controlId)
                {
                    s_ForceToUnShow = true;
                    s_UseControlId = controlId;
                }
                m_IsClickedComboButton = true;
            }

            if (m_IsClickedComboButton)
            {
                Rect listRect = new Rect(rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                    rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length);

                GUI.Box(listRect, "", boxStyle);
                int newSelectedItemIndex = GUI.SelectionGrid(listRect, m_SelectedItemIndex, listContent, 1, listStyle);
                if (newSelectedItemIndex != m_SelectedItemIndex)
                    m_SelectedItemIndex = newSelectedItemIndex;
            }

            if (done)
                m_IsClickedComboButton = false;

            return GetSelectedItemIndex();
        }

        public int GetSelectedItemIndex()
        {
            return m_SelectedItemIndex;
        }
    }
}
