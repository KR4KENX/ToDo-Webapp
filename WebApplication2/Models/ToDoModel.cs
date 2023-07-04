using Microsoft.EntityFrameworkCore;
using SQLite;

namespace WebApplication2.Models
{
    public class ToDoModel
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
