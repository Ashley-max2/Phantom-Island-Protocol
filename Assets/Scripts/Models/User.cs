using System;
using SQLite;

namespace Models {
    [Table("Users")]
    public class User {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        
        [Unique]
        public string username { get; set; }
        
        public string password { get; set; }
        
        public bool isFake { get; set; }

        public User() {}

        public User(string user, string pass, bool fake = false) {
            username = user;
            password = pass;
            isFake = fake;
        }
    }
}
