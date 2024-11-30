namespace RocketStoreApi.Tests.Configurations
{
    public abstract class TestCaseBase
    {
        public string TestDescription { get; set; } = string.Empty;

        // Override ToString to return the TestDescription 
        public override string ToString() => TestDescription;
    }
}
