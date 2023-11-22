namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public interface IHttpService
    {
        public Task<T?> Get<T>(string uri);
        public Task Post(string uri, object value);
        public Task<T?> Post<T>(string uri, object value);
        public Task Put(string uri, object value);
        public Task<T?> Put<T>(string uri, object value);
        public Task Delete(string uri);
        public Task<T?> Delete<T>(string uri);
    }
}
