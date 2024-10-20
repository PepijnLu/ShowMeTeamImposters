[System.Serializable]
public class Stat
{
    public float baseValue;

    public void SetValue(float value) 
    {
        baseValue = value;
    }

    public float GetValue() 
    {
        return baseValue;
    }
}
