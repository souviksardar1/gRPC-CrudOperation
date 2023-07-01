using Grpc.Core;
using WorkorderPackage;

public class Program
{
    const string address = "127.0.0.1:50052";
    static async Task Main(string[] args)
    {
        try
        {
            Channel c = new Channel(address, ChannelCredentials.Insecure);
            c.ConnectAsync().ContinueWith((t) =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Client connected successfully");
                }
            });

            var client = new WorkorderService.WorkorderServiceClient(c);

            await CreateWorkorder(client);
            await ReadWorkorder(client);
            await UpdateWorkorder(client);
            await DeleteWorkorder(client);

            c.ShutdownAsync().Wait();
            Console.ReadKey();
        }
        catch (IOException e)
        {
            Console.WriteLine("Unable to connect the server in channel");
            throw;
        }
        finally
        {

        }
    }

    public static async Task CreateWorkorder(WorkorderService.WorkorderServiceClient client)
    {
        try
        {
            var inputPayload = new CreateWorkorderInput { Input = new WorkorderData { Location = "India1", SystemId = 1234 } };
            var result = client.CreateWo(inputPayload);

            Console.WriteLine($"Workorder is created with id : {result.Output.Id}");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Error in CreateWorkorder method. StatusCode = {ex.StatusCode}, Ex = {ex.StackTrace}");
        }

    }

    public static async Task ReadWorkorder(WorkorderService.WorkorderServiceClient client)
    {
        try
        {
            var input = new ReadWorkorderInput { Id = "64a03d2684107879a36a2dfc" };
            var result = client.ReadWo(input);
            if (string.IsNullOrWhiteSpace(result.Output.Id))
            {
                Console.WriteLine($"No data found for the id {input.Id}, Insert data and change id in ReadWorkorder method");
            }
            else 
            {
                Console.WriteLine($"Workorder response => {result.Output.ToString()}");
            }
            
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Error in ReadWorkorder method. StatusCode = {ex.StatusCode}, Ex = {ex.StackTrace}");
        }

    }
    public static async Task UpdateWorkorder(WorkorderService.WorkorderServiceClient client)
    {
        try
        {
            var input = new UpdateWorkorderInput { Id = "64a03d2684107879a36a2dfc" };
            var result = client.UpdateWo(input);

            if (result.ModifiedCount > 0)
            {
                Console.WriteLine($"Workorder updated count => {result.ModifiedCount.ToString()}");
            }
            else
            {
                Console.WriteLine($"No Workorder found with id '{input.Id}' to update");
            }
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Error in UpdateWorkorder method. StatusCode = {ex.StatusCode}, Ex = {ex.StackTrace}");
        }

    }

    public static async Task DeleteWorkorder(WorkorderService.WorkorderServiceClient client)
    {
        try
        {
            var input = new DeleteWorkorderInput { Id = "64a03d2684107879a36a2dfc" };
            var result = client.DeleteWo(input);

            if (result.DeleteCount > 0)
            {
                Console.WriteLine($"Workorder delete count => {result.DeleteCount.ToString()}, Id = {input.Id}");
            }
            else
            {
                Console.WriteLine($"No Workorder found with id '{input.Id}' to delete");
            }
            
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Error in DeleteWorkorder method. StatusCode = {ex.StatusCode}, Ex = {ex.StackTrace}");
        }

    }
}
