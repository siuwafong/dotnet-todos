using Microsoft.EntityFrameworkCore;

public static class TodoHandlers
{
    public static async Task<IResult> GetAllTodos(TodoDb db)
    {
        return TypedResults.Ok(await db.Todos.Select(todo => new TodoItemDTO(todo)).ToArrayAsync());
    }
    
    public static async Task<IResult> GetCompleteTodos(TodoDb db)
    {
        return TypedResults.Ok(await db.Todos.Where(todo => todo.IsComplete).Select(todo => new TodoItemDTO(todo)).ToArrayAsync());
    }

    public static async Task<IResult> GetTodo(int id, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);
        return todo is null ? TypedResults.NotFound() : TypedResults.Ok(new TodoItemDTO(todo));
    }

    public static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
    {
        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name
        };
        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();
        todoItemDTO = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    }

    public static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> PatchTodo(int id, TodoPatchDto inputTodo, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        if (inputTodo.Name is not null) todo.Name = inputTodo.Name;
        if (inputTodo.IsComplete is not null) todo.IsComplete = inputTodo.IsComplete.Value;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}