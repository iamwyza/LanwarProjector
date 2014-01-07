using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using AutoMapper;
using NHibernate;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;
using System.IO;
using System.Net;
using System.ComponentModel;
using LanwarProjector.Common;
using LanwarProjector.Common.Domain;
using LanwarProjector.Common.DTO;

namespace LanwarProjectorMediaIndexer
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static string AdminKey = ConfigurationManager.AppSettings["AdminKey"];
        
        [STAThread]
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                log.Debug("Startup");

                log.Debug("Setup AutoMapper");
                AutoMapperSetup.Setup();

                IList<ConnectionInfo> connections;

                log.Debug("Testing NH Session Creation");
                using (var session = NhFactory.OpenSession())
                {
                    log.Debug("Testing NH Session Querying");
                    // Just a test to ensure we connect successfully
                    connections = session.QueryOver<ConnectionInfo>().List();
                }



                log.Debug("Setup SignalR Connection");
                var queryStringData = new Dictionary<string, string>();
                queryStringData.Add("admin", AdminKey);
                var connection = new HubConnection("http://127.0.0.1/LanwarProjector/", queryStringData);
                var managerHub = connection.CreateHubProxy("ManagerHub");
                connection.JsonSerializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                
                connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        log.Debug(String.Format("There was an error opening the connection:{0}",
                                          task.Exception.GetBaseException()));
                    }
                    else
                    {
                       log.Debug("Signalr Connected");
                    }
                }).Wait();

                managerHub.Invoke("SendManagerMessage", "Indexer Connected!", "status", 10).Wait();

                log.Debug("Setup & Testing Complete");

                log.Debug("Start Indexing Tasks");
                using (var session = NhFactory.OpenSession())
                {
                    foreach (var conninfo in connections)
                    {
                        try
                        {
                            NetworkCredential creds;

                            if (conninfo.Username.Contains("\\"))
                            {
                                var temp = conninfo.Username.Split('\\');
                                creds = new NetworkCredential(temp[1], System.Text.Encoding.UTF8.GetString(Encryption.AESGCM.SimpleDecrypt(conninfo.Password, Global.EncryptionKey)), temp[0]);
                            }
                            else
                            {
                                creds = new NetworkCredential(conninfo.Username, System.Text.Encoding.UTF8.GetString(Encryption.AESGCM.SimpleDecrypt(conninfo.Password, Global.EncryptionKey)));
                            }

                            using (new LanwarProjector.Common.Utilities.NetworkConnection(conninfo.Path, creds))
                            {
                                using (var transaction = session.BeginTransaction())
                                {
                                    conninfo.Status = "Indexing";
                                    conninfo.Statustime = DateTime.Now;
                                    session.Update(conninfo);
                                    transaction.Commit();

                                    managerHub.Invoke<ConnectioninfoDTO>("StorageUpdate", Mapper.Map<ConnectioninfoDTO>(conninfo)).Wait();
                                }

                                using (var transaction = session.BeginTransaction())
                                {
                                    log.Debug("Deleting existing records for " + conninfo.Path);
                                    var queryString = string.Format("delete MediaIndex where ConnectionId = :connectionid");
                                    session.CreateQuery(queryString)
                                           .SetParameter("connectionid", conninfo.Id)
                                           .ExecuteUpdate();
                                }

                                using (var transaction = session.BeginTransaction())
                                {
                                    var files = Directory.GetFiles(conninfo.Path, "*.*", SearchOption.AllDirectories);
                                    foreach (var file in files)
                                    {
                                        log.Debug(file.Replace(conninfo.Path, String.Empty));
                                        var index = new MediaIndex { ConnectionInfo = conninfo, Id = Guid.NewGuid(), Path = file.Replace(conninfo.Path, String.Empty) };
                                        session.Save(index);
                                    }
                                    conninfo.Status = "Indexed";
                                    conninfo.Statustime = DateTime.Now;
                                    session.Update(conninfo);
                                    transaction.Commit();

                                    managerHub.Invoke<ConnectioninfoDTO>("StorageUpdate", Mapper.Map<ConnectioninfoDTO>(conninfo)).Wait();
                                }
                            }
                        }
                        catch (Win32Exception ex)
                        {
                            log.Error(ex);
                            managerHub.Invoke("SendManagerMessage", String.Format("Indexer: Couldn't connect to {0} {1}", conninfo.Name, conninfo.Path), "critical", 10).Wait();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            managerHub.Invoke("SendManagerMessage", "Indexer: Failed to save connection info:" + ex, "critical", 10).Wait();
                        }
                    }
                }
                managerHub.Invoke("SendManagerMessage", "Indexer: Indexing run complete.", "status",  10).Wait();
                log.Debug("Indexing Tasks Complete");

            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
            }
        }
    }
}
