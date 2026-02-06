namespace Characters.Player
{
    internal interface IPlayerCollisionInteractable
    {
        void OnPlayerCollision(PlayerFacade player, CollisionInfo hit);
    }
}
