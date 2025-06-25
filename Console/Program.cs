
// Thread inicial do programa:
foreach ( var configureAwait in new List<bool> { true, false } )
{
    Console.WriteLine($"Configuração do ConfigAwait: {configureAwait.ToString()}");
    Console.WriteLine($"Thread Inicial do Program Main: {Thread.CurrentThread.ManagedThreadId}");
    WithAsync(configureAwait);

    Console.WriteLine($"Fazendo alguma outra coisa no Program Main com a Thread Id {Thread.CurrentThread.ManagedThreadId}");

    Thread.Sleep(2000);
    // Esta thread será a mesma que do início da execução (Thread 1)
    Console.WriteLine($"Thread Id do Program Main após retorno do WithAsync: {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine($"Context do Program Main: {SynchronizationContext.Current}");

    Console.WriteLine("\n\n\n\n\n");
}


Console.WriteLine("Iniciando Testes do  do WithContinuationOnlyAsync");
// Thread inicial do programa:
foreach ( var configureAwait in new List<bool> { true, false } )
{
    Console.WriteLine($"Configuração do ConfigAwait: {configureAwait.ToString()}");
    Console.WriteLine($"Thread Id do Program Main: {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine($"Context do Program Main: {SynchronizationContext.Current}");


    WithContinuationOnlyAsync(configureAwait);
    Console.WriteLine($"Fazendo alguma outra coisa no Program Main com a Thread Id {Thread.CurrentThread.ManagedThreadId}");

    Thread.Sleep(2000);
    Console.WriteLine($"Thread Id do Program Main  após retorno do WithContinuationOnlyAsync: {Thread.CurrentThread.ManagedThreadId}");

    Console.WriteLine("\n\n\n\n\n");
}



Console.ReadLine();



static async void WithAsync(bool configureAwait = true)
{
    Console.WriteLine("Starting call of WithAsync");
    // Aqui ainda estamos na mesma thread inicial do programa (Thread 1).
    Console.WriteLine($"Thread Id do contexto do WithAsync com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");
    var mainContext = SynchronizationContext.Current;

    var client = new HttpClient();

    Console.WriteLine("Starting to download data from HTML");
    var uri = new Uri("https://bbc.co.uk");

    var task = client.GetAsync(uri);

    var configuredTask = task.ConfigureAwait(configureAwait);

    var response = await task.ContinueWith(t => {
        var result = t.Result;
        Console.WriteLine("Completed data download from HTML");
        Console.WriteLine($"Thread Id da ContinueWith com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");
        var withinAwaitContext = SynchronizationContext.Current;

        Console.WriteLine($"Contexto do await client.GetAsync: {withinAwaitContext}");
        Console.WriteLine($"Contexto do await client.GetAsync é igual ao mainContext do método WithAsync: {withinAwaitContext?.Equals(mainContext)}");
        return result;
    }).ConfigureAwait(configureAwait);

    // Notar que esta Thread é a mesma utilizada pelo client.GetAsync.
    // OU seja, a continuation do await client.GetAsync é a mesma; logo, sua thread também é.
    Console.WriteLine($"Thread Id da Continuation do await client.GetAsync com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine($"Contexto  da Continuation do await client.GetAsync: {SynchronizationContext.Current}");
    // Console.WriteLine($"{new StreamReader(response.Content.ReadAsStream()).ReadLine()?.Substring(0, 80) ?? ""}...");
}



static async void WithContinuationOnlyAsync(bool configureAwait = true)
{
    Console.WriteLine("Starting call of WithContinuationOnlyAsync");
    // Aqui ainda estamos na mesma thread inicial do programa (Thread 1).
    Console.WriteLine($"Thread Id do contexto do WithContinuationOnlyAsync com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");


    var client = new HttpClient();

    Console.WriteLine("Starting to download data from HTML");
    var response = await client.GetAsync(new Uri("https://bbc.co.uk")).ContinueWith(t => {
        var result = t.Result;
        Console.WriteLine("Completed data download from HTML");
        Console.WriteLine($"Thread Id da ContinueWith com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");

        // Console.WriteLine($"{new StreamReader(result.Content.ReadAsStream()).ReadLine()?.Substring(0, 80) ?? ""}...");

        return result;
    }).ConfigureAwait(configureAwait);


    // Notar que esta Thread é a mesma utilizada pelo client.GetAsync.
    // OU seja, a continuation do await client.GetAsync é a mesma; logo, sua thread também é.
    Console.WriteLine($"Thread Id da Continuation do await client.GetAsync com configureAwait = {configureAwait}: {Thread.CurrentThread.ManagedThreadId}");

}
