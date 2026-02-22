using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Database {
    // historial de acciones del inventario (ampliacion)
    public class ItemLogRepository {

        // guarda una accion en el log
        public void LogAction(int userId, int itemId, string action, int quantity) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO ItemLog (userId, itemId, action, quantity) VALUES (@uid, @iid, @act, @qty)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@iid"; p2.Value = itemId; cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter(); p3.ParameterName = "@act"; p3.Value = action; cmd.Parameters.Add(p3);
                    var p4 = cmd.CreateParameter(); p4.ParameterName = "@qty"; p4.Value = quantity; cmd.Parameters.Add(p4);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // saca las ultimas N acciones del user
        public List<LogEntry> GetHistory(int userId, int limit = 20) {
            List<LogEntry> history = new List<LogEntry>();

            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT l.id, l.userId, l.itemId, l.action, l.quantity, l.timestamp, i.itemName " +
                                     "FROM ItemLog l " +
                                     "LEFT JOIN Items i ON l.itemId = i.id " +
                                     "WHERE l.userId = @uid " +
                                     "ORDER BY l.timestamp DESC " +
                                     "LIMIT @lim";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@lim"; p2.Value = limit; cmd.Parameters.Add(p2);

                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            history.Add(new LogEntry {
                                id = reader.GetInt32(0),
                                userId = reader.GetInt32(1),
                                itemId = reader.GetInt32(2),
                                action = reader.GetString(3),
                                quantity = reader.GetInt32(4),
                                timestamp = reader.GetString(5),
                                itemName = reader.IsDBNull(6) ? "Desconocido" : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return history;
        }

        // cuantas veces se ha recogido un item
        public int GetTimesCollected(int userId, int itemId) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT COALESCE(SUM(quantity), 0) FROM ItemLog WHERE userId = @uid AND itemId = @iid AND action = 'RECOGER'";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@iid"; p2.Value = itemId; cmd.Parameters.Add(p2);
                    return System.Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // borra todo el historial de un user
        public void ClearHistory(int userId) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "DELETE FROM ItemLog WHERE userId = @uid";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    // modelo para una entrada del historial
    [System.Serializable]
    public class LogEntry {
        public int id;
        public int userId;
        public int itemId;
        public string action;
        public int quantity;
        public string timestamp;
        public string itemName;
    }
}
