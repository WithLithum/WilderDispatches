namespace WilderDispatches.Callouts;

using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WilderDispatches.Interop;

/// <summary>
/// Provides a base class for dispatches in Wilder Dispatch.
/// </summary>
public abstract class WilderDispatch : Callout
{
    private readonly Properties _properties;
    private bool _loaded;

    protected WilderDispatch(Properties properties)
    {
        _properties = properties;
    }

    public struct Properties
    {
        public float MaximumDistance { get; set; }
        public float MinimumDistance { get; set; }
        public string Message { get; set; }
        public string Advisory { get; set; }
        public EWorldZoneCounty County { get; set; }
        public string Priority { get; set; }
    }

    public Vector3 Location { get; protected set; }

    protected Blip Blip { get; set; }

    public override bool OnBeforeCalloutDisplayed()
    {
        if (Functions.GetZoneAtPosition(Game.LocalPlayer.Character.Position).County != _properties.County)
        {
            Game.LogTrivial($"WD: Ignored callout with message {_properties.Message} because not in county {_properties.County}");
            return false;
        }

        if (!OnCreation())
        {
            Game.LogTrivial($"WD: Ignored callout with message {_properties.Message} because OnCreation returned false");
            return false;
        }

        Game.LogTrivial("WD: Callout created");
        this.AddMaximumDistanceCheck(_properties.MaximumDistance, Location);
        this.AddMinimumDistanceCheck(_properties.MinimumDistance, Location);

        this.CalloutPosition = Location;
        this.CalloutAdvisory = _properties.Advisory;
        this.CalloutMessage = _properties.Message;

        this.ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 120f);

        return base.OnBeforeCalloutDisplayed();
    }

    public override void OnCalloutDisplayed()
    {
        if (InteropHelper.IsCalloutInterfaceRunning)
        {
            CalloutInterfaceFunctions.SendCalloutDetails(this, _properties.Priority);
        }

        base.OnCalloutDisplayed();
    }

    protected void PushComputerUpdate(string message)
    {
        if (InteropHelper.IsCalloutInterfaceRunning)
        {
            CalloutInterfaceFunctions.SendMessage(this, message);
        }
    }

    public override bool OnCalloutAccepted()
    {
        if (!base.OnCalloutAccepted())
        {
            return false;
        }

        var result = OnAccepted();

        if (result)
        {
            _loaded = true;
        }

        return result;
    }

    public override void Process()
    {
        if (!_loaded)
        {
            return;
        }

        Tick();
        base.Process();
    }

    public abstract void Tick();

    public override void End()
    {
        if (Blip) Blip.Delete();

        CleanUp();
        base.End();
    }

    public virtual void CleanUp()
    {
    }

    protected abstract bool OnAccepted();

    public abstract bool OnCreation();
}
