using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;

namespace gRPC.Server.Services
{
    public class ToDoService : TodoServiceContract.TodoServiceContractBase
    {
        private readonly IMemoryCache _cache;

        private const string CacheKey = "ToDoServiceStorage";

        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetPriority(CacheItemPriority.High)
            .SetAbsoluteExpiration(TimeSpan.FromHours(8));

        private List<TodoParam> Repository =>
            !_cache.TryGetValue(CacheKey, out var lista) ? new List<TodoParam>() : (List<TodoParam>)lista!;

        public ToDoService(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public override Task<Empty> Add(AddTodoParam request, ServerCallContext context)
        {
            Repository.Add(new TodoParam
            {
                Id = Repository.Count + 1,
                Description = request.Description,
                Done = request.Done
            });
                
            _cache.Set(CacheKey, Repository, _cacheEntryOptions);

            return Task.FromResult(new Empty());
        }

        public override Task<BoolValue> Remove(Int32Value request, ServerCallContext context)
        {
            var result = Repository.RemoveAll(i => i.Id == request.Value);

            return Task.FromResult(new BoolValue
            {
                Value = result > 0
            });
        }

        public override Task<BoolValue> Update(TodoParam request, ServerCallContext context)
        {
            var index = Repository.FindIndex(i => i.Id == request.Id);

            if (index > -1)
            {
                Repository[index] = request;
            }

            return Task.FromResult(new BoolValue
            {
                Value = index > -1
            });
        }

        public override Task<GetAllTodoResult> GetAll(Empty request, ServerCallContext context)
        {
            var result = new GetAllTodoResult();
            result.Value.AddRange(Repository);

            return Task.FromResult(result);
        }
    }
}