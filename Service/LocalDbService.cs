using SQLite;

namespace PersonalTask;

public class LocalDbService
{
    private const string DbName = "local.db";
    private readonly SQLiteAsyncConnection _connection;

    public LocalDbService()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory,DbName));
        _connection.CreateTableAsync<Duty>();
    }

    public async Task<List<Duty>> GetDuties()
    {
        return await _connection.Table<Duty>().ToListAsync();
    }

    public async Task<Duty>GetById(int id)
    {
        return await _connection.Table<Duty>().Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task Create(Duty entity)
    {
        await _connection.InsertAsync(entity);
    }

    public async Task Update(Duty entity)
    {
        await _connection.UpdateAsync(entity);
    }

    public async Task Delete(Duty entity)
    {
        await _connection.DeleteAsync(entity);
    }
}
