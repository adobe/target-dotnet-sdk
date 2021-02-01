namespace Adobe.Target.Client.Util
{
    internal static class Constants
    {
        internal const string SdkNameHeader = "X-EXC-SDK";
        internal const string SdkNameValue = "AdobeTargetNet";
        internal const string SdkVersionHeader = "X-EXC-SDK-Version";
        internal static readonly string SdkUserAgent = $"{SdkNameValue}/{TargetConstants.SdkVersion}";
    }
}
