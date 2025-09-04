using System;
using System.Text.Json;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string connectionString = builder.Configuration.GetConnectionString("Default") ??
    "Server=localhost;Database=accounts;User ID=userclient;Password=123321;";
string RootPath() => Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
string ReadSql(string file) => File.ReadAllText(Path.Combine(RootPath(), file));

app.MapPost("/api/login", async (LoginRequest req) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    await using var cmd = new MySqlCommand(ReadSql("rest_login.sql"), conn);
    cmd.Parameters.AddWithValue("@username", req.Username);
    cmd.Parameters.AddWithValue("@passwordHash", req.PasswordHash);
    await using var r = await cmd.ExecuteReaderAsync();
    if (await r.ReadAsync())
    {
        int id = Convert.ToInt32(r["id"]);
        string nickname = r["nickname"].ToString() ?? string.Empty;
        await r.CloseAsync();
        await using var seen = new MySqlCommand(ReadSql("rest_update_last_seen.sql"), conn);
        seen.Parameters.AddWithValue("@id", id);
        await seen.ExecuteNonQueryAsync();
        return Results.Json(new { success = true, userId = id, nickname });
    }
    return Results.Json(new { success = false });
});

app.MapPost("/api/register", async (RegisterRequest req) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    await using (var checkUser = new MySqlCommand(ReadSql("rest_check_username.sql"), conn))
    {
        checkUser.Parameters.AddWithValue("@username", req.Username);
        long exists = (long)(await checkUser.ExecuteScalarAsync() ?? 0);
        if (exists > 0) return Results.Json(new { success = false, message = "Username exists" });
    }
    await using (var checkNick = new MySqlCommand(ReadSql("rest_check_nickname.sql"), conn))
    {
        checkNick.Parameters.AddWithValue("@nickname", req.Nickname);
        long existsNick = (long)(await checkNick.ExecuteScalarAsync() ?? 0);
        if (existsNick > 0) return Results.Json(new { success = false, message = "Nickname exists" });
    }
    await using (var insert = new MySqlCommand(ReadSql("rest_register.sql"), conn))
    {
        insert.Parameters.AddWithValue("@username", req.Username);
        insert.Parameters.AddWithValue("@nickname", req.Nickname);
        insert.Parameters.AddWithValue("@passwordHash", req.PasswordHash);
        await insert.ExecuteNonQueryAsync();
        long newId = insert.LastInsertedId;
        await using (var ensureNode = new MySqlCommand(ReadSql("rest_ensure_node.sql"), conn))
        {
            ensureNode.Parameters.AddWithValue("@node", "nodeRiverVillage");
            ensureNode.Parameters.AddWithValue("@name", "River Village");
            await ensureNode.ExecuteNonQueryAsync();
        }
        string travelSql = File.ReadAllText(Path.Combine(RootPath(), "init_travel_state.sql"));
        await using var travel = new MySqlCommand(travelSql, conn);
        travel.Parameters.AddWithValue("@a", newId);
        travel.Parameters.AddWithValue("@node", "nodeRiverVillage");
        await travel.ExecuteNonQueryAsync();
    }
    return Results.Json(new { success = true });
});

app.MapGet("/api/inventory/{userId:int}", async (int userId) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    var items = new List<object>();
        await using (var cmd = new MySqlCommand(ReadSql("rest_inventory_load_items.sql"), conn))
        {
            cmd.Parameters.AddWithValue("@id", userId);
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                items.Add(new { item_name = r["item_name"].ToString(), quantity = Convert.ToInt32(r["quantity"]) });
        }
    var equipment = new List<object>();
        await using (var cmd = new MySqlCommand(ReadSql("rest_inventory_load_equipment.sql"), conn))
        {
            cmd.Parameters.AddWithValue("@id", userId);
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                equipment.Add(new
                {
                    character_name = r["character_name"].ToString(),
                    slot = r["slot"].ToString(),
                    item_name = r["item_name"].ToString()
                });
        }
    return Results.Json(new { items, equipment });
});

app.MapPost("/api/inventory/add", async (InventoryUpdate req) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    await using var cmd = new MySqlCommand(ReadSql("rest_inventory_add_item.sql"), conn);
    cmd.Parameters.AddWithValue("@id", req.UserId);
    cmd.Parameters.AddWithValue("@name", req.ItemName);
    cmd.Parameters.AddWithValue("@qty", req.Quantity);
    await cmd.ExecuteNonQueryAsync();
    return Results.Ok();
});

app.MapPost("/api/inventory/remove", async (InventoryUpdate req) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    string sql = ReadSql("rest_inventory_remove_item.sql");
    await using var cmd = new MySqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@id", req.UserId);
    cmd.Parameters.AddWithValue("@name", req.ItemName);
    cmd.Parameters.AddWithValue("@qty", req.Quantity);
    await cmd.ExecuteNonQueryAsync();
    return Results.Ok();
});

app.MapPost("/api/inventory/equip", async (EquipRequest req) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    if (string.IsNullOrEmpty(req.ItemName))
    {
        await using var cmd = new MySqlCommand(ReadSql("rest_inventory_remove_equipment.sql"), conn);
        cmd.Parameters.AddWithValue("@id", req.UserId);
        cmd.Parameters.AddWithValue("@character", req.CharacterName);
        cmd.Parameters.AddWithValue("@slot", req.Slot);
        await cmd.ExecuteNonQueryAsync();
    }
    else
    {
        await using var cmd = new MySqlCommand(ReadSql("rest_inventory_save_equipment.sql"), conn);
        cmd.Parameters.AddWithValue("@id", req.UserId);
        cmd.Parameters.AddWithValue("@character", req.CharacterName);
        cmd.Parameters.AddWithValue("@slot", req.Slot);
        cmd.Parameters.AddWithValue("@name", req.ItemName);
        await cmd.ExecuteNonQueryAsync();
    }
    return Results.Ok();
});

app.MapGet("/api/ability/{name}", async (string name) =>
{
    await using var conn = new MySqlConnection(connectionString);
    await conn.OpenAsync();
    await using var cmd = new MySqlCommand(ReadSql("rest_get_ability_by_name.sql"), conn);
    cmd.Parameters.AddWithValue("@name", name);
    await using var r = await cmd.ExecuteReaderAsync();
    if (await r.ReadAsync())
    {
        return Results.Json(new { id = Convert.ToInt32(r["id"]), description = r["description"].ToString() });
    }
    return Results.NotFound();
});

app.Run();

record LoginRequest(string Username, string PasswordHash);
record RegisterRequest(string Username, string Nickname, string PasswordHash);
record InventoryUpdate(int UserId, string ItemName, int Quantity);
record EquipRequest(int UserId, string CharacterName, string Slot, string? ItemName);
