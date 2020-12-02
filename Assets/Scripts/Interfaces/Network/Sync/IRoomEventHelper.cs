using System;

/// <summary>
/// This requires no NetworkID, and can be directly used 
/// </summary>
namespace NetworkLayer {
    public interface IRoomEventHelper
    {
        void Register(string key, RoomEventRegistration act);
        void Unregister(string key);
    }

    public struct RoomEventRegistration
    {
        string key;

        /// <summary>
        /// How to Keep in room
        /// </summary>
        public EventCaching cachingOption;

        /// <summary>
        /// target
        /// </summary>
        public EventTarget receivers;

        public Action<object[]> onRoomEvent;
    }

    public enum EventTarget
    {
        /// <summary>Default value (not sent). Anyone else gets my event.</summary>
        Others = 0,

        /// <summary>Everyone in the current room (including this peer) will get this event.</summary>
        All = 1,

        /// <summary>The server sends this event only to the actor with the RoomOwner.</summary>
        RoomOwner = 2,
    }

    public enum EventCaching
    {
        /// <summary>Default value (not sent).</summary>
        DoNotCache = 0,
        /// <summary>Adds an event to the room's cache</summary>
        AddToRoomCache = 4,
        /// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
        AddToRoomCacheGlobal = 5,
        /// <summary>Remove fitting event from the room's cache.</summary>
        RemoveFromRoomCache = 6,
        /// <summary>Removes events of players who already left the room (cleaning up).</summary>
        RemoveFromRoomCacheForActorsLeft = 7,
    }
}
