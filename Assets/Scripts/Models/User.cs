using System;

namespace Models {
    [Serializable]
    public class User {
        public int id;
        public string username;
        public string password;
        public bool isFake;

        public User() {}

        public User(string user, string pass, bool fake = false) {
            username = user;
            password = pass;
            isFake = fake;
        }
    }
}
