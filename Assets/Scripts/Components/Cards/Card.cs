using System;

public enum Suit 
{
    Espada, 
    Basto, 
    Oro, 
    Copa
}

public enum Rank 
{
    As = 1, 
    Dos = 2, 
    Tres = 3, 
    Cuatro = 4, 
    Cinco = 5, 
    Seis = 6, 
    Siete = 7, 
    Sota = 10,
    Caballo = 11,
    Rey = 12
}

[Serializable]
public class Card
{
    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
    
    public int GetPower()
    {
        if (rank == Rank.As && suit == Suit.Espada) return 14;
        if (rank == Rank.As && suit == Suit.Basto) return 13;
        if (rank == Rank.Siete && suit == Suit.Espada) return 12;
        if (rank == Rank.Siete && suit == Suit.Oro) return 11;
        if (rank == Rank.Tres) return 10;
        if (rank == Rank.Dos) return 9;
        if (rank == Rank.As && (suit == Suit.Copa || suit == Suit.Oro)) return 8;
        if (rank == Rank.Rey) return 7;
        if (rank == Rank.Caballo) return 6;
        if (rank == Rank.Sota) return 5;
        if (rank == Rank.Siete && (suit == Suit.Basto || suit == Suit.Copa)) return 4;
        if (rank == Rank.Seis) return 3;
        if (rank == Rank.Cinco) return 2;
        if (rank == Rank.Cuatro) return 1;
        return 0;
    }

    public override string ToString()
    {
        return $"{rank} de {suit}";
    }
}
