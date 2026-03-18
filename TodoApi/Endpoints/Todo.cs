public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todoitems");

        group.MapGet("/", TodoHandlers.GetAllTodos);
        group.MapGet("/{id}", TodoHandlers.GetTodo);
        group.MapGet("/complete", TodoHandlers.GetCompleteTodos);
        group.MapPost("/", TodoHandlers.CreateTodo);
        group.MapPut("/{id}", TodoHandlers.UpdateTodo);
        group.MapPatch("/{id}", TodoHandlers.PatchTodo);
        group.MapDelete("/{id}", TodoHandlers.DeleteTodo);

        return app;
    }
}