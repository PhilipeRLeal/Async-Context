
using DeferredAsyncOperationsSemDeadLock;

public class Program
{
    public static async Task Main()
    {

        await ExemploDeOperacoesAsincronasTardias1.Executar();

        Console.WriteLine("\n");

        await ExemploDeOperacoesAsincronasTardias2.Executar();

        Console.WriteLine("\n");

        await ExemploDeOperacoesAsincronasTardias3.Executar();

        Console.WriteLine("\n");

        await ExemploDeOperacoesAsincronasTardias4.Executar();

        Console.WriteLine($"\n\n \t Finalização de todos os testes.  Thread Id: {Thread.CurrentThread.ManagedThreadId}. ");

        Console.WriteLine("Pressione qualquer botão para finalização do app. \n\n\n");
        Console.ReadLine();

    }
}