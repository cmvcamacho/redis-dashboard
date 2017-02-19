# redis-dashboard
A simple dashboard to monitor Redis instances

A dashboard to display the status of an individual Redis server or multiple servers arranged in Cluster or Sentinel modes.

###### Dashboard
![dashboard](/capture.png)

## Usage

The dashboard can be configured editing the config.json file:
```
[
  {
    "GroupId": 1,
    "Title": "Staging",
    "ServerGroups": [
      {
        "FriendlyUrl": "Staging",
        "Title": "STG",
        "Endpoints": "127.0.0.1:6379,abortConnect=false,allowAdmin=true"
      }
    ]
  },  
  {
    "GroupId": 2,
    "Title": "Production",
    "ServerGroups": [
      {
        "FriendlyUrl": "Cluster1",
        "Title": "Cluster 1",
        "Endpoints": "10.93.237.1:6379,10.93.237.2:6379,10.93.237.1:6379,abortConnect=false,allowAdmin=true"
      },
      {
        "FriendlyUrl": "Cluster2",
        "Title": "Cluster 2",
        "Endpoints": "10.93.235.6:6379,10.93.235.7:6379,10.93.235.8:6379,10.93.235.9:6379,10.93.235.10:6379,abortConnect=false,allowAdmin=true"
      }
    ]
  }
]
```
If don't have a Redis server available but want to give this dashboard a go, you can use the executables in _tools_ folder to run a local Redis server. The commands are:
* .\tools\redis-server.exe --> this will start a Redis server
* .\tools\redis-cli.exe --latency --> this will simulate work on Redis



## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Technology

This project is built using the following stack:
* BootStrap
* Microsoft MVC
* Newtonsoft.Json
* StackExchange.Redis

## Credits

Copyright (c) 2017 Carlos Camacho

## License

See the [LICENSE](LICENSE) file for license rights and limitations (MIT).