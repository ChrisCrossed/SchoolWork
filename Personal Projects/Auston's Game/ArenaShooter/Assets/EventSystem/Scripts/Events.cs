namespace Events
{
    public static class GameOver
    {
        public delegate void SubscripionDelegate(int winningTeam);
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes: int winningTeam
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes: int winningTeam
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send(int winningTeam)
        {
            subscriptions.Invoke(winningTeam);
        }
    }
    public static class GameStarted
    {
        public delegate void SubscripionDelegate(int team);
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes: int team
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes: int team
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send(int team)
        {
            subscriptions.Invoke(team);
        }
    }
    public static class PlayerDied
    {
        public delegate void SubscripionDelegate(Player player, UnityEngine.GameObject killedBy);
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes: Player player, UnityEngine.GameObject killedBy
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes: Player player, UnityEngine.GameObject killedBy
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send(Player player, UnityEngine.GameObject killedBy)
        {
            subscriptions.Invoke(player, killedBy);
        }
    }
    public static class PlayerSpawned
    {
        public delegate void SubscripionDelegate(Player player);
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes: Player player
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes: Player player
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send(Player player)
        {
            subscriptions.Invoke(player);
        }
    }
    public static class ScoreChanged
    {
        public delegate void SubscripionDelegate(int team);
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes: int team
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes: int team
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send(int team)
        {
            subscriptions.Invoke(team);
        }
    }
    public static class TogglePause
    {
        public delegate void SubscripionDelegate();
        private static SubscripionDelegate subscriptions = delegate{ };

        /// <summary>
        /// Subscribes to this event so that when the event is sent the supplied function is called.
        /// Requires a function which takes no parameters.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Subscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions += _eventFunction;
        }
        /// <summary>
        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.
        /// Requires a function which takes no parameters.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction)
        {
            subscriptions -= _eventFunction;
        }
        /// <summary>
        /// Sends the event, calling all functions which are subscribe to this event.
        /// </summary>
        /// <param name="_eventFunction"></param>
        public static void Send()
        {
            subscriptions.Invoke();
        }
    }
}
namespace InputEvents
{
    public class SubscriptionData
    {
        public virtual void Send(InputEventInfo _inputEventInfo) { }
        public virtual void Send(InputEventInfo _inputEventInfo, int _channel) { }

        internal static bool ChannelIsInvalid(int _channel, int _channelSubscriptionsCount)
        {
            if (_channel < 0 || _channel > _channelSubscriptionsCount - 1)
            {
                UnityEngine.Debug.Log("Channel '" + _channel.ToString() + "' is not a valid channel.");
                return true;
            }
            return false;
        }
    }
    public class Aim : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class HowToPlay : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Grapple : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class LeftBasic : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class LeftSpecial : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class LockOn : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class MenuDirectional : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class MenuPrevious : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class MenuSelect : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Dash : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Jump : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Movement : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Restart : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class VerticalMovement : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Pause : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class HighPower : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class LowPower : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class RightBasic : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class RightSpecial : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
    public class Spectate : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] {  delegate { }, delegate { }, delegate { }, delegate { } }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name="_eventFunction">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name="_eventFunction">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name="_channel">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name="_inputEventInfo">The information that will be sent with the input event.</param>
        /// /// <param name="_channel">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } 
}
