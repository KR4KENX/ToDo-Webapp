using Microsoft.EntityFrameworkCore;
using SQLite;

namespace WebApplication2.Models
{
    public class UserModel
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public byte[]? Salt { get; set; }
    }
}