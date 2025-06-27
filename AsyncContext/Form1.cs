using System.Diagnostics;
namespace AsyncContext
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ReportThread()
        {
            Debug.WriteLine($"Thread Id: {Environment.CurrentManagedThreadId}");
        }

        private async void BtnGo_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();

            lbxList.Items.Add("Starting call...");

            ReportThread();

            int count = await CallClient(client);

            ReportThread();

            lbxList.Items.Add("Results in...");

            lbxList.Items.Add($"Found {count} BBCs");

            async Task<int> CallClient(HttpClient client)
            {
                Debug.WriteLine($"Thread Id prior to await: {Environment.CurrentManagedThreadId}");

                var response = await client.GetAsync(new Uri("https://bbc.co.uk"))
                                           .ConfigureAwait(true);
                // if ConfigureAwait == true, 
                // Then this continuation will be run in the context that was captured by the await.
                // In our case, this captured context is the UI context (this can be checked by line 34, which depicts the same thread
                // of the UI context (shown by line 22).
                
                Debug.WriteLine($"Thread Id after await: {Environment.CurrentManagedThreadId}");

                int count = CountInstances(response.Content.ReadAsStream());
                
                return count;
            }
        }

        private static int CountInstances(Stream stream)
        {
            int count = 0;

            var reader = new StreamReader(stream);

            var line = reader.ReadLine();

            while (line != null)
            {
                Thread.Sleep(5);
                count += line.Contains("BBC") ? 1 : 0;
                line = reader.ReadLine();
            }

            return count;
        }
    }
}