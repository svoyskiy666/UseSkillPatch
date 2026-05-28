using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UseSkillPatch;

public class UseSkillPatch : BasePlugin
{
    public override string ModuleName => "UseSkillPatch";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Maslenka";
    public override string ModuleDescription => "Use jRandomSkills on E button";

    private readonly Dictionary<ulong, bool> _pressed = new();
    private readonly Dictionary<ulong, DateTime> _lastUseTime = new();
    private const double DebounceMs = 300;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            ulong steamId = @event.Userid?.SteamID ?? 0;
            if (steamId != 0)
            {
                _pressed.Remove(steamId);
                _lastUseTime.Remove(steamId);
            }
            return HookResult.Continue;
        });
    }

    private void OnTick()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid)
                continue;

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid)
                continue;

            if (!player.PawnIsAlive)
                continue;

            if (player.TeamNum <= 1) 
                continue;

            ulong steamId = player.SteamID;

            bool isUsing = (player.Buttons & PlayerButtons.Use) != 0;

            // Инициализация состояния, если его нет
            if (!_pressed.ContainsKey(steamId))
                _pressed[steamId] = false;

            // Обработка нажатия (переход 0 -> 1)
            if (isUsing && !_pressed[steamId])
            {
                _pressed[steamId] = true;

                bool canExecute = true;
                if (_lastUseTime.ContainsKey(steamId))
                {
                    double elapsed = (DateTime.Now - _lastUseTime[steamId]).TotalMilliseconds;
                    if (elapsed < DebounceMs)
                        canExecute = false;
                }

                if (canExecute)
                {
                    _lastUseTime[steamId] = DateTime.Now;

                    player.ExecuteClientCommandFromServer("css_useskill");
                    
                }
            }
            else if (!isUsing)
            {
                _pressed[steamId] = false;
            }
        }
    }
}