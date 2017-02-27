using Newtonsoft.Json;
using Redis.Dashboard.Web.Models;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Redis.Dashboard.Web.Services
{
    public class RedisManager: IDisposable
    {
        private static ConcurrentDictionary<string, ConnectionMultiplexer> Connections;
        private static ConcurrentDictionary<string, IServer> Servers;
        private static ConcurrentDictionary<string, LoadedLuaScript> LoadedLuaScriptByServer;

        static RedisManager()
        {
            Connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
            Servers = new ConcurrentDictionary<string, IServer>();
            LoadedLuaScriptByServer = new ConcurrentDictionary<string, LoadedLuaScript>();
        }


        private static void CreateConnectionToServers(string key, string connString)
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
                Log.Error(e, "CreateConnectionToServers for {key} - {connString}", key, connString);
            }
        }
        private static IServer GetServer(ConnectionMultiplexer redis, EndPoint endpoint)
        {
            var key = endpoint.ToString();
            IServer server;
            if (Servers.TryGetValue(key, out server))
                return server;

            server = redis.GetServer(endpoint);
            Servers.TryAdd(key, server);
            return server;
        }
        private static async Task<LoadedLuaScript> GetLoadedLuaScript(ConnectionMultiplexer redis, IServer server)
        {
            #region search lua script
            const string searchLuaScript = @"
local limit = tonumber(@rows)
local pattern = @keyPattern
local cursor = 0
local len = 0
local keys = {}

repeat
    local r = redis.call('scan', cursor, 'MATCH', pattern, 'COUNT', limit)
    cursor = tonumber(r[1])
    for k,v in ipairs(r[2]) do
        table.insert(keys, v)
        len = len + 1
        if len == limit then break end
    end
until cursor == 0 or len == limit

if len == 0 then
    return '[]'
end

local keyAttrs = {}
for i,key in ipairs(keys) do
    local type = redis.call('type', key)['ok']
    local pttl = redis.call('pttl', key)
    local size = 0
    if type == 'string' then
        size = redis.call('strlen', key)
    elseif type == 'list' then
        size = redis.call('llen', key)
    elseif type == 'set' then
        size = redis.call('scard', key)
    elseif type == 'zset' then
        size = redis.call('zcard', key)
    elseif type == 'hash' then
        size = redis.call('hlen', key)
    end

    local attrs = {['key'] = key, ['type'] = type, ['ttl'] = pttl, ['size'] = size}

    table.insert(keyAttrs, attrs)    
end

return cjson.encode(keyAttrs)";
            #endregion
            var key = string.Format("search_{0}", server.EndPoint.ToString());
            LoadedLuaScript loaded = null;
            if (LoadedLuaScriptByServer.TryGetValue(key, out loaded))
                return loaded;

            if (!await server.ScriptExistsAsync(searchLuaScript))
            {
                var prepared = LuaScript.Prepare(searchLuaScript);
                loaded = await prepared.LoadAsync(server);
                LoadedLuaScriptByServer.TryAdd(key, loaded);
            }
            return loaded;
        }

        public static async Task<List<ServerInfo>> GetRedisServersInfo(string key, string connString)
        {
            try
            {
                CreateConnectionToServers(key, connString);

                List<ServerInfo> list = new List<ServerInfo>();
                if (Connections.ContainsKey(key))
                {
                    var redis = Connections[key];
                    var endpoints = redis.GetEndPoints();

                    var tasks = endpoints.Select(e => GetServerInfo(redis, e, list));
                    await Task.WhenAll(tasks.ToArray());
                }

                return list.OrderBy(l => l.Replication.Role).ThenBy(l => l.Title).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, "GetRedisServersInfo for {key} - {connString}", key, connString);
            }
            return null;
        }

        private static async Task GetServerInfo(ConnectionMultiplexer redis, EndPoint endpoint, List<ServerInfo> list)
        {
            var info = await GetServer(redis, endpoint).InfoAsync();
            var serverInfo = new ServerInfo(endpoint.ToString(), info.ToList());
            list.Add(serverInfo);
        }


        public static async Task<List<SearchResult>> GetRedisKeysByPattern(string key, string connString, string pattern)
        {
            try
            {
                CreateConnectionToServers(key, connString);

                List<SearchResult> results = new List<SearchResult>();
                if (Connections.ContainsKey(key))
                {
                    var redis = Connections[key];
                    var endpoints = redis.GetEndPoints();

                    var tasks = endpoints.Select(e => GetServerKeysByPattern(redis, e, pattern, results));
                    await Task.WhenAll(tasks.ToArray());
                }

                return results.OrderBy(l => l.Key).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, "GetRedisServersInfo for {key} - {connString}", key, connString);
            }
            return null;
        }
        private static async Task GetServerKeysByPattern(ConnectionMultiplexer redis, EndPoint endpoint, string pattern, List<SearchResult> results)
        {
            IServer server = GetServer(redis, endpoint);
            if (server.IsSlave)
                return;

            var loaded = await GetLoadedLuaScript(redis, server);
            if (loaded == null)
                return;

            var json = await loaded.EvaluateAsync(redis.GetDatabase(0), new { keyPattern = pattern, rows = 100 });
            if(!json.IsNull)
            {
                var listResults = JsonConvert.DeserializeObject<List<SearchResult>>(json.ToString());
                if(listResults != null)
                { 
                    listResults.ForEach(r => r.Server = endpoint.ToString());
                    results.AddRange(listResults);
                }
            }
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