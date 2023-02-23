namespace Todo.API.Data;

public class TodoItemDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    public TodoItemDTO() { }
    public TodoItemDTO(TodoItem todoItem) =>
    (Id, Title, IsComplete) = (todoItem.Id, todoItem.Title, todoItem.IsComplete);
}