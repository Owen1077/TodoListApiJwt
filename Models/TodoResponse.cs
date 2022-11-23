namespace TodoListAPI.Models
{
    public class TodoResponse
    {
        public List<Todo> todoss { get; set; } = new List<Todo>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
