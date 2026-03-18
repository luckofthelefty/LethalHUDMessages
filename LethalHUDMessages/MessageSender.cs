using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace com.github.luckofthelefty.LethalHUDMessages;

internal enum MessageTier
{
    Death,
    Event
}

/// <summary>
/// Attached to the notification panel to override the Animator's color every LateUpdate.
/// </summary>
internal class NotifColorOverride : MonoBehaviour
{
    internal Image bgImage;
    internal TMPro.TMP_Text tmpText;
    internal Color bgColor;
    internal Color textColor;
    internal float remainingTime;

    #pragma warning disable IDE0051
    private void LateUpdate()
    #pragma warning restore IDE0051
    {
        if (remainingTime <= 0f)
        {
            Destroy(this);
            return;
        }
        remainingTime -= Time.deltaTime;

        if (bgImage != null)
            bgImage.color = bgColor;
        if (tmpText != null)
            tmpText.color = textColor;
    }
}

internal static class MessageSender
{
    private struct QueuedMessage
    {
        internal string text;
        internal MessageTier tier;
    }

    // Background colors
    private static readonly Color DeathBg = new Color(0.7f, 0.1f, 0.1f, 1f);   // red
    private static readonly Color EventBg = new Color(0.85f, 0.7f, 0f, 1f);    // yellow

    private const float MinDisplayTime = 4f;
    private static readonly Queue<QueuedMessage> _queue = new Queue<QueuedMessage>();
    private static float _lastShowTime = -999f;
    private static Coroutine _drainCoroutine;

    internal static void Send(string message, MessageTier tier = MessageTier.Death)
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;

        ShowTip(message, tier);
    }

    internal static void ShowTip(string message, MessageTier tier = MessageTier.Death)
    {
        if (HUDManager.Instance == null) return;

        float elapsed = Time.time - _lastShowTime;
        if (elapsed >= MinDisplayTime && _queue.Count == 0)
        {
            DisplayNow(message, tier);
        }
        else
        {
            _queue.Enqueue(new QueuedMessage { text = message, tier = tier });
            if (_drainCoroutine == null)
                _drainCoroutine = Plugin.Instance.StartCoroutine(DrainQueue());
        }
    }

    private static void DisplayNow(string message, MessageTier tier)
    {
        _lastShowTime = Time.time;

        if (ConfigManager.NotificationStyle.Value == DisplayStyle.GlobalNotification)
        {
            HUDManager.Instance.DisplayGlobalNotification(message);
            ApplyColorOverride(tier);
        }
        else
        {
            string header = tier == MessageTier.Death ? "DEATH" : "EVENT";
            bool isWarning = tier == MessageTier.Death;
            HUDManager.Instance.DisplayTip(header, message, isWarning);
        }
    }

    private static IEnumerator DrainQueue()
    {
        while (_queue.Count > 0)
        {
            float wait = MinDisplayTime - (Time.time - _lastShowTime);
            if (wait > 0f)
                yield return new WaitForSeconds(wait);

            if (_queue.Count > 0)
            {
                var next = _queue.Dequeue();
                DisplayNow(next.text, next.tier);
            }
        }
        _drainCoroutine = null;
    }

    private static void ApplyColorOverride(MessageTier tier)
    {
        var animator = HUDManager.Instance.globalNotificationAnimator;
        if (animator == null) return;

        // Find the Image on the animator's GameObject or any child
        var bgImage = animator.GetComponent<Image>();
        if (bgImage == null)
            bgImage = animator.GetComponentInChildren<Image>();

        // Find the TMP text
        var tmpText = HUDManager.Instance.globalNotificationText;

        // Attach or update our LateUpdate override component
        var overrider = animator.gameObject.GetComponent<NotifColorOverride>();
        if (overrider == null)
            overrider = animator.gameObject.AddComponent<NotifColorOverride>();

        overrider.bgImage = bgImage;
        overrider.tmpText = tmpText;
        overrider.bgColor = tier == MessageTier.Death ? DeathBg : EventBg;
        overrider.textColor = tier == MessageTier.Death ? Color.white : Color.black;
        overrider.remainingTime = 8f; // generous duration to outlast the full animation

        if (bgImage != null)
            Plugin.Log.LogInfo($"[GlobalNotif] Color override attached on '{bgImage.gameObject.name}'");
        else
            Plugin.Log.LogWarning($"[GlobalNotif] No Image found on animator or children. Animator GO: '{animator.gameObject.name}'");
    }
}
