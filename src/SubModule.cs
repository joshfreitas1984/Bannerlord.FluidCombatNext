using Bannerlord.ButterLib;
using Bannerlord.ButterLib.Common.Extensions;
using Bannerlord.ButterLib.HotKeys;
using Bannerlord.FluidCombatNext.Helper;
using Bannerlord.FluidCombatNext.HotKeys;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog;
using System;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.FluidCombatNext
{
    public class SubModule : MBSubModuleBase
    {
        private HotKeyManager? _hotkeyManager = null;

        public static readonly string Namespace = typeof(SubModule).Namespace;
        public static readonly string MainHarmonyDomain = $"mod.{Namespace.ToLower()}";
        public const string DisplayName = "Fluid Combat Next";
        public const string ModuleName = "FluidCombatNext"; //Can't have . or spaces
        public const string HotkeyCategory = "FluidCombatNext"; //Can't have . or spaces

        protected override void OnSubModuleLoad()
        {            
            AddLogging();

            try
            {
                Harmony harmony = new Harmony(MainHarmonyDomain);
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                MessageHelper.HandleError("Harmony Patching Failed", ex.ToString());
            }

            base.OnSubModuleLoad();
        }

        private void AddLogging()
        {
            //TODO: Replace the logging system because it seems to be badly implemented in ButrLib
            this.AddSerilogLoggerProvider($"{ModuleName}.txt",
                new[] { $"{Namespace}.*" },
                config => config.MinimumLevel.Is(LogEventLevel.Verbose));

            var logger = this.GetTempServiceProvider().GetRequiredService<ILogger<SubModule>>();
            MessageHelper.Log = logger;
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            //Register Hotkeys
            if (_hotkeyManager == null)
            {
                MessageHelper.HandleTrace($"{ModuleName}: Creating Hotkeys");

                _hotkeyManager = HotKeyManager.CreateWithOwnCategory(ModuleName, HotkeyCategory)!;

                _hotkeyManager.Add<FluidAttackKey>();
                _hotkeyManager.Add<FluidBlockKey>();

                _hotkeyManager.Add<LeftAttackKey>();
                _hotkeyManager.Add<RightAttackKey>();
                _hotkeyManager.Add<OverheadAttackKey>();
                _hotkeyManager.Add<StabAttackKey>();

                _hotkeyManager.Add<DefendLeftKey>();
                _hotkeyManager.Add<DefendRightKey>();
                _hotkeyManager.Add<DefendUpKey>();
                _hotkeyManager.Add<DefendDownKey>();

                _hotkeyManager.Build();
            }

            MessageHelper.HandleSuccess($"{ModuleName} Loaded Successfully!");
            MessageHelper.HandleWarning($"{ModuleName}: Don't forget to unbind default attack and block if using Fluid Attack or Fluid Block!");
        }
    }
}
