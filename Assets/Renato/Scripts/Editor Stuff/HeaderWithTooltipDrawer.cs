using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HeaderWithTooltipAttribute))]
public class HeaderWithTooltipDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HeaderWithTooltipAttribute headerWithTooltip = (HeaderWithTooltipAttribute)attribute;

        // Create a rect for the header
        Rect headerRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        // Create a GUI content for the header with a tooltip
        GUIContent headerContent = new(headerWithTooltip.header, headerWithTooltip.tooltip);

        // Draw the header with a tooltip
        EditorGUI.LabelField(headerRect, headerContent, EditorStyles.boldLabel);

        // Move the position down to draw the field under the header
        position.y += EditorGUIUtility.singleLineHeight;

        // Draw the actual property field (next line)
        EditorGUI.PropertyField(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight; // Extra space for the header
    }
}
