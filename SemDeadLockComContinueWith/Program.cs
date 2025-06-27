
using System.Net;

public class Program
{
    public static void Main()
    {

        var uri = new Uri("https://www.youtube.com/watch?v=5d6YoHuSvoI&lc=UgzCBk81zt4CgFLLArZ4AaABAg.AJnM3DxVEYlAJne9eFsBwy");

        ReportThread();
        CustomObterJsonAsync(uri, true).ConfigureAwait(true);

        ExecutarAlgumaOutraCoisa();
        Console.WriteLine($"Thread Id após execução do ExecutarAlgumaOutraCoisa (já no escopo main): {Thread.CurrentThread.ManagedThreadId}.");
        Console.WriteLine($"Context após execução do ExecutarAlgumaOutraCoisa (já no escopo main): {TaskScheduler.Current}. \n");
        Console.ReadLine();

    }


    private static void ExecutarAlgumaOutraCoisa()
    {
        Console.WriteLine("Executando alguma outra coisa enquanto isto na main thread");
        ReportThread();
        Thread.Sleep(1000);
        Console.WriteLine("Finalização da execução da outra coisa");
        ReportThread();
    }

    private static void ReportThread()
    {
        Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}. \n");
    }

    private static async Task<List<string>> CustomObterJsonAsync(Uri uri, bool configureAwait = true)
    {
        Console.WriteLine($"Thread Id do CustomObterJsonAsync antes do await: {Thread.CurrentThread.ManagedThreadId}. \n");
        using ( var client = new WebClient() )
        {
            var task = Task.Factory.StartNew( () => {
                string x = "";
                client.DownloadStringAsync(uri, x);

                return x;
            });

            var dadosProcessados = new List<string>();

            var dadosProcessadosTask = task.ContinueWith(t =>
            {
                var result  = t.Result as string;
                Console.WriteLine($"Thread Id do ContinueWith do CustomObterJsonAsync: {Thread.CurrentThread.ManagedThreadId} com configureAwait = {configureAwait}.");
                Console.WriteLine($"Context do ContinueWith do CustomObterJsonAsync após await com configureAwait = {configureAwait}: {TaskScheduler.Current}. \n");
                // Processando dados do Resultado:
                // Vamos representar uma operação bem demorada aqui (5 segundos):
                Thread.Sleep(5000);
                var x = result.Split(',');

                dadosProcessados = x.ToList();
            });

            await dadosProcessadosTask;


            Console.WriteLine($"Thread Id do CustomObterJsonAsync após await com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}.");
            Console.WriteLine($"Context do CustomObterJsonAsync após await com configureAwait = {configureAwait}: {TaskScheduler.Current}. \n");
            return dadosProcessados;
        }
    }
}
