using System.Collections.Generic;
using System.Threading.Tasks;
using Demo_API.Models;

namespace Demo_API.Services
{
    public interface ITargetAssetService
    {
        Task<List<TargetAsset>> GetTargetAssets();
    }
}