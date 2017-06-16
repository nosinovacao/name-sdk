namespace NAME.Core
{
    /// <summary>
    /// Represents the supported NAME behaviours
    /// </summary>
    public enum SupportedNAMEBehaviours
    {
        /// <summary>
        /// The standard behaviour. Provides full NAME functionality.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// The reporting disabled behaviour. Provides the funcionationalilies in <see cref="Standard"/> EXCEPT reporting the manifest to registry.
        /// </summary>
        AnnounceDisabled = 1,

        /// <summary>
        /// The heartbeat disabled behaviour. Will register it self with the Registry on application startup but will not heartbeat it's status.
        /// </summary>
        HeartbeatDisabled = 2,

        /// <summary>
        /// The announce disabled behaviour. Will not announce it self with the Registry.
        /// </summary>
        BootstrapDisabled = 3,

        /// <summary>
        /// The name disabled behaviour. Will not provide any NAME functionality.
        /// </summary>
        NAMEDisabled = 4
    }
}
