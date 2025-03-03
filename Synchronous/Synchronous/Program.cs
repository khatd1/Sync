using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static ConcurrentBag<int> threadIds = new ConcurrentBag<int>();
    static SemaphoreSlim semaphore = new SemaphoreSlim(4); // Giới hạn tối đa 4 thread chạy cùng lúc

    static async Task ProcessCustomer(int customerId)
    {
        await semaphore.WaitAsync(); // Đợi đến khi có thread trống
        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            threadIds.Add(threadId);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] User {customerId} yêu cầu data (Thread: {threadId})");
            await Task.Delay(3000); // Giả lập xử lý
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] User {customerId} lấy được data (Thread: {threadId})");

            sw.Stop();
        }
        finally
        {
            semaphore.Release(); // Giải phóng slot trong thread pool
        }
    }

    static async Task Main()
    {
        // Giới hạn thread pool
        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);

        Stopwatch stopwatch = Stopwatch.StartNew();
        Console.WriteLine("Bắt đầu xử lý yêu cầu...");

        var tasks = Enumerable.Range(1, 6)
            .Select(i => Task.Run(() => ProcessCustomer(i))) // Chạy các task
            .ToArray();

        await Task.WhenAll(tasks); // Chờ tất cả hoàn thành

        stopwatch.Stop();
        Console.WriteLine($"\nTất cả user đã được data trong {stopwatch.Elapsed.TotalSeconds} giây.");
        Console.WriteLine($"Tổng số thread đã sử dụng: {threadIds.Distinct().Count()}");
    }
}
