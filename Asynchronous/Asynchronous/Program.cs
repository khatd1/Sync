using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static ConcurrentBag<int> threadIds = new ConcurrentBag<int>();

    static async Task ProcessCustomerAsync(int customerId)
    {
        Stopwatch sw = Stopwatch.StartNew();
        int threadId = Thread.CurrentThread.ManagedThreadId;
        threadIds.Add(threadId);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] User {customerId} yêu cầu data (Thread: {threadId})");
        await Task.Delay(3000); // Giả lập thời gian xử lý
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] User {customerId} lấy được data (Thread: {threadId})");

        sw.Stop();
    }

    static async Task Main()
    {
        // Giới hạn Thread Pool chỉ có 4 thread
        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);

        Stopwatch stopwatch = Stopwatch.StartNew();
        Console.WriteLine("Bắt đầu xử lý yêu cầu...");

        var tasks = Enumerable.Range(1, 6)
                              .Select(i => ProcessCustomerAsync(i))
                              .ToArray();

        await Task.WhenAll(tasks); // Chờ tất cả request hoàn thành

        stopwatch.Stop();
        Console.WriteLine($"\nTất cả user đã được data trong {stopwatch.Elapsed.TotalSeconds} giây.");
        Console.WriteLine($"Tổng số thread đã sử dụng: {threadIds.Distinct().Count()}");
    }
}
