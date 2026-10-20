using library.Models;
using Npgsql;

namespace library.Data;

public class MemberRepository
{

    private readonly NpgsqlDataSource _dataSource;

    public MemberRepository(NpgsqlDataSource dataSource)
    {

        _dataSource = dataSource;
    }

    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            member_id = reader.GetInt64(0),
            full_name = reader.GetString(1),
            email = reader.IsDBNull(2) ? null : reader.GetString(2), // NULL becomes null
            join_date = reader.GetDateTime(0),
            member_type = reader.GetString(1)
        };
    }
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT member_id, full_name, email, join_date, member_type FROM lending.member ORDER BY member_id;";
        var books = new List<Member>();
        // The command borrows a connection; 'await using' gives it back when the method ends.
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) // once for each row
        {
            books.Add(ReadMember(reader));
        }
        return books;
    }
    public async Task AddAsync(Book book)
    {
        const string sql = "INSERT INTO lending.book (title, category, price) " +
                           "VALUES (@title, @category, @price);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", book.Title);
        // A C# null must be sent as DBNull.Value, which is how a database NULL is written.
        command.Parameters.AddWithValue("category", (object?)book.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("price", (object?)book.Price ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();           // runs SQL that returns no rows
    }
    public async Task<Book?> GetByIdAsync(long id)
    {
        const string sql = "SELECT book_id, title, category, price FROM lending.book WHERE book_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);      // @id in the SQL receives this value

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadBook(reader) : null;
    }
    public async Task UpdateAsync(Book book)
    {
        const string sql = "UPDATE lending.book " +
                           "SET title = @title, category = @category, price = @price " +
                           "WHERE book_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", book.Title);
        command.Parameters.AddWithValue("category", (object?)book.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("price", (object?)book.Price ?? DBNull.Value);
        command.Parameters.AddWithValue("id", book.BookId);

        await command.ExecuteNonQueryAsync();
    }
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM lending.book WHERE book_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }

}