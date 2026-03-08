using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using SQLite;

namespace Database {
    public class DatabaseManager : MonoBehaviour {
        public static DatabaseManager Instance;
        private string dbName = "URI=file:PhantomIsland.sqlite";
        private string ormDbPath;

        void Awake() {
            ormDbPath = Path.Combine(Application.dataPath, "../../PhantomIsland.sqlite");
            // normalize path for different OS if needed, but here we are on windows
            ormDbPath = Path.GetFullPath(ormDbPath);

            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CreateTables();
                InitializeORM();
            } else {
                Destroy(gameObject);
            }
        }

        private void InitializeORM() {
            using (var db = GetORMConnection()) {
                db.CreateTable<Models.User>();
                Debug.Log("ORM Tables initialized.");
            }
        }

        private void CreateTables() {
            using (var connection = new SqliteConnection(dbName)) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    // tabla usuarios
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Users (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "username VARCHAR(20) UNIQUE, " + 
                                          "password VARCHAR(20), " +
                                          "isFake INTEGER DEFAULT 0);";
                    command.ExecuteNonQuery();

                    // tabla items
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Items (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "itemName VARCHAR(50), " + 
                                          "description TEXT," +
                                          "maxStack INTEGER DEFAULT 99," +
                                          "iconPath VARCHAR(100));";
                    command.ExecuteNonQuery();

                    // inventario con 3 slots
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Inventory (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "userId INTEGER, " + 
                                          "itemId INTEGER, " +
                                          "quantity INTEGER," +
                                          "slotIndex INTEGER DEFAULT 0," +
                                          "FOREIGN KEY(userId) REFERENCES Users(id)," +
                                          "FOREIGN KEY(itemId) REFERENCES Items(id));";
                    command.ExecuteNonQuery();

                    // historial de acciones (ampliacion)
                    command.CommandText = "CREATE TABLE IF NOT EXISTS ItemLog (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "userId INTEGER, " +
                                          "itemId INTEGER, " +
                                          "action VARCHAR(20), " +
                                          "quantity INTEGER, " +
                                          "timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, " +
                                          "FOREIGN KEY(userId) REFERENCES Users(id), " +
                                          "FOREIGN KEY(itemId) REFERENCES Items(id));";
                    command.ExecuteNonQuery();

                    // llaves de logins falsos
                    command.CommandText = "CREATE TABLE IF NOT EXISTS HackingKeys (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "userId INTEGER, " +
                                          "keyName VARCHAR(50), " +
                                          "FOREIGN KEY(userId) REFERENCES Users(id));";
                    command.ExecuteNonQuery();

                    // pcs hackeados
                    command.CommandText = "CREATE TABLE IF NOT EXISTS HackedPCs (" +
                                          "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                          "userId INTEGER, " +
                                          "pcName VARCHAR(50), " +
                                          "FOREIGN KEY(userId) REFERENCES Users(id));";
                    command.ExecuteNonQuery();

                    // items por defecto
                    command.CommandText = "INSERT OR IGNORE INTO Items (id, itemName, description, maxStack, iconPath) VALUES " +
                                          "(1, 'Laptop', 'Dispositivo para hackear terminales', 1, 'Icons/Laptop')," +
                                          "(2, 'Fragmento', 'Fragmento de datos corruptos', 3, 'Icons/Fragment')," +
                                          "(3, 'Fragmento de Hielo', 'Fragmento congelado de datos', 3, 'Icons/IceFragment')," +
                                          "(4, 'Llave', 'Llave de acceso generada por login falso', 1, 'Icons/Key')";
                    command.ExecuteNonQuery();
                    
                    // usuario inicial
                    command.CommandText = "INSERT OR IGNORE INTO Users (username, password, isFake) VALUES ('hacker', 'password123', 0)";
                    command.ExecuteNonQuery();

                    // migracion por si la tabla es vieja
                    try {
                        command.CommandText = "ALTER TABLE Inventory ADD COLUMN slotIndex INTEGER DEFAULT 0;";
                        command.ExecuteNonQuery();
                    } catch (SqliteException) {
                        // ya existe la columna
                    }
                }
            }
            Debug.Log("Database initialized: " + dbName);
        }

        public IDbConnection GetConnection() {
            return new SqliteConnection(dbName);
        }

        public SQLiteConnection GetORMConnection() {
            return new SQLiteConnection(ormDbPath);
        }
    }
}

