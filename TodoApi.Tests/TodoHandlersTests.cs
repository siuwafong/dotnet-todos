using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace TodoApi.Tests;

public class TodoHandlersTests
{
    private TodoDb GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TodoDb(options);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsEmptyArray_WhenNoTodosExist()
    {
        // Arrange
        var db = GetInMemoryDbContext();

        // Act
        var result = await TodoHandlers.GetAllTodos(db);

        // Assert
        var okResult = result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<TodoItemDTO[]>>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllTodos_ReturnsAllTodos_WhenTodosExist()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        db.Todos.AddRange(
            new Todo { Name = "Task 1", IsComplete = false },
            new Todo { Name = "Task 2", IsComplete = true }
        );
        await db.SaveChangesAsync();

        // Act
        var result = await TodoHandlers.GetAllTodos(db);

        // Assert
        var okResult = result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<TodoItemDTO[]>>().Subject;
        okResult.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTodo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var db = GetInMemoryDbContext();

        // Act
        var result = await TodoHandlers.GetTodo(999, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>();
    }

    [Fact]
    public async Task GetTodo_ReturnsTodo_WhenTodoExists()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var todo = new Todo { Name = "Test Task", IsComplete = false };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        // Act
        var result = await TodoHandlers.GetTodo(todo.Id, db);

        // Assert
        var okResult = result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<TodoItemDTO>>().Subject;
        okResult.Value.Name.Should().Be("Test Task");
    }

    [Fact]
    public async Task CreateTodo_AddsNewTodo_AndReturnsCreated()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var dto = new TodoItemDTO { Name = "New Task", IsComplete = false };

        // Act
        var result = await TodoHandlers.CreateTodo(dto, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Created<TodoItemDTO>>();
        db.Todos.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateTodo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var dto = new TodoItemDTO { Name = "Updated", IsComplete = true };

        // Act
        var result = await TodoHandlers.UpdateTodo(999, dto, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>();
    }

    [Fact]
    public async Task UpdateTodo_UpdatesTodo_WhenTodoExists()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var todo = new Todo { Name = "Original", IsComplete = false };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        var dto = new TodoItemDTO { Name = "Updated", IsComplete = true };

        // Act
        var result = await TodoHandlers.UpdateTodo(todo.Id, dto, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        var updatedTodo = await db.Todos.FindAsync(todo.Id);
        updatedTodo.Name.Should().Be("Updated");
        updatedTodo.IsComplete.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTodo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var db = GetInMemoryDbContext();

        // Act
        var result = await TodoHandlers.DeleteTodo(999, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>();
    }

    [Fact]
    public async Task DeleteTodo_RemovesTodo_WhenTodoExists()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var todo = new Todo { Name = "To Delete", IsComplete = false };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        // Act
        var result = await TodoHandlers.DeleteTodo(todo.Id, db);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        db.Todos.Should().BeEmpty();
    }
}