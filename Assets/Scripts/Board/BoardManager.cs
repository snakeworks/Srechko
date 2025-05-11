using System.Collections.Generic;

public class BoardManager : StaticInstance<BoardManager>
{
    public BoardPlayer CurrentPlayer { get; private set; }

    private List<BoardPlayer> _boardPlayers = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init()
    {

    }

    public BoardSnapshot GetBoardSnapshot()
    {
        return new BoardSnapshot();
    }
}
