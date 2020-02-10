namespace zPoolMiner.Utils
{
    public static class MinersDownloadManager
    {
        public static DownloadSetup StandardDlSetup = new DownloadSetup(
            "http://hash-kings.com/Downloads/Multipool_Miner/Bins/bin-3.0.0.0-Alpha3.zip",
            "bins.zip",
            "bin");

        public static DownloadSetup ThirdPartyDlSetup = new DownloadSetup(
            "http://hash-kings.com/Downloads/Multipool_Miner/Bins/bin_3rdparty-3.0.0.0-Alpha3.zip",
            "bins_3rdparty.zip",
            "bin_3rdparty");
    }
}