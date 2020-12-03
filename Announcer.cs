using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using SDG.Unturned;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.Core.Helpers;

[assembly: PluginMetadata("F.Announcer", DisplayName = "Announcer")]
namespace Announcer
{
    public class Announcer : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly ILogger<Announcer> m_Logger;

        private bool Load = false;
        private int message = 0;

        public Announcer(
            IConfiguration configuration, 
            ILogger<Announcer> logger, 
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_Logger = logger;
        }
        
#pragma warning disable 1998
        protected override async UniTask OnLoadAsync()
#pragma warning restore 1998
        {
            m_Logger.LogInformation("Announcer Loaded");
            m_Logger.LogInformation("FPlugins Discord: https://discord.gg/RuWChce");
            AsyncHelper.Schedule("Announcement Start", () => Announcement().AsTask());
            Load = true;
        }
        
#pragma warning disable 1998
        protected override async UniTask OnUnloadAsync()
#pragma warning restore 1998
        {
            m_Logger.LogInformation("Announcer Unloaded");
            Load = false;
        }

        private async UniTask Announcement()
        {
            await UniTask.SwitchToMainThread();
            while(Load)
            {
                await Task.Delay(m_Configuration.Get<Config>().miliseconds);
                var announces = m_Configuration.Get<Config>().announces;
                if (message >= announces.Count())
                {
                    message = 0;
                }

                var selected = announces[message];
                
                ChatManager.serverSendMessage(selected.message.Replace("{", "<").Replace("}", ">"), UnityEngine.Color.white, null, null, EChatMode.GLOBAL, selected.url, selected.isrich);
                message++;
            }
        }
    }

    public class Config
    {
        public List<Announce> announces { get; set; }
        public int miliseconds { get; set; } 
    }
    
    public class Announce
    {
        public string url { get; set; }
        public string message { get; set; }
        public bool isrich { get; set; }
    }
}
