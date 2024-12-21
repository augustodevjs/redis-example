using MediatR;
using redis_example.Models;
using redis_example.Events;
using redis_example.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using redis_example.Abstractions.Caching;
using redis_example.Infrastructure.Persistance;

namespace redis_example.Controllers;

[ApiController]
[Route("[controller]")]
public class TodosController : ControllerBase
{
    private readonly IPublisher _publisher;
    private readonly ToDoListDbContext _context;
    private readonly ICacheService _cacheService;

    public TodosController(
        ToDoListDbContext context,
        ICacheService cacheService,
        IPublisher publisher
    )
    {
        _context = context;
        _publisher = publisher;
        _cacheService = cacheService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var todos = await _cacheService
            .GetOrCreateAsync("todos", async () => await _context.ToDos.ToListAsync());
        
        if (todos is null) return NoContent();

        return Ok(todos);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(ToDoInputModel model)
    {
        var todo = new ToDo(0, model.Title, model.Description);

        await _context.ToDos.AddAsync(todo);
        await _context.SaveChangesAsync();
        
        var createdTodoEvent = new CreatedTodoEvent
        {
            Title = todo.Title,
            Description = todo.Description
        };

        await _publisher.Publish(createdTodoEvent);

        return CreatedAtAction(nameof(GetTodos), new { id = todo.Id }, model);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _context.ToDos.FindAsync(id);

        if (todo is null) return NotFound();

        _context.ToDos.Remove(todo);
        await _context.SaveChangesAsync();

        var deleteTodoEvent = new DeleteTodoEvent { Id = id};

        await _publisher.Publish(deleteTodoEvent);

        return NoContent();
    }
}