using System.Data;
using Models;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Database {
    public class UserRepository {
        // login normal, busca user y pass en la bd
        public User Login(string username, string password) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT id, username, password, isFake FROM Users WHERE username = @u AND password = @p";
                    
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@u"; p1.Value = username; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@p"; p2.Value = password; cmd.Parameters.Add(p2);

                    using (var reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                           return new User {
                                id = reader.GetInt32(0),
                                username = reader.GetString(1),
                                password = reader.GetString(2),
                                isFake = reader.GetInt32(3) == 1
                           };
                        }
                    }
                }
            }
            return null;
        }

        // crea un user falso para generar llaves
        public bool CreateFakeUser(string username, string password) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO Users (username, password, isFake) VALUES (@u, @p, 1)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@u"; p1.Value = username; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@p"; p2.Value = password; cmd.Parameters.Add(p2);

                    try {
                        cmd.ExecuteNonQuery();
                        return true;
                    } catch {
                        return false;
                    }
                }
            }
        }
    }
}
