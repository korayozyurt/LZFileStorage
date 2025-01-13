namespace ConsoleTest
{
    internal class Program
    {
        private static CountdownEvent countdownEvent = new CountdownEvent(4);
        static async Task Main(string[] args)
        {
            new Thread(() => DoOperation("opt1", 500)).Start();
            new Thread(() => DoOperation("opt2", 1000)).Start();
            new Thread(() => DoOperation("opt3", 1500)).Start();

            Console.WriteLine("Ana thread 3 operasyonu bekliyor");
            countdownEvent.Wait();

            Console.WriteLine("Tüm operasyonlar tamamlandı");

        }

        static void DoOperation(string name, int delay)
        {
            Console.WriteLine($"{name} başladı.");
            Thread.Sleep(delay);
            Console.WriteLine($"{name} bitti. Signal gönderiliyor...");

            countdownEvent.Signal();
        }

    }
}
