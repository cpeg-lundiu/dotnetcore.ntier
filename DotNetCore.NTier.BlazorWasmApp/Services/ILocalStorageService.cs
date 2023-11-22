namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public interface ILocalStorageService
    {
        public Task<T?> GetItem<T>(string key);
        public Task SetItem<T>(string key, T value);
        public Task RemoveItem(string key);
    }
}
