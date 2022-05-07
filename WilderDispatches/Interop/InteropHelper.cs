namespace WilderDispatches.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class InteropHelper
{
    private static readonly Func<string, Version, bool> IsVersionLoaded = (plugin, version) =>
            LSPD_First_Response.Mod.API.Functions.GetAllUserPlugins().Any(x => x.GetName().Name.Equals(plugin) && x.GetName().Version.CompareTo(version) >= 0);

    public static readonly bool IsCalloutInterfaceRunning = IsVersionLoaded("CalloutInterface", new Version("1.2"));
}
