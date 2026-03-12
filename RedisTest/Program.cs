using Cms.Infrastructure.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing Redis connection...");
        
        try
        {
            // 创建Redis缓存服务实例
            var cacheService = new RedisCacheService("localhost:6379");
            
            // 测试设置缓存
            Console.WriteLine("Setting cache...");
            await cacheService.SetAsync("test:key", "Hello Redis!", TimeSpan.FromMinutes(5));
            Console.WriteLine("Cache set successfully.");
            
            // 测试获取缓存
            Console.WriteLine("Getting cache...");
            var value = await cacheService.GetAsync<string>("test:key");
            Console.WriteLine($"Cache value: {value}");
            
            // 测试删除缓存
            Console.WriteLine("Removing cache...");
            await cacheService.RemoveAsync("test:key");
            Console.WriteLine("Cache removed successfully.");
            
            // 验证缓存是否已删除
            var removedValue = await cacheService.GetAsync<string>("test:key");
            Console.WriteLine($"Cache value after removal: {removedValue}");
            
            Console.WriteLine("Redis connection test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
