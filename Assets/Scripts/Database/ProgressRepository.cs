using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Database {
    // guarda y carga el progreso del juego (llaves y pcs hackeados)
    public class ProgressRepository {

        // guarda una llave nueva
        public void SaveKey(int userId, string keyName) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO HackingKeys (userId, keyName) VALUES (@uid, @key)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@key"; p2.Value = keyName; cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // carga todas las llaves del user
        public List<string> LoadKeys(int userId) {
            List<string> keys = new List<string>();
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT keyName FROM HackingKeys WHERE userId = @uid";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            keys.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return keys;
        }

        // borra solo UNA llave con ese nombre
        public void DeleteKey(int userId, string keyName) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "DELETE FROM HackingKeys WHERE id = (SELECT id FROM HackingKeys WHERE userId = @uid AND keyName = @key LIMIT 1)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@key"; p2.Value = keyName; cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // guarda un pc como hackeado
        public void SaveHackedPC(int userId, string pcName) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO HackedPCs (userId, pcName) VALUES (@uid, @pc)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@pc"; p2.Value = pcName; cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // carga la lista de pcs hackeados
        public List<string> LoadHackedPCs(int userId) {
            List<string> pcs = new List<string>();
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT pcName FROM HackedPCs WHERE userId = @uid";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            pcs.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return pcs;
        }

        // mira si un pc ya esta hackeado
        public bool IsPCHacked(int userId, string pcName) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT COUNT(*) FROM HackedPCs WHERE userId = @uid AND pcName = @pc";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@pc"; p2.Value = pcName; cmd.Parameters.Add(p2);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
