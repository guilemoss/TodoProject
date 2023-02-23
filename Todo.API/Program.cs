using Microsoft.EntityFrameworkCore;
using Todo.API.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

RouteGroupBuilder todos = app.MapGroup("/v1/todos");

todos.MapGet("/", GetAllTodos);
todos.MapGet("/complete", GetCompleteTodos);
todos.MapGet("/{id}", GetTodo);
todos.MapPost("/", CreateTodo);
todos.MapPut("/{id}", UpdateTodo);
todos.MapDelete("/{id}", DeleteTodo);

RouteGroupBuilder todosByAdmin = app.MapGroup("/v1/admin/todos");

todosByAdmin.MapGet("/", GetAllTodosByAdminMode);

app.Run();

static async Task<IResult> GetAllTodosByAdminMode(TodoDbContext db) 
    => TypedResults.Ok(await db.Todos.ToArrayAsync());

static async Task<IResult> GetAllTodos(TodoDbContext db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDbContext db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDbContext db)
{
    return await db.Todos.FindAsync(id)
        is TodoItem todo
            ? TypedResults.Ok(new TodoItemDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDbContext db)
{
    var todoItem = new TodoItem
    {
        IsComplete = todoItemDTO.IsComplete,
        Title = todoItemDTO.Title
    };
    
    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return TypedResults.Created($"/v1/todos/{todoItem.Id}", todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    if(!string.IsNullOrEmpty(todoItemDTO.Title))
        todo.Title = todoItemDTO.Title;
    
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDbContext db)
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.Ok(todo);
    }

    return TypedResults.NotFound();
}