namespace WilderDispatches.Callouts;

using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;

[CalloutInfo("WDFirearmDischarge", CalloutProbability.Low)]
public sealed class FirearmDischarge : WilderDispatch
{
    private Ped _suspect;
    private bool _objectiveDealWith;
    private bool _sendBackup;
    private Vehicle _backupCar;
    private Blip _backupBlip;

    public FirearmDischarge() : base(new Properties
    {
        Message = "Firearm discharged",
        Advisory = "",
        County = EWorldZoneCounty.BlaineCounty,
        MaximumDistance = 870f,
        MinimumDistance = 350f,
        Priority = "CODE 3"
    })
    {
    }

    public override bool OnCreation()
    {
        this.Location = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position
            .Around2D(400f, 760f));

        Functions.PlayScannerAudioUsingPosition("ATTENTION_WD WE_HAVE CRIME_FIREARM_DISCHARGED_IN_PUBLIC IN_OR_ON_POSITION SUSPECT_ARMED RESPOND_CODE_3",
            Location);

        return true;
    }

    protected override bool OnAccepted()
    {
        _suspect = new Ped(Location.Around(10f, 30f))
        {
            IsPersistent = true,
            RelationshipGroup = new RelationshipGroup("MISSION_1")
        };

        _suspect.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, 9999, true);

        Game.DisplaySubtitle("Go to ~r~suspect.");
        Blip = _suspect.AttachBlip();
        NativeFunction.Natives.SET_BLIP_COLOUR(Blip, 2);
        Blip.IsFriendly = false;
        Blip.Sprite = BlipSprite.Enemy;
        Blip.Scale = 0.8f;
        Blip.IsRouteEnabled = true;

        return true;
    }

    public override void Tick()
    {
        if (!_sendBackup && (Game.LocalPlayer.Character.Position.DistanceTo(Location) < 75f
            || _objectiveDealWith))
        {
            _sendBackup = true;
            _backupCar = Functions.RequestBackup(CalloutPosition, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);

            _backupBlip = _backupCar.AttachBlip();
            NativeFunction.Natives.SET_BLIP_COLOUR(Blip, 2);
            _backupBlip.IsFriendly = true;
            _backupBlip.Sprite = BlipSprite.Enemy;
            _backupBlip.Scale = 1.0f;

            PushComputerUpdate($"* UNIT {_backupCar.Handle.Value:x5} ATTACHED.");
        }

        if (!_objectiveDealWith && Game.LocalPlayer.Character.Position.DistanceTo(_suspect.Position) < 95f
            && _suspect.IsOnScreen)
        {
            _objectiveDealWith = true;
            Game.DisplaySubtitle("Take out or arrest ~r~suspect~s~.");
            Blip.IsRouteEnabled = false;
        }
    }

    public override void CleanUp()
    {
        base.CleanUp();

        if (_suspect && !Functions.IsPedArrested(_suspect)) _suspect.Dismiss();
        if (_backupCar) _backupCar.Dismiss();
        if (_backupBlip) _backupBlip.Delete();
    }
}
