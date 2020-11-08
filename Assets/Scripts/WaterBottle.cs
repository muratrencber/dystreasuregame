public class WaterBottle : IInteractableMono
{
    public override void OnInteract()
    {
        ProgressManager.AddWater();
        Destroy(gameObject);
    }
}
