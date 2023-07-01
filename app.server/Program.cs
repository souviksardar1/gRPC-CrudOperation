using app.server;
using app.server.Impl;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using WorkorderPackage;

public class Program
{
    const int Port = 50052;
    static void Main(string[] args)
    {
        Server s = null;
        try
        {
            var refleactionTemp = new ReflectionServiceImpl(WorkorderService.Descriptor, ServerReflection.Descriptor);
            s = new Server()
            {
                Services = { WorkorderService.BindService(new WorkorderServiceImpl()),
                ServerReflection.BindService(refleactionTemp)
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            s.Start();
            Console.WriteLine("Server is up and running in port 50052!");
            Console.ReadKey();
        }
        catch (IOException e)
        {
            Console.WriteLine("Unable to start server");
            throw;
        }
        finally
        {
            if (s != null)
            {
                s.ShutdownAsync().Wait();
            }
        }
    }
}