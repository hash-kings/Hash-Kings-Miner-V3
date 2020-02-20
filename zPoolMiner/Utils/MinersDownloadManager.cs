namespace zPoolMiner.Utils
{
    public static class MinersDownloadManager
    {
        public static DownloadSetup AMD_Miners = new DownloadSetup(
            "https://bitbucket.org/hashkings/binz/downloads/AMD_Miners_3.0.1.1.zip",
            "AMD_Miners.zip",
            "AMD_Miners");
        public static DownloadSetup NVIDIA_Miners = new DownloadSetup(
            "https://bitbucket.org/hashkings/binz/downloads/NVIDIA_Miners_3.0.1.1.zip",
            "NVIDIA_Miners.zip",
            "NVIDIA_Miners");
        public static DownloadSetup AMD_NVIDIA_Miners = new DownloadSetup(
            "https://bitbucket.org/hashkings/binz/downloads/AMD_NVIDIA_Miners_3.0.1.1.zip",
            "AMD_NVIDIA_Miners.zip",
            "AMD_NVIDIA_Miners");
        public static DownloadSetup CPU_Miners = new DownloadSetup(
            "https://bitbucket.org/hashkings/binz/downloads/CPU_Miners_3.0.1.1.zip",
            "CPU_Miners.zip",
            "CPU_Miners");






        /*Samples
        
        public static DownloadSetup name = new DownloadSetup(
            "URLHERE",
            "name.zip",
            "bin/NVIDIA");
            */
    }
}
 