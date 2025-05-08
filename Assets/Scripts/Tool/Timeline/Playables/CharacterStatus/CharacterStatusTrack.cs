using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.CharacterStatus
{
    [TrackClipType(typeof(CharacterStatusGhostClip))]
    [TrackClipType(typeof(CharacterStatusSuperArmorClip))]
    [TrackClipType(typeof(CharacterStatusIntangibleClip))]
    [TrackClipType(typeof(CharacterStatusInvisibleClip))]
    [TrackClipType(typeof(CharacterStatusParryingClip))]
    public class CharacterStatusTrack : TrackAsset
    {

    }
}