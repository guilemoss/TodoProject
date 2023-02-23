namespace Todo.API.Data;

public class TodoItem
{
    private bool isComplete;

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get => isComplete; set => SetComplete(value); }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow; 
    public DateTimeOffset? CompletedAt { get; private set; }

    private void SetComplete(bool _isComplete)
    {
        isComplete = _isComplete;

        if(_isComplete)
            SetCompleteAt();
    }

    private void SetCompleteAt()
    {
        if (!CompletedAt.HasValue)
            CompletedAt = DateTimeOffset.UtcNow;
    }
}