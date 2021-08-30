using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Dal.Extensions;
using Dal.Interfaces;
using Logic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.ViewModels.Config;
using Newtonsoft.Json;
using static Models.Constants.GlobalConfigs;
using static Models.Constants.ApplicationConstants;

namespace Logic.Logic
{
    public class ConfigLogic : IConfigLogic
    {
        private readonly IFileService _fileService;

        private readonly ILogger<ConfigLogic> _logger;

        public ConfigLogic(IFileService fileService, ILogger<ConfigLogic> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        private async Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            UpdateGlobalConfigs(globalConfigViewModel);

            var response = await _fileService.Upload(ConfigFile, ConfigFile, "application/json",
                new MemoryStream(DefaultEncoding.GetBytes(JsonConvert.SerializeObject(globalConfigViewModel))),
                new Dictionary<string, string>
                {
                    {"Description", "Application config file"}
                }
            );

            if (response.Status == HttpStatusCode.BadRequest)
            {
                _logger.LogError("Failed to sync config file with S3");
            }
        }

        public GlobalConfigViewModel ResolveGlobalConfig()
        {
            return ToViewModel();
        }

        public async Task UpdateGlobalConfig(Func<GlobalConfigViewModel, GlobalConfigViewModel> update)
        {
            var re = update(ResolveGlobalConfig());
            
            await SetGlobalConfig(re);
        }

        public async Task Refresh()
        {
            var response = await _fileService.Download(ConfigFile);
            
            if (response.Status == HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully fetched the config from S3");

                UpdateGlobalConfigs(response.Data.Deserialize<GlobalConfigViewModel>());
            }
            else
            {
                _logger.LogError("Failed to fetch the config from S3");
            }
        }
    }
}