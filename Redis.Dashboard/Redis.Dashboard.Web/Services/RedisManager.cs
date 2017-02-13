using Redis.Dashboard.Web.Models;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Redis.Dashboard.Web.Services
{
    public class RedisManager: IDisposable
    {
        private static ConcurrentDictionary<string, ConnectionMultiplexer> Connections;

        static RedisManager()
        {
            Connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        public void ReadServerInfo(string key, string connString)
        {
            try
            {
                if (!Connections.ContainsKey(key))
                {
                    var redis = ConnectionMultiplexer.Connect(connString);
                    Connections.TryAdd(key, redis);
                }
            }
            catch (Exception e)
            {

            }
        }

        public List<ServerInfo> GetServerInfo(string key)
        {
            try
            {
                List<ServerInfo> list = new List<ServerInfo>();
                if (Connections.ContainsKey(key))
                {
                    var redis = Connections[key];
                    var endpoints = redis.GetEndPoints();
                    Parallel.ForEach(endpoints, (endpoint) => 
                    {
                        var x = redis.GetServer(endpoint).Info();
                        var y = new ServerInfo(endpoint.ToString(), x.ToList());
                        list.Add(y);
                    });
                }

                return list.OrderBy(l => l.Replication.Role).ThenBy(l => l.Title).ToList();
            }
            catch (Exception e)
            {

            }
            return null;
        }

        // Dispose() calls Dispose(true)  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // NOTE: Leave out the finalizer altogether if this class doesn't   
        // own unmanaged resources itself, but leave the other methods  
        // exactly as they are.   
        ~RedisManager()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                if (Connections != null)
                {
                    foreach (var conn in Connections.Values)
                    {
                        conn.Dispose();
                    }
                    Connections.Clear();
                    Connections = null;
                }
            }
        }

    }
}