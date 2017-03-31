namespace DragonSpeak.NET
{
    /// <returns>
    ///  If true, continue to the next <see cref="Trigger"/> in the paragraph.
    ///  If false, terminate the execution of the current paragraph.
    /// </returns>
    /// <param name="context"> The context object provided during the execution. </param>
    /// <param name="trigger"> The trigger currently executed in the trigger block. </param>
    public delegate bool TriggerHandler(IContext context, Trigger trigger);
}