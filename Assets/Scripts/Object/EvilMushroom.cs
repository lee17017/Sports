public class EvilMushroom : BaseObject {
    protected override void NotifyPlayerCollision(Player player) {
        player.Damage(1);
    }
}
