using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UseSkillPatch;

public class UseSkillPatch : BasePlugin
{
    public override string ModuleName => "UseSkillPatch";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Melonholea";
    public override string ModuleDescription => "Use jRandomSkills on E button";

    private readonly Dictionary<ulong, bool> _pressed = new();

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
    }

    private void OnTick()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null)
                continue;

            if (!player.IsValid)
                continue;

            if (player.PlayerPawn == null)
                continue;

            if (!player.PlayerPawn.IsValid)
                continue;

            if (!player.PawnIsAlive)
                continue;

            if (player.TeamNum <= 1)
                continue;

            ulong steamId = player.SteamID;

            bool isUsing = (player.Buttons & PlayerButtons.Use) != 0;

            if (!_pressed.ContainsKey(steamId))
                _pressed[steamId] = false;

            if (isUsing && !_pressed[steamId])
            {
                _pressed[steamId] = true;

                Server.NextFrame(() =>
                {
                    if (player == null)
                        return;

                    if (!player.IsValid)
                        return;

                    player.ExecuteClientCommandFromServer("css_useskill");
                });
            }
            else if (!isUsing)
            {
                _pressed[steamId] = false;
            }
        }
    }
}
