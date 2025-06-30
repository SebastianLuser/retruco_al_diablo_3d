using System;

[System.Serializable]
public class Match  : IComparable<Match>
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Player1 { get; set; }
    public string Result { get; set; }
    
    public int CompareTo(Match other)
    {
        return Date.CompareTo(other.Date);
    }
    
    public override string ToString()
    {
        return $"{Date:dd/MM/yyyy HH:mm} - {Player1} vs Diablo: {Result}";
    }
}




