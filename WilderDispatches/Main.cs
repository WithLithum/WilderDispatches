namespace WilderDispatches;

using LSPD_First_Response.Mod.API;
using Rage;

public class Main : Plugin
{
    public static readonly RelationshipGroup Killer =
        new("WDKILLER");

    public override void Finally()
    {
        // Nothing to cleanup really
    }

    public override void Initialize()
    {
        Killer.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
        Killer.SetRelationshipWith(new RelationshipGroup("CIVMALE"), Relationship.Hate);
        Killer.SetRelationshipWith(new RelationshipGroup("CIVFEMALE"), Relationship.Hate);
        Killer.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
    }
}
