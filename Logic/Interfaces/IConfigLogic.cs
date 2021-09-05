using System;
using System.Threading.Tasks;
using Models.ViewModels.Config;

namespace Logic.Interfaces
{
    public interface IConfigLogic
    {
        GlobalConfigViewModel GetResolveGlobalConfig();

        Task UpdateGlobalConfig(Func<GlobalConfigViewModel, GlobalConfigViewModel> update);

        Task Refresh();
    }
}