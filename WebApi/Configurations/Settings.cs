namespace RocketStoreApi.Configurations
{
    // DI needs a parameterless constructor to initialize,
    // so needed to define a record with init-only properties as AppSettings is injected in builder
    public record AppSettings()
    {
        public LoggingSettings Logging { get; init; } = default!;
        public PositionStackSettings PositionStack { get; init; } = default!;
    }
    public record LoggingSettings(LogLevelSettings LogLevel) { }
    public record LogLevelSettings(string Default = "Information") { }
    public record PositionStackSettings(string AccessKey, string Url) { }
}
