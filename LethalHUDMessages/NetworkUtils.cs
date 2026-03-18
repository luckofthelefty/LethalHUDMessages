using System.Collections.Generic;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalHUDMessages;

internal static class NetworkUtils
{
    private static readonly Dictionary<string, int> _lastProcessedFrame = new Dictionary<string, int>();

    public static bool ShouldProcess(string eventKey)
    {
        int frame = UnityEngine.Time.frameCount;
        if (_lastProcessedFrame.TryGetValue(eventKey, out int lastFrame) && lastFrame == frame)
            return false;

        _lastProcessedFrame[eventKey] = frame;
        return true;
    }
}
