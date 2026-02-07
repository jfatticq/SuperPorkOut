namespace SuperPorkOut.Characters.Player
{
    public interface IPlayerCollisionInteractable
    {
        void OnPlayerCollision(PlayerFacade player, CollisionInfo hit);
    }
}
