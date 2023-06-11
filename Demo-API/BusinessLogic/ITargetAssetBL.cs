using Demo_API.Models;

namespace Demo_API.BusinessLogic
{
    public interface ITargetAssetBL
    {
        Task<List<TargetAsset>> ProcessTargetAssets();
    }
}
