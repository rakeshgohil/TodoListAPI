using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListAPI.Data;
using TodoListAPI.Models;

namespace TodoListAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        public readonly TodoContext _context;

        public TodosController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            TodoItem todoItem = await _context.TodoItems.FindAsync(id);
            if(todoItem == null) 
            {
                return NotFound();            
            }
            return todoItem;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> PutTodoItem(int id, TodoItem todoItem)
        {
            try
            {
                if (id != todoItem.Id)
                {
                    return BadRequest();
                }
                _context.Entry(todoItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(int id)
        {
            try
            {
                TodoItem todoItem = await _context.TodoItems.FindAsync(id);
                if (todoItem == null)
                {
                    return NotFound();
                }
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
    }
}
