using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class ServerInfo
    {
        public bool IsMaster
        {
            get
            {
                return Replication != null
                    && !string.IsNullOrWhiteSpace(Replication.Role)
                    && Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase);
            }
        }
        public bool IsSlave
        {
            get
            {
                return !IsMaster;
            }
        }
        public string Title { get; set; }
        public Server Server { get; set; }
        public Memory Memory { get; set; }
        public Clients Clients { get; set; }
        public Stats Stats { get; set; }
        public Replication Replication { get; set; }
        public CPU CPU { get; set; }
        public Cluster Cluster { get; set; }
        public List<Keyspace> Keyspace { get; set; }
        
        public ServerInfo(string title, List<IGrouping<string, KeyValuePair<string, string>>> info)
        {
            this.Title = title;

            if (info == null || !info.Any())
                return;

            Keyspace = new List<Models.Keyspace>();

            foreach (var group in info)
            {
                switch (group.Key.ToLower())
                {
                    case "server":
                        Server = new Server(group.Select(x => x).ToList());
                        break;
                    case "clients":
                        Clients = new Clients(group.Select(x => x).ToList());
                        break;
                    case "memory":
                        Memory = new Memory(group.Select(x => x).ToList());
                        break;
                    case "stats":
                        Stats = new Stats(group.Select(x => x).ToList());
                        break;
                    case "replication":
                        Replication = new Replication(group.Select(x => x).ToList());
                        break;
                    case "cpu":
                        CPU = new CPU(group.Select(x => x).ToList());
                        break;
                    case "cluster":
                        Cluster = new Cluster(group.Select(x => x).ToList());
                        break;
                    case "keyspace":
                        Keyspace = Models.Keyspace.Create(group.Select(x => x).ToList());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Server
    {
        public string Version { get; set; }
        public string Mode { get; set; }
        public string OS { get; set; }
        public string ProcessId { get; set; }
        public long UptimeInSeconds { get; set; }
        public long UptimeInDays { get; set; }

        public Server(List<KeyValuePair<string, string>> values)
        {
            long l;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "redis_version":
                        Version = value.Value;
                        break;
                    case "redis_mode":
                        Mode = value.Value;
                        break;
                    case "os":
                        OS = value.Value;
                        break;
                    case "process_id":
                        ProcessId = value.Value;
                        break;
                    case "uptime_in_seconds":
                        if (long.TryParse(value.Value, out l))
                            UptimeInSeconds = l;
                        break;
                    case "uptime_in_days":
                        if (long.TryParse(value.Value, out l))
                            UptimeInDays = l;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Memory
    {
        public long UsedMemory { get; set; }
        public string UsedMemoryHuman { get; set; }
        public string UsedMemoryPeakHuman { get; set; }
        public decimal FragmentationRatio { get; set; }

        public Memory(List<KeyValuePair<string, string>> values)
        {
            long l;
            decimal d;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "used_memory":
                        if (long.TryParse(value.Value, out l))
                            UsedMemory = l;
                        break;
                    case "used_memory_human":
                        UsedMemoryHuman = value.Value;
                        break;
                    case "used_memory_peak_human":
                        UsedMemoryPeakHuman = value.Value;
                        break;
                    case "mem_fragmentation_ratio":
                        if (decimal.TryParse(value.Value, out d))
                            FragmentationRatio = d;
                        break;                    
                    default:
                        break;
                }
            }
        }
    }

    public class Clients
    {
        public long ConnectedClients { get; set; }
        public long BlockedClients { get; set; }

        public Clients(List<KeyValuePair<string, string>> values)
        {
            long l;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "connected_clients":
                        if (long.TryParse(value.Value, out l))
                            ConnectedClients = l;
                        break;
                    case "blocked_clients":
                        if (long.TryParse(value.Value, out l))
                            BlockedClients = l;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Stats
    {
        public long TotalConnectionsReceived { get; set; }
        public long TotalCommandsProcessed { get; set; }
        public long InstantaneousOpsPerSec { get; set; }
        public long TotalNetInputBytes { get; set; }
        public long TotalNetOutputBytes { get; set; }
        public decimal InstantaneousInputKbps { get; set; }
        public decimal InstantaneousOutputKbps { get; set; }
        public long RejectedConnections { get; set; }
        public long SyncFull { get; set; }
        public long SyncPartialOk { get; set; }
        public long SyncPartialErr { get; set; }
        public long ExpiredKeys { get; set; }
        public long EvictedKeys { get; set; }
        public long KeyspaceHits { get; set; }
        public long KeyspaceMisses { get; set; }
        public long PubsubChannels { get; set; }
        public long PubsubPatterns { get; set; }
        public long LatestForkUsec { get; set; }

        public Stats(List<KeyValuePair<string, string>> values)
        {
            long l;
            decimal d;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "total_connections_received":
                        if (long.TryParse(value.Value, out l))
                            TotalConnectionsReceived = l;
                        break;
                    case "total_commands_processed":
                        if (long.TryParse(value.Value, out l))
                            TotalCommandsProcessed = l;
                        break;
                    case "instantaneous_ops_per_sec":
                        if (long.TryParse(value.Value, out l))
                            InstantaneousOpsPerSec = l;
                        break;
                    case "total_net_input_bytes":
                        if (long.TryParse(value.Value, out l))
                            TotalNetInputBytes = l;
                        break;
                    case "total_net_output_bytes":
                        if (long.TryParse(value.Value, out l))
                            TotalNetOutputBytes = l;
                        break;
                    case "instantaneous_input_kbps":
                        if (decimal.TryParse(value.Value, out d))
                            InstantaneousInputKbps = d;
                        break;
                    case "instantaneous_output_kbps":
                        if (decimal.TryParse(value.Value, out d))
                            InstantaneousOutputKbps = d;
                        break;
                    case "rejected_connections":
                        if (long.TryParse(value.Value, out l))
                            RejectedConnections = l;
                        break;
                    case "sync_full":
                        if (long.TryParse(value.Value, out l))
                            SyncFull = l;
                        break;
                    case "sync_partial_ok":
                        if (long.TryParse(value.Value, out l))
                            SyncPartialOk = l;
                        break;
                    case "sync_partial_err":
                        if (long.TryParse(value.Value, out l))
                            SyncPartialErr = l;
                        break;
                    case "expired_keys":
                        if (long.TryParse(value.Value, out l))
                            ExpiredKeys = l;
                        break;
                    case "evicted_keys":
                        if (long.TryParse(value.Value, out l))
                            EvictedKeys = l;
                        break;
                    case "keyspace_hits":
                        if (long.TryParse(value.Value, out l))
                            KeyspaceHits = l;
                        break;
                    case "keyspace_misses":
                        if (long.TryParse(value.Value, out l))
                            KeyspaceMisses = l;
                        break;
                    case "pubsub_channels":
                        if (long.TryParse(value.Value, out l))
                            PubsubChannels = l;
                        break;
                    case "pubsub_patterns":
                        if (long.TryParse(value.Value, out l))
                            PubsubPatterns = l;
                        break;
                    case "latest_fork_usec":
                        if (long.TryParse(value.Value, out l))
                            LatestForkUsec = l;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public class Replication
    {
        public string Role { get; set; }
        public long ConnectedSlaves { get; set; }
        public long ReplBacklogActive { get; set; }
        public long ReplBacklogSize { get; set; }
        public long ReplBacklogFirstByteOffset { get; set; }
        public long ReplBacklogHistlen { get; set; }

        public Replication(List<KeyValuePair<string, string>> values)
        {
            long l;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "role":
                        Role = value.Value;
                        break;
                    case "connected_slaves":
                        if (long.TryParse(value.Value, out l))
                            ConnectedSlaves = l;
                        break;
                    case "repl_backlog_active":
                        if (long.TryParse(value.Value, out l))
                            ReplBacklogActive = l;
                        break;
                    case "repl_backlog_size":
                        if (long.TryParse(value.Value, out l))
                            ReplBacklogSize = l;
                        break;
                    case "repl_backlog_first_byte_offset":
                        if (long.TryParse(value.Value, out l))
                            ReplBacklogFirstByteOffset = l;
                        break;
                    case "repl_backlog_histlen":
                        if (long.TryParse(value.Value, out l))
                            ReplBacklogHistlen = l;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class CPU
    {
        public decimal UsedCpuSys { get; set; }
        public decimal UsedCpuUser { get; set; }

        public CPU(List<KeyValuePair<string, string>> values)
        {
            decimal d;
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "used_cpu_sys":
                        if (decimal.TryParse(value.Value, out d))
                            UsedCpuSys = d;
                        break;
                    case "used_cpu_user":
                        if (decimal.TryParse(value.Value, out d))
                            UsedCpuUser = d;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Cluster
    {
        public string Enabled { get; set; }

        public Cluster(List<KeyValuePair<string, string>> values)
        {
            foreach (var value in values)
            {
                switch (value.Key.ToLower())
                {
                    case "cluster_enabled":
                        Enabled = value.Value;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Keyspace
    {
        public string Database { get; set; }
        public long Keys { get; set; }
        public long Expires { get; set; }
        public decimal AvgTTL { get; set; }

        public static List<Keyspace> Create(List<KeyValuePair<string, string>> values)
        {
            List<Keyspace> list = new List<Keyspace>();
            long l;
            decimal d;
            foreach (var value in values)
            {
                var keyspace = new Keyspace();
                keyspace.Database = value.Key;

                var arr = value.Value.Split(',');
                if (arr != null && arr.Length == 3)
                {
                    if (long.TryParse(arr[0].Split('=')[1], out l))
                        keyspace.Keys = l;
                    if (long.TryParse(arr[1].Split('=')[1], out l))
                        keyspace.Expires = l;
                    if (decimal.TryParse(arr[2].Split('=')[1], out d))
                        keyspace.AvgTTL = d;
                }
                list.Add(keyspace);
            }
            return list;
        }
    }
}