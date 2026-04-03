using Core.Models;

namespace DungeonGenerator.Sandbox.Data;

public static class RoomTags
{
    public static readonly RoomTag SafeZone = new("SafeZone");
    public static readonly RoomTag Tutorial = new("Tutorial");
    public static readonly RoomTag Stealth = new("Stealth");
    public static readonly RoomTag Entry = new("Entry");
    public static readonly RoomTag HighSecurity = new("HighSecurity");
    public static readonly RoomTag Escape = new("Escape");
    public static readonly RoomTag Critical = new("Critical");
    public static readonly RoomTag Destroy = new("Destroy");
    public static readonly RoomTag Locked = new("Locked");
    public static readonly RoomTag LaserTraps = new("LaserTraps");
    public static readonly RoomTag Large = new("Large");
    public static readonly RoomTag Damaged = new("Damaged");
    public static readonly RoomTag Loot = new("Loot");
    public static readonly RoomTag Laser = new("Laser");
    public static readonly RoomTag Damage = new("Damage");
    public static readonly RoomTag Spikes = new("Spikes");
    public static readonly RoomTag Timing = new("Timing");
    public static readonly RoomTag Poison = new("Poison");
    public static readonly RoomTag Switch = new("Switch");
    public static readonly RoomTag Weight = new("Weight");
    public static readonly RoomTag Code = new("Code");
}
