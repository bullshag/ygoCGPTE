using MySql.Data.MySqlClient;
using System.Data;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private const string ConnectionString = "Server=localhost;User ID=root;Password=;Database=cardData;";

    public Form1()
    {
        InitializeComponent();
        Load += (_, _) => LoadCards();
    }

    private void LoadCards()
    {
        using var connection = new MySqlConnection(ConnectionString);
        using var adapter = new MySqlDataAdapter("SELECT name, tcg_legal FROM cards", connection);
        var table = new DataTable();
        adapter.Fill(table);
        dataGridViewCards.DataSource = table;
    }
}
