



namespace DeferredAsyncOperationsSemDeadLock
{
    internal static class ExemploDeOperacoesAsincronasTardias4
    {

        public async static Task Executar()
        {
            Console.WriteLine($"Iniciando operação do exemplo 4: operação assíncrona adiatada com ContinueWith. Thread id: {Thread.CurrentThread.ManagedThreadId}");

            var task = OperacaoAssincronaAdiada();

            var task2 = OutraOperacao();

            await task;

            await task2;

            Console.WriteLine($"operação do exemplo 1 Finalizada. Thread Id: {Thread.CurrentThread.ManagedThreadId}. Resultado: {task.Result}");

        }

        private static Task OutraOperacao()
        {
            Console.WriteLine($"Fazendo alguma outra coisa enquanto isto na main thread. Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            return Task.CompletedTask;
        }

        private static async Task<int> OperacaoAssincronaAdiada()
        {

            var listaDeURLs = new List<string>();

            listaDeURLs.Add("https://www.youtube.com/youtube");
            listaDeURLs.Add("https://www.google.com");
            listaDeURLs.Add("https://github.com");

            IEnumerable<Task<int>> tasks = listaDeURLs.Select(url => ProcessarUrl(url));

            var continuation = Task.WhenAll(tasks);

            var resultTask = continuation.ContinueWith(t =>
            {
                return t.Result.Sum();
            });

            try
            {
                return await resultTask;
            }
            catch (OperationCanceledException ex)
            {
                // pass
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return -1;
        }

        private static async Task<int> ProcessarUrl(string url)
        {
            using ( var cliente = new HttpClient() )
            {
                var result = await cliente.GetStringAsync(url);
                // simular um processamento destes dados aqui:
                Thread.Sleep(5000);
                return result.Length;
            }
        }
    }
}
