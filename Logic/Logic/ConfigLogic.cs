using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Constants;

namespace Logic.Logic;

public class ConfigLogic(IEfRepository efRepository) : IConfigLogic
{
    private readonly IBasicCrud<GlobalConfigs> _globalConfigCrud = efRepository.For<GlobalConfigs>();

    public async Task<GlobalConfigs> ResolveGlobalConfig()
    {
        return (await _globalConfigCrud.GetAll()).First();
    }

    public async Task SetGlobalConfig(GlobalConfigs globalConfigs)
    {
        await _globalConfigCrud.Update(globalConfigs.Id, globalConfigs);
    }
}