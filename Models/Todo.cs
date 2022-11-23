namespace TodoListAPI.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }

    }
}
