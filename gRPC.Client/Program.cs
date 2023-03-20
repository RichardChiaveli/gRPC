// See https://aka.ms/new-console-template for more information

using gRPC.Client;
using Grpc.Net.Client;

Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("http://localhost:5042");
var client = new TodoServiceContract.TodoServiceContractClient(channel);

client.Add(new AddTodoParam());