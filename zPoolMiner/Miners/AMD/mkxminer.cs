﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using zPoolMiner.Configs;
using zPoolMiner.Devices;
using zPoolMiner.Enums;
using zPoolMiner.Miners.Grouping;
using zPoolMiner.Miners.Parsing;

namespace zPoolMiner.Miners
{
    internal class Mkxminer : Miner
    {
#pragma warning disable CS0414 // The field 'Mkxminer.benchmarkTimeWait' is assigned but its value is never used
        private int benchmarkTimeWait = 11 * 60;
#pragma warning restore CS0414 // The field 'Mkxminer.benchmarkTimeWait' is assigned but its value is never used

        private readonly int GPUPlatformNumber;

        public Mkxminer() : base("mkxminer_AMD")

        {
            GPUPlatformNumber = ComputeDeviceManager.Avaliable.AMDOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;
        }

#pragma warning disable CS0108 // 'Mkxminer.BenchmarkException' hides inherited member 'Miner.BenchmarkException'. Use the new keyword if hiding was intended.
        private bool BenchmarkException
#pragma warning restore CS0108 // 'Mkxminer.BenchmarkException' hides inherited member 'Miner.BenchmarkException'. Use the new keyword if hiding was intended.
        {
            get
            {
                return MiningSetup.MinerPath == MinerPaths.Data.mkxminer;
            }
        }

        protected override int GET_MAX_CooldownTimeInMilliseconds()
        {
            if (this.MiningSetup.MinerPath == MinerPaths.Data.mkxminer)
            {
                return 60 * 1000 * 11; // wait wait for hashrate string
            }
            return 660 * 1000; // 11 minute max
        }

      
        public override void Start(string url, string btcAddress, string worker)
        {
            if (MiningSession.DONATION_SESSION)
            {
                if (url.Contains("zpool.ca"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";
                }
                if (url.Contains("ahashpool.com"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";

                }
                if (url.Contains("hashrefinery.com"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";

                }
                if (url.Contains("nicehash.com"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";

                }
                if (url.Contains("zergpool.com"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";

                }
                if (url.Contains("blockmasters.co"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";

                }
                if (url.Contains("blazepool.com"))
                {
                    btcAddress = Globals.DemoUser;
                    worker = "c=BTC,ID=Donation";
                }
                if (url.Contains("miningpoolhub.com"))
                {
                    btcAddress = "cryptominer.Devfee";
                    worker = "x";
                }
                else
                {
                    btcAddress = Globals.DemoUser;
                }
            }
            else
            {
                if (url.Contains("zpool.ca"))
                {
                    btcAddress = zPoolMiner.Globals.GetzpoolUser();
                    worker = zPoolMiner.Globals.GetzpoolWorker();
                }
                if (url.Contains("ahashpool.com"))
                {
                    btcAddress = zPoolMiner.Globals.GetahashUser();
                    worker = zPoolMiner.Globals.GetahashWorker();

                }
                if (url.Contains("hashrefinery.com"))
                {
                    btcAddress = zPoolMiner.Globals.GethashrefineryUser();
                    worker = zPoolMiner.Globals.GethashrefineryWorker();

                }
                if (url.Contains("nicehash.com"))
                {
                    btcAddress = zPoolMiner.Globals.GetnicehashUser();
                    worker = zPoolMiner.Globals.GetnicehashWorker();

                }
                if (url.Contains("zergpool.com"))
                {
                    btcAddress = zPoolMiner.Globals.GetzergUser();
                    worker = zPoolMiner.Globals.GetzergWorker();

                }
                if (url.Contains("minemoney.co"))
                {
                    btcAddress = zPoolMiner.Globals.GetminemoneyUser();
                    worker = zPoolMiner.Globals.GetminemoneyWorker();

                }
                if (url.Contains("blazepool.com"))
                {
                    btcAddress = zPoolMiner.Globals.GetblazepoolUser();
                    worker = zPoolMiner.Globals.GetblazepoolWorker();
                }
                if (url.Contains("blockmasters.co"))
                {
                    btcAddress = zPoolMiner.Globals.GetblockmunchUser();
                    worker = zPoolMiner.Globals.GetblockmunchWorker();
                }
                if (url.Contains("miningpoolhub.com"))
                {
                    btcAddress = zPoolMiner.Globals.GetMPHUser();
                    worker = zPoolMiner.Globals.GetMPHWorker();
                }
            }
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTAG(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAddress, worker);

            LastCommandLine = " --url " + url +
                                 " --user " + btcAddress +
                         " -p " + worker + ",m=party.NPlusMiner" + "-I 23 " +
                                 ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                                 " --device ";
            LastCommandLine += GetDevicesCommandString();
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        // new decoupled benchmarking routines

        #region Decoupled benchmarking routines


        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string url = Globals.GetLocationURL(algorithm.CryptoMiner937ID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);

            string username = Globals.DemoUser;

            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();

            string CommandLine = " --url " + url + " --user " + Globals.DemoUser + " -p Benchmark" + ",m=party.NPlusMiner" + " -I 23" +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                                  " --device ";
            CommandLine += GetDevicesCommandString();

            Helpers.ConsolePrint(MinerTAG(), CommandLine);

            return CommandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)

        {
            Helpers.ConsolePrint(MinerTAG(), outdata);
            if (BenchmarkException)
            {
                if (outdata.Contains("> "))
                {
                    int st = outdata.IndexOf("> ");
                    int end = outdata.IndexOf("MH/s");
                    //      int len = outdata.Length - speedLength - st;

                    //          string parse = outdata.Substring(st, len-1).Trim();
                    //          double tmp = 0;
                    //          Double.TryParse(parse, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp);

                    // save speed
                    //       int i = outdata.IndexOf("Benchmark:");
                    //       int k = outdata.IndexOf("/s");
                    string hashspeed = outdata.Substring(st + 6, end - st - 6);
                    /*
                    int b = hashspeed.IndexOf(" ");
                       if (hashspeed.Contains("k"))
                           tmp *= 1000;
                       else if (hashspeed.Contains("m"))
                           tmp *= 1000000;
                       else if (hashspeed.Contains("g"))
                           tmp *= 1000000000;
                   }
                   */

                    double speed = Double.Parse(hashspeed, CultureInfo.InvariantCulture);
                    BenchmarkAlgorithm.BenchmarkSpeed = speed * 1000;
                    BenchmarkSignalFinnished = true;
                }
            }
            return false;
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }

        #endregion Decoupled benchmarking routines

        // TODO _currentMinerReadStatus
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public override async Task<APIData> GetSummaryAsync()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            // cryptonight does not have api bind port
            APIData mkxminerData = new APIData(MiningSetup.CurrentAlgorithmType)
            {
                Speed = 0
            };
            if (IsAPIReadException)
            {
                // check if running
                if (ProcessHandle == null)
                {
                    //_currentMinerReadStatus = MinerAPIReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTAG(), ProcessTag() + " Could not read data from mkxminer Proccess is null");
                    return null;
                }
                try
                {
                    var runningProcess = Process.GetProcessById(ProcessHandle.Id);
                }
                catch (ArgumentException ex)
                {
                    //_currentMinerReadStatus = MinerAPIReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTAG(), ProcessTag() + " Could not read data from mkxminer reason: " + ex.Message);
                    return null; // will restart outside
                }
                catch (InvalidOperationException ex)
                {
                    //_currentMinerReadStatus = MinerAPIReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTAG(), ProcessTag() + " Could not read data from mkxminer reason: " + ex.Message);
                    return null; // will restart outside
                }

                var totalSpeed = 0.0d;
                foreach (var miningPair in MiningSetup.MiningPairs)
                {
                    /*var algo = miningPair.Device.GetAlgorithm(MinerBaseType.mkxminer, AlgorithmType.Lyra2REv2, AlgorithmType.NONE);
                    if (algo != null)
                    {
                        totalSpeed += algo.BenchmarkSpeed;
                        Helpers.ConsolePrint(MinerTAG(), ProcessTag() + " Could not read data from mkxminer. Used benchmark hashrate");
                    }*/
                }

                mkxminerData.Speed = totalSpeed;
                return mkxminerData;
            }

            //  return await GetSummaryCPU_mkxminerAsync();
            return mkxminerData;
        }
    }
}