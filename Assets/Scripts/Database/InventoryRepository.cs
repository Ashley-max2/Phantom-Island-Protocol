using System.Collections.Generic;
using System.Data;
using Models;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Database {
    public class InventoryRepository {
        
        // carga todo el inventario de un user
        public List<InventorySlot> LoadInventory(int userId) {
            List<InventorySlot> inventory = new List<InventorySlot>();
            
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT id, userId, itemId, quantity, slotIndex FROM Inventory WHERE userId = @uid ORDER BY slotIndex ASC";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);

                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            inventory.Add(new InventorySlot {
                                id = reader.GetInt32(0),
                                userId = reader.GetInt32(1),
                                itemId = reader.GetInt32(2),
                                quantity = reader.GetInt32(3),
                                slotIndex = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return inventory;
        }

        // mete un item en un slot, si ya hay algo suma cantidad
        public void AddItem(int userId, int itemId, int quantity, int slotIndex) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                
                using (var checkCmd = conn.CreateCommand()) {
                    checkCmd.CommandText = "SELECT id, quantity FROM Inventory WHERE userId = @uid AND slotIndex = @slot";
                    var p1 = checkCmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; checkCmd.Parameters.Add(p1);
                    var p2 = checkCmd.CreateParameter(); p2.ParameterName = "@slot"; p2.Value = slotIndex; checkCmd.Parameters.Add(p2);

                    int existingId = -1;
                    int existingQty = 0;

                    using (var reader = checkCmd.ExecuteReader()) {
                        if (reader.Read()) {
                            existingId = reader.GetInt32(0);
                            existingQty = reader.GetInt32(1);
                        }
                    }

                    if (existingId != -1) {
                        // ya hay algo, update
                        using (var updateCmd = conn.CreateCommand()) {
                            updateCmd.CommandText = "UPDATE Inventory SET quantity = @qty, itemId = @iid WHERE id = @id";
                            var pQty = updateCmd.CreateParameter(); pQty.ParameterName = "@qty"; pQty.Value = existingQty + quantity; updateCmd.Parameters.Add(pQty);
                            var pIid = updateCmd.CreateParameter(); pIid.ParameterName = "@iid"; pIid.Value = itemId; updateCmd.Parameters.Add(pIid);
                            var pId = updateCmd.CreateParameter(); pId.ParameterName = "@id"; pId.Value = existingId; updateCmd.Parameters.Add(pId);
                            updateCmd.ExecuteNonQuery();
                        }
                    } else {
                        // slot vacio, insert
                        using (var insertCmd = conn.CreateCommand()) {
                            insertCmd.CommandText = "INSERT INTO Inventory (userId, itemId, quantity, slotIndex) VALUES (@uid, @iid, @qty, @slot)";
                            var pUi = insertCmd.CreateParameter(); pUi.ParameterName = "@uid"; pUi.Value = userId; insertCmd.Parameters.Add(pUi);
                            var pIi = insertCmd.CreateParameter(); pIi.ParameterName = "@iid"; pIi.Value = itemId; insertCmd.Parameters.Add(pIi);
                            var pQi = insertCmd.CreateParameter(); pQi.ParameterName = "@qty"; pQi.Value = quantity; insertCmd.Parameters.Add(pQi);
                            var pSi = insertCmd.CreateParameter(); pSi.ParameterName = "@slot"; pSi.Value = slotIndex; insertCmd.Parameters.Add(pSi);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        // vacia un slot entero
        public void ClearSlot(int userId, int slotIndex) {
            using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "DELETE FROM Inventory WHERE userId = @uid AND slotIndex = @slot";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@slot"; p2.Value = slotIndex; cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // quita cantidad de un item
        public void RemoveItem(int userId, int itemId, int quantity) {
             using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                
                using (var checkCmd = conn.CreateCommand()) {
                    checkCmd.CommandText = "SELECT id, quantity FROM Inventory WHERE userId = @uid AND itemId = @iid";
                    var p1 = checkCmd.CreateParameter(); p1.ParameterName = "@uid"; p1.Value = userId; checkCmd.Parameters.Add(p1);
                    var p2 = checkCmd.CreateParameter(); p2.ParameterName = "@iid"; p2.Value = itemId; checkCmd.Parameters.Add(p2);

                    int existingId = -1;
                    int existingQty = 0;

                    using (var reader = checkCmd.ExecuteReader()) {
                        if (reader.Read()) {
                            existingId = reader.GetInt32(0);
                            existingQty = reader.GetInt32(1);
                        }
                    }

                    if (existingId != -1) {
                        int newQty = existingQty - quantity;
                        if (newQty <= 0) {
                            using (var delCmd = conn.CreateCommand()) {
                                delCmd.CommandText = "DELETE FROM Inventory WHERE id = @id";
                                var pId = delCmd.CreateParameter(); pId.ParameterName = "@id"; pId.Value = existingId; delCmd.Parameters.Add(pId);
                                delCmd.ExecuteNonQuery();
                            }
                        } else {
                            using (var updateCmd = conn.CreateCommand()) {
                                updateCmd.CommandText = "UPDATE Inventory SET quantity = @qty WHERE id = @id";
                                var pQty = updateCmd.CreateParameter(); pQty.ParameterName = "@qty"; pQty.Value = newQty; updateCmd.Parameters.Add(pQty);
                                var pId = updateCmd.CreateParameter(); pId.ParameterName = "@id"; pId.Value = existingId; updateCmd.Parameters.Add(pId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
             }
        }
    
        // saca la info de un item por id
        public Item GetItemDefinition(int itemId) {
             using (var conn = DatabaseManager.Instance.GetConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                     cmd.CommandText = "SELECT id, itemName, description, maxStack, iconPath FROM Items WHERE id = @id";
                     var p1 = cmd.CreateParameter(); p1.ParameterName = "@id"; p1.Value = itemId; cmd.Parameters.Add(p1);
                     using (var reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            return new Item {
                                id = reader.GetInt32(0),
                                itemName = reader.GetString(1),
                                description = reader.GetString(2),
                                maxStack = reader.GetInt32(3),
                                iconPath = reader.GetString(4)
                            };
                        }
                     }
                }
             }
             return null;
        }
    }
}
