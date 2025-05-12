using System.Collections.Generic;

public class BoardSnapshot
{
    public int CurrentPlayerTurnIndex;
    public List<int> BoardPlayerIndexOrder;

    public BoardSnapshot(int currentPlayerTurnIndex, List<int> boardPlayerIndexOrder)
    {
        CurrentPlayerTurnIndex = currentPlayerTurnIndex;
        BoardPlayerIndexOrder = boardPlayerIndexOrder;
    }
}