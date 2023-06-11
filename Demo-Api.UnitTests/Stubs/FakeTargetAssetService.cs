using Demo_API.Models;
using Demo_API.Services;

namespace Demo_API.UnitTests.Stubs
{
    public class FakeTargetAssetService : ITargetAssetService
    {
        public List<TargetAsset> _targetAssets { get; set; }

        public FakeTargetAssetService(List<TargetAsset> targetAssets)
        {
            _targetAssets = targetAssets;
        }

        public Task<List<TargetAsset>> GetTargetAssets()
        {
            return Task.FromResult(_targetAssets);
        }
    }
}
