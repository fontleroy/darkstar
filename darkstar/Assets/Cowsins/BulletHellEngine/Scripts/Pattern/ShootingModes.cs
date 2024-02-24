namespace cowsins.BulletHell
{
    /// <summary>
    /// Static class used by the PatternSpawner to define the shooting mode when shooting certain pattern. 
    /// The Shooting Method is defined in an enumerator called Mode.
    /// </summary>
    public static class ShootingModes
    {
        public enum Mode
        {
            //Allows you to set a specific number of shots
            DefinedAmount,
            //Used to stablish continuous shooting depending on your pattern fire rate.
            //This will be continuous until you call StopShooting() on your PatternSpawner or call a new
            //Shoot() function on your PatternSpawner
            Continuous
        };       
    }

}