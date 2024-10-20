using UnityEngine;

public class HeaderWithTooltipAttribute : PropertyAttribute
{
    public string header;
    public string tooltip;

    public HeaderWithTooltipAttribute(string header, string tooltip)
    {
        this.header = header;
        this.tooltip = tooltip;
    }
}
