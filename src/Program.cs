using Npgsql;
using System.Text.RegularExpressions;

//Database connection data
const string Adress = "localhost";
const string DBName = "banking";
//Superuser login data
const string Username = "postgres";
const string Password = "123";
const string AdminCS = $"Host={Adress};Username={Username};Password={Password};Database={DBName}";

static bool IsConnectionEstablished(string cs)
{
    using (NpgsqlConnection conn = new NpgsqlConnection(cs))
    {
        try
        {
            conn.Open();
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
    }
}
bool IsUsernameTaken(string username)
{
    var cs = AdminCS;
    var sql = $"SELECT if_username_taken(\'{username}\');";
    using var cnt = new NpgsqlConnection(cs);
    cnt.Open();
    var cmd = new NpgsqlCommand(sql, cnt);
    if (cmd.ExecuteScalar().ToString() == "True")
    {
        cnt.Close();
        return true;
    }
    else
    {
        cnt.Close();
        return false;
    }
}

bool DoesClientExist(int id)
{
    var cs = AdminCS;
    var sql = $"SELECT if_id_exists(\'{id}\');";
    using var cnt = new NpgsqlConnection(cs);
    cnt.Open();
    var cmd = new NpgsqlCommand(sql, cnt);
    if (cmd.ExecuteScalar().ToString() == "True")
    {
        cnt.Close();
        return true;
    }
    else
    {
        cnt.Close();
        return false;
    }
}

int CheckIntFormat(string number)
{
    if (!int.TryParse(number, out int value))
    {
        Console.Write("Invalid value, try again: ");
        return CheckIntFormat(Console.ReadLine());
    }
    else
    {
        return int.Parse(number);
    }
}

float CheckFloatFormat(string number)
{
    if (!float.TryParse(number, out float value))
    {
        Console.Write("Invalid value, try again: ");
        return CheckFloatFormat(Console.ReadLine());
    }
    else
    {
        return float.Parse(number);
    }
}

decimal CheckDecimalFormat(string number)
{
    if (!decimal.TryParse(number, out decimal value))
    {
        Console.Write("Invalid value, try again: ");
        return CheckDecimalFormat(Console.ReadLine());
    }
    else
    {
        decimal dec = decimal.Parse(number);
        byte decimals = (byte)((Decimal.GetBits(dec)[2] >> 16) & 0x7F);
        if(dec == 2)
        {
            return dec;
        }    
        else
        {
            Console.Write("Invalid value, try again: ");
            return CheckDecimalFormat(Console.ReadLine());
        }
    }
}


char CheckCharFormat(string character)
{
    if (!char.TryParse(character, out char value))
    {
        Console.Write("Invalid value, try again: ");
        return CheckCharFormat(Console.ReadLine());
    }
    else
    {
        return char.Parse(character);
    }
}

string CheckIfStringNotEmpty(string str)
{
    if (string.IsNullOrEmpty(str))
    {
        Console.Write("Invalid value, try again: ");
        return CheckIfStringNotEmpty(Console.ReadLine());
    }
    else
    {
        return str;
    }
}

char CheckGenderFormat(char gender)
{
    if (gender != 'm' && gender != 'M' && gender != 'f' && gender != 'F')
    {
        Console.Write("Invalid value, try again: ");
        gender = CheckCharFormat(Console.ReadLine());
        return CheckGenderFormat(gender);
    }
    else if (gender == 'm' || gender == 'M')
    {
        return 'M';
    }
    else if (gender == 'f' || gender == 'F')
    {
        return 'F';
    }
    else return '0';
}

void displayMenu()
{
    while(true)
    {
        char ch;
        Console.WriteLine("1. Create an account\n" +
            "2. Log into an existing account\n" +
            "3. Quit");
        ch = Console.ReadKey().KeyChar;
        if (ch == '1')
        {
            Console.Clear();
            CreateAccount();
        }
        else if (ch == '2')
        {
            Console.Clear();
            LogIntoAccount();
        }
        else if (ch == '3')
        {
            break;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Invalid key, try again.\n");
        }
    }
}

void CreateAccount()
{
    var connectionString = AdminCS;
    if (IsConnectionEstablished(connectionString) == true)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT version()";
        using var command = new NpgsqlCommand(sql, connection);
        var version = command.ExecuteScalar().ToString();
        Console.WriteLine($"Connected to the database\n{version}\n");
        string firstName, lastName, username, password;
        char gender;
        int age;
        Console.Write("\nEnter your first name: ");
        firstName = CheckIfStringNotEmpty(Console.ReadLine());
        Console.Write("Enter your last name: ");
        lastName = CheckIfStringNotEmpty(Console.ReadLine());
        Console.Write("Enter your gender (M/F): ");
        gender = CheckCharFormat(Console.ReadLine());
        gender = CheckGenderFormat(gender);
        Console.Write("Enter your age: ");
        age = CheckIntFormat(Console.ReadLine());
        Console.Write("Choose your username: ");
        username = CheckIfStringNotEmpty(Console.ReadLine());
        while (IsUsernameTaken(username) == true)
        {
            Console.Write("Username already taken, try again: ");
            username = CheckIfStringNotEmpty(Console.ReadLine());
        }
        Console.Write("Choose your password: ");
        password = CheckIfStringNotEmpty(Console.ReadLine());
        sql = $"INSERT INTO \"client\" (first_name, last_name, gender, age, username) VALUES (\'{firstName}\', \'{lastName}\', \'{gender}\', \'{age}\', \'{username}\'); " +
            $"CREATE USER {username} WITH PASSWORD \'{password}\';" +
            $"GRANT SELECT ON client to {username};" +
            $"GRANT SELECT ON transfer to {username};";
        command.CommandText = sql;
        command.ExecuteNonQuery();
        Console.WriteLine("Account successfully created. Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
    else
    {
        Console.WriteLine("Couldn't connect to the database");
    }
}

void LogIntoAccount()
{
    while (true)
    {
        char chh = ' ';
        string username, password;
        Console.Write("Enter username: ");
        username = CheckIfStringNotEmpty(Console.ReadLine());
        Console.Write("Enter password: ");
        password = CheckIfStringNotEmpty(Console.ReadLine());
        var connectionString = $"Host=localhost;Username={username};Password={password};Database=banking";
        Console.Clear();
        if (IsConnectionEstablished(connectionString) == true && chh != '5')
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var sql = "SELECT version()";
            using var command = new NpgsqlCommand(sql, connection);
            var version = command.ExecuteScalar().ToString();
            
            sql = $"SELECT first_name FROM client where username=\'{username}\';";
            command.CommandText = sql;
            var firstName = command.ExecuteScalar().ToString();
            sql = $"SELECT last_name FROM client where username=\'{username}\';";
            command.CommandText = sql;
            var lastName = command.ExecuteScalar().ToString();
            sql = $"SELECT client_id FROM client where username=\'{username}\';";
            command.CommandText = sql;
            int clientId = int.Parse(command.ExecuteScalar().ToString());
            while (true)
            {
                sql = $"SELECT balance FROM client where username=\'{username}\';";
                command.CommandText = sql;
                var balance = command.ExecuteScalar().ToString();
                Console.WriteLine($"Connected to the database\n{version}\n");
                Console.WriteLine($"Welcome, {firstName} {lastName}!");
                Console.WriteLine($"Your Client ID is: {clientId}");
                Console.WriteLine($"Your current balance is: {balance}\n");
                Console.WriteLine("1. Deposit money\n" +
                    "2. Withdraw money\n" +
                    "3. Make a transfer\n" +
                    "4. Show transfer history\n" +
                    "5. Log out\n");

                chh = Console.ReadKey().KeyChar;
                if (chh == '1')
                {
                    Console.Write("\nEnter the amount you would like to deposit: ");
                    string amountDeposited = Console.ReadLine();
                    while (!Regex.IsMatch(amountDeposited, @"^[0-9]{1,10}\,[0-9]{2}$"))
                    {
                        Console.Write("Invalid value, try again: ");
                        amountDeposited = Console.ReadLine();
                    }
                    sql = $"UPDATE client SET balance = balance + {amountDeposited.Replace(",",".")} WHERE username=\'{username}\';";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                    Console.Clear();
                    Console.WriteLine($"{amountDeposited} USD have been successfully deposited");

                }
                else if (chh == '2')
                {
                    Console.Write("\nEnter the amount you would like to withdraw: ");
                    string amountWithdrawn = Console.ReadLine();
                    decimal amountWithdrawnDec = Decimal.Parse(amountWithdrawn.Replace(".", ","));
                    decimal balanceDec = Decimal.Parse(balance);
                    while (!Regex.IsMatch(amountWithdrawn, @"^[0-9]{1,10}\,[0-9]{2}$") || amountWithdrawnDec > balanceDec)
                    {
                        Console.Write("Invalid value, try again: ");
                        amountWithdrawn = Console.ReadLine();
                        amountWithdrawnDec = Decimal.Parse(amountWithdrawn.Replace(".", ","));
                    }
                    sql = $"UPDATE client SET balance = balance - {amountWithdrawn.Replace(",", ".")} WHERE username=\'{username}\';";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                    Console.Clear();
                    Console.WriteLine($"{amountWithdrawn} USD have been successfully withdrawn");
                }
                else if (chh == '3')
                {
                    int recipientId;
                    while (true)
                    {
                        Console.Write("\nEnter the recipient\'s Client ID: ");
                        recipientId = CheckIntFormat(Console.ReadLine());
                        if (DoesClientExist(recipientId) == true)
                        {
                            Console.Write("Please enter the amount you would like to transfer: ");
                            string amountTransferred = Console.ReadLine();
                            decimal amountTransferredDec = Decimal.Parse(amountTransferred.Replace(".", ","));
                            decimal balanceDec = Decimal.Parse(balance);
                            while (!Regex.IsMatch(amountTransferred, @"^[0-9]{1,10}\,[0-9]{2}$") || amountTransferredDec > balanceDec)
                            {
                                Console.Write("Invalid value, try again: ");
                                amountTransferred= Console.ReadLine();
                                amountTransferredDec = Decimal.Parse(amountTransferred.Replace(".", ","));
                            }
                            BankingConsole.Transfer transfer = new BankingConsole.Transfer(clientId, recipientId, amountTransferred);
                            transfer.TransferMoney(AdminCS);
                            Console.Clear();
                            Console.WriteLine($"Successfuly transferred {amountTransferred} to client with ID {recipientId}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Client does not exist, please try again.");
                        }
                    }
                }
                else if (chh == '4')
                {
                    Console.Clear();
                    sql = "SELECT * from transfer;";
                    command.CommandText = sql;
                    NpgsqlDataReader transferHistory = command.ExecuteReader();
                    Console.WriteLine("Your transfer history\n");
                    Console.Write(String.Format("|{0,11}|{1,9}|{2,12}|{3,8}|{4,20}|\n", "Transfer ID", "Sender ID", "Recipient ID", "Amount", "Time"));
                    while (transferHistory.Read())
                    {
                        List<string> rowData = new List<string>();
                        for(int i = 0; i < transferHistory.FieldCount; i++)
                        {
                            rowData.Add(transferHistory.GetValue(i).ToString());
                        }
                        Console.Write(String.Format("|{0,11}|{1,9}|{2,12}|{3,8}|{4,20}|", rowData[0], rowData[1], rowData[2], rowData[3], rowData[4]));
                        Console.WriteLine();
                    }
                    transferHistory.Close();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else if (chh == '5')
                {
                    connection.Close();
                    Console.Clear();
                    Console.WriteLine("Logged out successfully");
                    break;
                }
            }
        }
        else if (chh != '5')
        {
            Console.Clear();
            Console.WriteLine("Couldn't connect to the database, please check your credentials");
        }
        if (chh == '5')
        {
            break;
        }
    }
}

displayMenu();