namespace DragonSpeak.NET
{
    public enum TriggerCategory
    {
        /// <summary>
        /// A trigger defined with a 0
        /// <para>Example: (0:1) when someone says something, </para>
        /// </summary>
        Cause = 0,
        /// <summary>
        /// A trigger defined with a 1
        /// <para>Example: (1:1) and they moved # units left, </para>
        /// </summary>
        Condition = 1,
        /// <summary>
        /// A trigger defined with a 3
        /// <para>Example: (3:1) at position (#,#) on the map,</para>
        /// </summary>
        Area = 3,
        /// <summary>
        /// A trigger defined with a 4
        /// <para>Example: (4:1) only where there are trees,</para>
        /// </summary>
        Filter = 4,
        /// <summary>
        /// A trigger defined with a 5
        /// <para>Example: (5:1) send a message in the chat saying {...}. </para>
        /// </summary>
        Effect = 5,
        /// <summary>
        /// A trigger that was not defined. You should never encounter this
        /// if you do then something isn't quite right.
        /// </summary>
        Undefined = -1
    }
}