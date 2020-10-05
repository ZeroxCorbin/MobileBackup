using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;

namespace Classes
{
    //Usage: ping [-t] [-a] [-n count] [-l size] [-f] [-i TTL] [-v TOS] [-r count] [-s count] [[-j host-list] | [-k host-list]] [-w timeout] [-R] [-S srcaddr] [-4] [-6 target_name]

    public class PingCommandLine
    {

        public string GetCommandLineArguments()
        {
            StringBuilder args = new StringBuilder();

            if (Continuous) args.Append(Continuous_Command);
            else args.Append(EchoRequestCount_Command);
            args.Append(ResolveAddressToHostname_Command);
            args.Append(EchoRequestPacketSize_Command);
            args.Append(ICMPNoFragment_Command);
            args.Append(TimeToLive_Command);
            args.Append(NumberOfHops_Command);
            args.Append(InternetTimestampCount_Command);
            args.Append(EchoTimeout_Command);
            args.Append(TraceRoundTripPath_Command);
            args.Append(SourceAddress_Command);
            args.Append(IPV4Only_Command);
            args.Append(IPV6Only_Command);
            args.Append(Target_Command);

            return args.ToString();
        }

        public void Run(string target)
        {
            Target = target;
            System.Diagnostics.Process.Start("cmd", "/K \"echo ping " + GetCommandLineArguments() + " & ping " + GetCommandLineArguments() + "\"");
        }

        public string Target { get; set; } = string.Empty;
        public string Target_Command
        {
            get { if (!string.IsNullOrEmpty(Target)) return Target; else return string.Empty; }
        }

        public bool Continuous { get; set; } = false;
        public string Continuous_Command
        {
            get { if (Continuous) return "-t "; else return string.Empty; }
        }

        public bool ResolveAddressToHostname { get; set; } = false;
        public string ResolveAddressToHostname_Command
        {
            get { if (ResolveAddressToHostname) return "-a "; else return string.Empty; }
        }


        private uint _EchoRequestCount = 4;
        public uint EchoRequestCount
        {
            get
            {
                return _EchoRequestCount;
            }
            set
            {
                if (value > 4294967295) _EchoRequestCount = 4294967295;
                else _EchoRequestCount = value;
            }
        }
        public string EchoRequestCount_Command
        {
            get { if (_EchoRequestCount > 0) return "-n " + _EchoRequestCount.ToString() + " "; else return string.Empty; }
        }


        private ushort _EchoRequestPacketSize = 32;
        public ushort EchoRequestPacketSize
        {
            get
            {
                return _EchoRequestPacketSize;
            }
            set
            {
                if(value > 65527) _EchoRequestPacketSize = 65527;
                else _EchoRequestPacketSize = value;
            }
        }
        public string EchoRequestPacketSize_Command
        {
            get { if (_EchoRequestPacketSize > 0) return "-l " + _EchoRequestPacketSize.ToString() + " "; else return string.Empty; }
        }

        public bool ICMPNoFragment { get; set; } = false;
        public string ICMPNoFragment_Command
        {
            get { if (ICMPNoFragment) return "-f "; else return string.Empty; }
        }

        public byte TimeToLive { get; set; } = 32;
        public string TimeToLive_Command
        {
            get { if (TimeToLive > 0) return "-i " + TimeToLive.ToString() + " "; else return string.Empty; }
        }


        public byte NumberOfHops { get; set; } = 0;
        public string NumberOfHops_Command
        {
            get { if (NumberOfHops > 0) return "-r " + NumberOfHops.ToString() + " "; else return string.Empty; }
        }


        private byte _InternetTimestampCount = 0;
        public byte InternetTimestampCount
        {
            get
            {
                return _InternetTimestampCount;
            }
            set
            {
                if (value > 4) _InternetTimestampCount = 4;
                else _InternetTimestampCount = value;
            }
        }
        public string InternetTimestampCount_Command
        {
            get { if (_InternetTimestampCount > 0) return "-s " + _InternetTimestampCount.ToString() + " "; else return string.Empty; }
        }


        private uint _EchoTimeout = 4000;
        public uint EchoTimeout
        {
            get
            {
                return _EchoTimeout;
            }
            set
            {
                if (value > 4294967295) _EchoTimeout = 4294967295;
                else _EchoTimeout = value;
            }
        }
        public string EchoTimeout_Command
        {
            get { if (_EchoTimeout > 0) return "-w " + _EchoTimeout.ToString() + " "; else return string.Empty; }
        }

        public bool TraceRoundTripPath { get; set; } = false;
        public string TraceRoundTripPath_Command
        {
            get { if (TraceRoundTripPath) return "-R "; else return string.Empty; }
        }

        public string SourceAddress { get; set; } = string.Empty;
        public string SourceAddress_Command
        {
            get { if (!string.IsNullOrEmpty(SourceAddress)) return "-S " + SourceAddress + " "; else return string.Empty; }
        }

        public bool IPV4Only { get; set; } = false;
        public string IPV4Only_Command
        {
            get { if (IPV4Only) return "-4 "; else return string.Empty; }
        }

        public bool IPV6Only { get; set; } = false;
        public string IPV6Only_Command
        {
            get { if (IPV6Only) return "-6 "; else return string.Empty; }
        }


    }

    public class PingAsync
    {
        public void Ping(string[] args)
        {
            if(args.Length == 0)
                throw new ArgumentException("Ping needs a host or IP Address.");

            string who = args[0];
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 12 seconds for a reply.
            int timeout = 12000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            Console.WriteLine("Time to live: {0}", options.Ttl);
            Console.WriteLine("Don't fragment: {0}", options.DontFragment);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do something useful
            // when possible.
            waiter.WaitOne();
            Console.WriteLine("Ping example completed.");
        }

        private static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if(e.Cancelled)
            {
                Console.WriteLine("Ping canceled.");

                // Let the main thread resume.
                // UserToken is the AutoResetEvent object that the main thread
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if(e.Error != null)
            {
                Console.WriteLine("Ping failed:");
                Console.WriteLine(e.Error.ToString());

                // Let the main thread resume.
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        public static void DisplayReply(PingReply reply)
        {
            if(reply == null)
                return;

            Console.WriteLine("ping status: {0}", reply.Status);
            if(reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
        }
    }
}
