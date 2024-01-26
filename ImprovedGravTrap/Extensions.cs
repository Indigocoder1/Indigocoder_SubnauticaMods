namespace ImprovedGravTrap
{
    public static class Extensions
    {
        public static bool IsEnhancedGravTrap(this TechType techType) => techType == ImprovedTrap_Craftable.techType;
    }
}
