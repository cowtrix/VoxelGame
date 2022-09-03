using UnityEngine;

[System.Serializable]
public class TriColorSet
{
    public enum eColorMode
    {
        Base, Compliment1, Compliment2
    }

    public Color BaseColor;
    public Color Compliment1 => HueShift(BaseColor, 1 / 3f);
    public Color Compliment2 => HueShift(BaseColor, -1 / 3f);

    private static Color HueShift(Color c, float amount)
    {
        Color.RGBToHSV(c, out var h, out var s, out var v);
        h += amount;
        while (h > 1)
            h--;
        while (h < 0)
            h++;
        return Color.HSVToRGB(h, s, v);
    }

    public Color GetColor(eColorMode mode)
    {
        switch (mode)
        {
            case eColorMode.Base:
                return BaseColor;
            case eColorMode.Compliment1:
                return Compliment1;
            case eColorMode.Compliment2:
                return Compliment2;
        }
        return BaseColor;
    }
}
