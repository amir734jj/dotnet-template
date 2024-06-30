using System.Threading.Tasks;
using Models.Constants;

namespace Logic.Interfaces;

public interface IConfigLogic
{
    Task<GlobalConfigs> ResolveGlobalConfig();

    Task SetGlobalConfig(GlobalConfigs globalConfigs);
}