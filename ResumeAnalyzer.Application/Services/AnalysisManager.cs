namespace ResumeAnalyzer.Application.Services;

public class AnalysisManager
{
    private CancellationTokenSource? _cts;

    public CancellationToken StartNew()
    {
        _cts?.Cancel(); // Отменяем старый, если он был
        _cts = new CancellationTokenSource();
        return _cts.Token;
    }

    public void Stop()
    {
        _cts?.Cancel();
    }
}