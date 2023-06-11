using Demo_API.Models;
using Demo_API.Services;

namespace Demo_API.BusinessLogic
{
    public class TargetAssetBL: ITargetAssetBL
    {
        private readonly ITargetAssetService _targetAssetService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TargetAssetBL(ITargetAssetService targetAssetService, IDateTimeProvider dateTimeProvider)
        {
            _targetAssetService = targetAssetService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<TargetAsset>> ProcessTargetAssets()
        {
            var targetAssets = await _targetAssetService.GetTargetAssets();
            if (targetAssets is null || targetAssets.Count == 0 || targetAssets.All(x => x is null))
            {
                throw new ArgumentException("Endpoint returned invalid data");
            }
            targetAssets.RemoveAll(a => a is null);
            Dictionary<int, TargetAsset> dict = targetAssets.ToDictionary(a => a.Id, a => a);

            foreach (var asset in targetAssets)
            {
                asset.IsStartable = asset.Status == "Running" && _dateTimeProvider.Now.Day == 3;
                asset.ParentTargetAssetCount = GetParentCount(asset, dict);
            }

            return targetAssets;
        }

        private int GetParentCount(TargetAsset asset, Dictionary<int, TargetAsset> dict)
        {
            HashSet<int> visited = new HashSet<int>();
            int count = 0;

            //Add starting ID so that we don't count that.
            visited.Add(asset.Id);
            
            while (asset.ParentId.HasValue)
            {
                if (!dict.TryGetValue(asset.ParentId.Value, out asset) || visited.Contains(asset.Id))
                {
                    // This will stop the loop if a circular dependency is detected
                    break;
                }

                visited.Add(asset.Id);
                count++;
            }

            return count;
        }
    }
}
