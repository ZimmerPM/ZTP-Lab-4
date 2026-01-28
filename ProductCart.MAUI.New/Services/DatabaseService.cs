using ProductCart.MAUI.Models;
using SQLite;
using System.Diagnostics;

namespace ProductCart.MAUI.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "productcart.db3");
        Debug.WriteLine($"Database path: {_dbPath}");
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<ProductCached>();
        Debug.WriteLine("Database initialized");
    }

    public async Task<List<ProductCached>> GetAllProductsAsync()
    {
        await InitAsync();
        return await _database!.Table<ProductCached>().ToListAsync();
    }

    public async Task<ProductCached?> GetProductByIdAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<ProductCached>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task SaveProductsAsync(List<ProductCached> products)
    {
        await InitAsync();

        await _database!.DeleteAllAsync<ProductCached>();

        await _database.InsertAllAsync(products);

        Debug.WriteLine($"Saved {products.Count} products to cache");
    }

    public async Task<bool> IsCacheValidAsync()
    {
        await InitAsync();

        var latestProduct = await _database!.Table<ProductCached>()
            .OrderByDescending(p => p.LastUpdated)
            .FirstOrDefaultAsync();

        if (latestProduct == null)
            return false;

        var cacheAge = DateTime.UtcNow - latestProduct.LastUpdated;
        return cacheAge.TotalHours < 24;
    }

    public async Task ClearCacheAsync()
    {
        await InitAsync();
        await _database!.DeleteAllAsync<ProductCached>();
        Debug.WriteLine("Cache cleared");
    }
}
