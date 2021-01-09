using BNC.TwitchApp;
using StardewValley;


namespace BNC.Actions
{
    class MessageAction : BaseAction
    {
        private readonly string _message;

        public MessageAction(string message)
        {
            _message = message;
        }

        public override ActionResponse Handle()
        {
            Game1.addHUDMessage(new HUDMessage(_message, null));
            return ActionResponse.Done;
        }
    }
}
