using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListAPI.Data;
using TodoListAPI.Models;

namespace TodoListAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoAPIDbContext dbContext;

        public TodoController(TodoAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            return Ok( await dbContext.Todos.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetTodo([FromRoute] Guid id)
        {
            var todo = await dbContext.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> AddTodo(AddTodoRequest addTodoRequest)
        {
            var todo = new Todo()
            {
                Id = Guid.NewGuid(),
                Name = addTodoRequest.Name,
                Created = addTodoRequest.Created
                //ASK KACHI ABOUT THIS DATETIME
               
            };

            await dbContext.Todos.AddAsync(todo);
            await dbContext.SaveChangesAsync();

            return Ok(todo);

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTodo([FromRoute] Guid id, UpdateTodoRequest updateTodoRequest)
        {
            var todo = await dbContext.Todos.FindAsync(id);

            if (todo != null)
            {
                todo.Name = updateTodoRequest.Name;
                todo.Created = updateTodoRequest.Created;

                await dbContext.SaveChangesAsync();

                return Ok(todo);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTodo([FromRoute] Guid id)
        {
            var todo = await dbContext.Todos.FindAsync(id);

            if (todo != null)
            {
                dbContext.Remove(todo);
                await dbContext.SaveChangesAsync();
                return Ok(todo);
            }

            return NotFound();
        }

        [HttpGet("{page}")]
        public async Task<ActionResult<List<Todo>>> GetTodoPages(int page)
        {
            if (dbContext.Todos == null)
                return NotFound();
            var pageResults = 3f;
            var pageCount = Math.Ceiling(dbContext.Todos.Count() / pageResults);
            var todos = await dbContext.Todos
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();
            var response = new TodoResponse
            {
                todoss = todos,
                CurrentPage = page,
                Pages = (int)pageCount
            };
            return Ok(response);
        }

    }
}
