using System;

public interface INpcTtsPlaybackAware
{
    event Action PlaybackStarted;
    event Action<string> PlaybackFailed;
}
