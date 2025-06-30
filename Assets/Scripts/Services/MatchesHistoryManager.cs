using System.Collections.Generic;
using Estructuras_de_Datos.Structures.Hierarchical;

public interface IMatchesHistoryManager
{
    void RegistryMatch(Match match);
    List<Match> GetHistory();
}

public class MatchesHistoryManager : IMatchesHistoryManager
{
    private AVLTree<Match> history = new AVLTree<Match>();
    
    public void RegistryMatch(Match match)
    {
        history.Insert(match);
    }
    
    public List<Match> GetHistory()
    {
        return history.GetSortedElementsDescending();
    }
}
