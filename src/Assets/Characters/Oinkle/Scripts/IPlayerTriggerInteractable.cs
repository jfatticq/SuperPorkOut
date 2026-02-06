namespace Assets.Characters.Player.Scripts
{
    internal interface IPlayerTriggerInteractable
    {
        void OnPlayerEnter(PlayerFacade player);   // called when player enters trigger

        void OnPlayerExit(PlayerFacade player);    // optional (zones use it)
    }
}
