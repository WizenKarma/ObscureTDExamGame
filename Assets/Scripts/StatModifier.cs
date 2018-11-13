namespace Keith.EnemyStats{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300,
    }

    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType Type;
        public readonly int Order;
        public readonly object Source;
        public readonly float Mod;

        public StatModifier(float value, StatModType type, int order, object source, float mod)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
            Mod = mod;
        }

        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null, 0f) { }
        public StatModifier(float value, StatModType type, int order) : this(value, type, order, null, 0f) { }
        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source, 0f) { }
        public StatModifier(float value, StatModType type, float mod) : this(value, type, (int)type, null, (float)mod) { }
    }
}


