using System.Collections.Generic;
using System.Linq;
using Models;
using SQLite;
using UnityEngine;

namespace Database {
    public class UserRepository {
        
        public User Login(string username, string password) {
            using (var db = DatabaseManager.Instance.GetORMConnection()) {
                return db.Table<User>()
                         .Where(u => u.username == username && u.password == password)
                         .FirstOrDefault();
            }
        }

        public List<User> GetUsers() {
            using (var db = DatabaseManager.Instance.GetORMConnection()) {
                return db.Table<User>().ToList();
            }
        }

        public void DeleteAllUsers() {
            using (var db = DatabaseManager.Instance.GetORMConnection()) {
                db.DeleteAll<User>();
                Debug.Log("All users deleted from database.");
            }
        }


        public bool CreateFakeUser(string username, string password) {
            using (var db = DatabaseManager.Instance.GetORMConnection()) {
                var existing = db.Table<User>().Where(u => u.username == username).FirstOrDefault();
                if (existing != null) return false;

                var newUser = new User(username, password, true);
                db.Insert(newUser);
                return true;
            }
        }
    }
}

