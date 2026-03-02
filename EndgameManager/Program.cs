using System;
using Microsoft.Data.Sqlite;

namespace EndgameManager
{
    class Program
    {
        // Connection string heroppe så alle funktioner kan bruge den
        static string connectionString = "Data Source=Endgame.db;Mode=ReadWriteCreate;";

        static void Main(string[] args)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    bool koerProgram = true;

                    // Her starter vores Menu-loop
                    while (koerProgram)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("=== ENDGAME DATABASE MANAGER ===");
                        Console.ResetColor();
                        Console.WriteLine("1. Opret ny karakter (CREATE)");
                        Console.WriteLine("2. Vis alle karakterer (READ)");
                        Console.WriteLine("3. Gennemfør et Map og få XP (UPDATE)");
                        Console.WriteLine("4. Slet en karakter (DELETE)");
                        Console.WriteLine("5. Afslut program");
                        Console.Write("\nVælg en handling (1-5): ");

                        string valg = Console.ReadLine();

                        Console.WriteLine(); 

                        switch (valg)
                        {
                            case "1":
                                CreateCharacter(connection);
                                break;
                            case "2":
                                ReadCharacters(connection);
                                break;
                            case "3":
                                UpdateXP(connection);
                                break;
                            case "4":
                                DeleteCharacter(connection);
                                break;
                            case "5":
                                koerProgram = false;
                                break;
                            default:
                                Console.WriteLine("Ugyldigt valg, prøv igen.");
                                break;
                        }

                        if (koerProgram)
                        {
                            Console.WriteLine("\nTryk på en vilkårlig tast for at vende tilbage til menuen...");
                            Console.ReadKey();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Der skete en database-fejl: " + ex.Message);
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        }

        // CRUD FUNKTIONER HERUNDER 

        // 1. CREATE
        static void CreateCharacter(SqliteConnection connection)
        {
            Console.Write("Indtast karakterens navn: ");
            string navn = Console.ReadLine();

            Console.Write("Indtast karakterens Class (Warrior eller Mage): ");
            string charClass = Console.ReadLine();

            // Vi bruger parametre
            string sql = "INSERT INTO Character (Name, Class, CurrentXP) VALUES (@Navn, @Class, 0)";

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Navn", navn);
                command.Parameters.AddWithValue("@Class", charClass);

                int rowsAffected = command.ExecuteNonQuery();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{rowsAffected} karakter(er) blev oprettet med succes!");
                Console.ResetColor();
            }
        }

        // 2. READ
        static void ReadCharacters(SqliteConnection connection)
        {
            string sql = "SELECT CharacterID, Name, Class, CurrentXP FROM Character";

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("ID\tNavn\t\tClass\t\tXP");
                    Console.WriteLine("--------------------------------------------------");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["CharacterID"]}\t{reader["Name"]}\t\t{reader["Class"]}\t\t{reader["CurrentXP"]}");
                    }
                }
            }
        }

        // 3. UPDATE
        static void UpdateXP(SqliteConnection connection)
        {
            ReadCharacters(connection); // Vis listen først, så man kan se ID'erne

            Console.Write("\nIndtast ID på den karakter, der lige har gennemført et Map: ");
            string idInput = Console.ReadLine();

            int xpReward = 50000; // Vi lader som om et Map giver 50.000 XP

            string sql = "UPDATE Character SET CurrentXP = CurrentXP + @Reward WHERE CharacterID = @ID";

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Reward", xpReward);
                command.Parameters.AddWithValue("@ID", idInput);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Karakteren gennemførte sit Map og fik {xpReward} XP!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Fandt ingen karakter med det ID.");
                }
            }
        }

        // 4. DELETE
        static void DeleteCharacter(SqliteConnection connection)
        {
            ReadCharacters(connection);

            Console.Write("\nIndtast ID på den karakter der skal slettes: ");
            string idInput = Console.ReadLine();

            string sql = "DELETE FROM Character WHERE CharacterID = @ID";

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ID", idInput);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Karakteren er blevet permanent slettet!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Fandt ingen karakter med det ID.");
                }
            }
        }
    }
}
