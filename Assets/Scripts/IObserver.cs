public interface IObserver
{
    public void OnRestartDataNotify(int currentNumberOfSquaresInRow, int currentNumberOfMines) { }
    public void OnStatusNotify(EnumGameStatus status) { }
}
