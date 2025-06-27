

namespace DeferredAsyncOperationsSemDeadLock
{
    internal static class ExemploDeOperacoesAsincronasTardias2
    {
        public async static Task Executar()
        {
            Console.WriteLine($"Iniciando operação do exemplo 2: operação assíncrona adiatada com Task.WhenAll. Thread id: {Thread.CurrentThread.ManagedThreadId}");

            var task = OperacaoAssincronaAdiada();

            var task2 = OutraOperacao();

            await task;

            await task2;

            Console.WriteLine($"operação do exemplo 2 Finalizada. Thread Id: {Thread.CurrentThread.ManagedThreadId}. Resultado: {task.Result}");
            
        }

        private static Task OutraOperacao()
        {
            Console.WriteLine($"Fazendo alguma outra coisa enquanto isto na main thread. Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            return Task.CompletedTask;
        }

        private static async Task<long> OperacaoAssincronaAdiada()
        {
            var tasks = new List<Task<long>>();
            for ( int ctr = 1 ; ctr <= 10 ; ctr++ )
            {
                Task<long> t = GerarNovaTask();

                tasks.Add(t);
            }


            var continuation = Task.WhenAll(tasks);

            try
            {
                await continuation;
            }
            catch ( AggregateException )
            { }

            Console.WriteLine($"Thread Id do DeferredAsyncOperation: {Thread.CurrentThread.ManagedThreadId} ");
            if ( continuation.Status == TaskStatus.RanToCompletion )
            {
                long grandTotal = 0;
                foreach ( var result in continuation.Result )
                {
                    grandTotal += result;
                    Console.WriteLine("Mean: {0:N2} -> n = 1,000", result / 1000.0);
                }

                Console.WriteLine("\nMean of Means: {0:N2}, n = 10,000",
                                  grandTotal / 10000);

                return grandTotal;
            }
            // Display information on faulted tasks.
            else
            {
                foreach ( var t in tasks )
                {
                    Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
                }

                return -1L;
            }
        }

        private static Task<long> GerarNovaTask()
        {
            int delayInterval = 1000;

            var t = Task.Run(async () =>
            {
                await Task.Delay(delayInterval);
                long total = 0;

                var rnd = new Random();
                // Generate 1,000 random numbers.
                for ( int n = 1 ; n <= 1000 ; n++ )
                    total += rnd.Next(0, 1000);
                return total;
            });
            return t;
        }

    }
}
