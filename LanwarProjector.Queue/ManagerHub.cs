using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.IO;
using System.Net;
using Microsoft.AspNet.SignalR;
using AutoMapper;
using LanwarProjector.Common;
using LanwarProjector.Common.DTO;
using LanwarProjector.Common.Utilities;
using LanwarProjector.Common.Domain;

namespace LanwarProjector.Queue
{
    public class ManagerHub : Hub
    {

        private IHubContext _queryHubContext;

        public ManagerHub()
        {
            _queryHubContext = GlobalHost.ConnectionManager.GetHubContext<QueryHub>(); ;
        }

        public void SendMessage(String message, string level, int duration)
        {
            if (IsAdmin())
                _queryHubContext.Clients.All.Status(message, duration, level);
        }

        public void SendManagerMessage(String message, string level, int duration)
        {
            if (IsAdmin())
                Clients.All.Status(message, duration, level);
        }

        public void Setup()
        {
            if (IsAdmin())
            {
                using (var session = NhFactory.OpenSession())
                {
                    try
                    {
                        Clients.Caller.Setup(new Config { Videos = Mapper.Map<List<VideoDTO>>(session.QueryOver<Video>().List()), ConnectionInfos = Mapper.Map<List<ConnectioninfoDTO>>(session.QueryOver<ConnectionInfo>().List()) });
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Error fetching setup: " + ex, 30, "critical");
                    }
                }
            }
        }

        public void DeleteVideo(Guid id)
        {
            if (IsAdmin())
            {
                using (var session = NhFactory.OpenSession())
                {
                    try
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            session.Delete(session.Get<Video>(id));
                            transaction.Commit();
                            _queryHubContext.Clients.All.DeleteVideo(id);
                            Clients.All.DeleteVideo(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Failed to delete video: " + ex, 15, "critical");
                    }
                }
            }
        }

        public void BanVideo(Guid id, String message)
        {
            if (IsAdmin())
            {
                using (var session = NhFactory.OpenSession())
                {
                    try
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            var video = session.Get<Video>(id);
                            video.Status = Status.Banned;
                            video.Message = message;
                            session.SaveOrUpdate(video);
                            transaction.Commit();
                            _queryHubContext.Clients.All.UpdateVideoStatus(Mapper.Map<VideoDTO>(video));
                            Clients.All.UpdateVideoStatus(Mapper.Map<VideoDTO>(video));
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Failed to ban video: " + ex, 15, "critical");
                    }
                }
            }
        }

        public void UnBanVideo(Guid id)
        {
            if (IsAdmin())
            {
                using (var session = NhFactory.OpenSession())
                {
                    try
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            var video = session.Get<Video>(id);
                            video.Status = Status.Pending;
                            video.Message = String.Empty;
                            session.SaveOrUpdate(video);
                            transaction.Commit();

                            _queryHubContext.Clients.All.UpdateVideoStatus(Mapper.Map<VideoDTO>(video));
                            Clients.All.UpdateVideoStatus(Mapper.Map<VideoDTO>(video));
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Failed to unban video: " + ex, 15, "critical");
                    }
                }
            }
        }

        public void AddStorage(string path, string username, string password, string name)
        {
            if (IsAdmin())
            {
                using (var session = NhFactory.OpenSession())
                {
                    try
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            if (path.EndsWith("\\"))
                                path = path.Substring(0, path.Length - 1);
                            var conninfo = new ConnectionInfo { Id = Guid.NewGuid(), Name = name, Password = Encryption.AESGCM.SimpleEncrypt(System.Text.Encoding.UTF8.GetBytes(password), LanwarProjector.Common.Global.EncryptionKey), Path = path, Username = username, Status = "OK!", Statustime = DateTime.Now };
                            var test = ConnectionTest.Test(conninfo);
                            conninfo.Status = "OK!";
                            conninfo.Statustime = DateTime.Now;
                            session.Save(conninfo);
                            transaction.Commit();
                            Clients.Caller.Status("Connection Successful.  Found " + test.Item1 + " files and " + test.Item2 + " directories.");
                            Clients.All.AddStorageComplete(Mapper.Map<ConnectioninfoDTO>(conninfo));
                        }
                    }
                    catch (Win32Exception ex)
                    {
                        Clients.Caller.Status(String.Format("Couldn't connect ({0} {1} {2} {3}): {4} {5}", path, username, password, name, ex.Message, ex), 10, "critical");
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Failed to save connection info:" + ex, 10, "critical");
                    }
                }
            }
        }

        public void StorageUpdate(ConnectioninfoDTO connInfo)
        {
            if (IsAdmin()) {
                Clients.All.StorageUpdate(connInfo);
            }
        }

        private bool IsAdmin()
        {
            return Context.QueryString["admin"] == Global.AdminKey;
        }


    }
}