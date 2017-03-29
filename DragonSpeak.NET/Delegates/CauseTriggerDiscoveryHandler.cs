namespace DragonSpeak.NET.Delegates
{
    /// <summary>
    /// Whenever a <see cref="TriggerCategory.Cause"/> trigger is found during parsing.
    /// </summary>
    /// <param name="sender"> The page the trigger was found in. </param>
    /// <param name="trigger"> The <see cref="Trigger.Cause"/> trigger found. </param>
    public delegate void CauseTriggerDiscoveryHandler(Page sender, Trigger trigger);
}
