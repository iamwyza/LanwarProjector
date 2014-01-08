using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using LanwarProjector.Queue;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using AutoMapper;
using LanwarProjector.Common.Domain;
using NHibernate.Criterion;
using log4net;
using LanwarProjector.Common;
using LanwarProjector.Common.DTO;

namespace LanwarProjector.Queue
{
    public class QueryHub : Hub
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(QueryHub));

        public void SubmitUrl(string originalUrl)
        {
            using (var session = NhFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    
                    try
                    {
                        var url = originalUrl.ToLower().Trim().Replace("http://", "").Replace("https://", "").Replace("www.", "");

                        //var videos = session.QueryOver<Video>().Where(x => x.Url.ToLower().EndsWith(url)).Take(1).List<Video>();
                        var videos = session.CreateCriteria<Video>().Add(Restrictions.InsensitiveLike("Url", url)).List<Video>();
                        if (videos.Any())
                        {
                            var video = videos.First();
                            if (video.Status == Status.Banned)
                            {
                                Clients.Caller.Status("Video has been banned.");
                            }
                            else if (video.Status == Status.Watched && !video.Votes.Any(x => x.IpAddress == GetIpAddress()))
                            {
                                video.Rank++;
                                video.Votes.Add(new Vote() { Id = Guid.NewGuid(), IpAddress = GetIpAddress(), VoteValue = 1, VoteTime = DateTime.Now, Video = video });
                                Clients.Caller.Status("Video has already been watched, however votes can still impact its likelyhood to be watched again.  Voted up.");
                                session.SaveOrUpdate(video);
                                transaction.Commit();
                                Clients.All.UpdateRank(video.Id, video.Rank);
                            }
                            else if (video.Status != Status.Watched && !video.Votes.Any(x => x.IpAddress == GetIpAddress()))
                            {
                                video.Rank++;
                                video.Votes.Add(new Vote() { Id = Guid.NewGuid(), IpAddress = GetIpAddress(), VoteValue = 1, VoteTime = DateTime.Now, Video = video });
                                session.SaveOrUpdate(video);
                                transaction.Commit();
                                Clients.All.UpdateRank(video.Id, video.Rank);
                            }

                        }
                        else
                        {
                            var video = new Video();
                            video.Id = Guid.NewGuid();
                            video.Url = originalUrl.Trim();
                            video.Status = Status.Pending;
                            video.Rank = 1;
                            video.Title = GetTitle(originalUrl);
                            video.Votes.Add(new Vote() { Id = Guid.NewGuid(), IpAddress = GetIpAddress(), VoteValue = 1, VoteTime = DateTime.Now, Video = video });

                            session.SaveOrUpdate(video);
                            transaction.Commit();
                            Clients.Others.AddVideo(Mapper.Map<VideoDTO>(video));
                            Clients.Caller.AddVideo(Mapper.Map<VideoDTO>(video), true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Caller.Status("Failed to add new video:" + ex, 30, "critical");
                    }
                }
            }
        }

        public void Vote(Guid id, bool up)
        {
            using (var session = NhFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var video = session.Get<Video>(id);
                    if (video != null)
                    {
                        Console.WriteLine(GetIpAddress());
                        if (video.Votes.All(x => x.IpAddress != GetIpAddress()))
                        {

                            if (up)
                            {
                                video.Rank++;
                            }
                            else
                            {
                                video.Rank--;
                            }
                            video.Votes.Add(new Vote() { Id = Guid.NewGuid(), IpAddress = GetIpAddress(), VoteValue = (short)(up ? 1 : -1), VoteTime = DateTime.Now, Video = video });
                            session.SaveOrUpdate(video);
                            transaction.Commit();
                            Clients.All.UpdateRank(video.Id, video.Rank);
                        }
                    }
                    else
                    {
                        Clients.Caller.Status("Video not found");
                    }
                }
            }
        }

        public void RequestAll()
        {
            using (var session = NhFactory.OpenSession())
            {
                try
                {
                    var videos = Mapper.Map<IList<Video>, IList<VideoDTO>>(session.QueryOver<Video>().List());
                    var votes = session.QueryOver<Vote>().Where(x => x.IpAddress == GetIpAddress()).Select(x => x.Video.Id).List<Guid>();
                    Clients.Caller.DisplayAll(videos, votes);
                }
                catch (Exception ex)
                {
                    Clients.Caller.Status("Error fetching : " + ex, 30, "critical");
                }
            }
        }

        private string GetTitle(string Url)
        {
            if (!Url.StartsWith("http"))
                Url = "http://" + Url;
            Console.WriteLine("fetching:" + Url);
            string title = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest.Create(Url) as HttpWebRequest);
                using (HttpWebResponse response = (request.GetResponse() as HttpWebResponse))
                {

                    using (Stream stream = response.GetResponseStream())
                    {
                        // compiled regex to check for <title></title> block
                        Regex titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        int bytesToRead = 8092;
                        byte[] buffer = new byte[bytesToRead];
                        string contents = "";
                        int length = 0;
                        while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                        {
                            Console.WriteLine(contents);
                            // convert the byte-array to a string and add it to the rest of the
                            // contents that have been downloaded so far
                            contents += Encoding.UTF8.GetString(buffer, 0, length);

                            Match m = titleCheck.Match(contents);
                            if (m.Success)
                            {
                                // we found a <title></title> match =]
                                title = m.Groups[1].Value.ToString();
                                break;
                            }
                            else if (contents.Contains("</head>"))
                            {
                                // reached end of head-block; no title found =[
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(title);
            return title;
        }

        private string GetIpAddress()
        {
            string ipAddress;
            object tempObject;
            if (Context.QueryString["admin"] == Global.AdminKey && !String.IsNullOrWhiteSpace(Context.QueryString["ip"]))
                return Context.QueryString["ip"];

            Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);

            if (tempObject != null)
            {
                ipAddress = (string)tempObject;
            }
            else
            {
                ipAddress = "";
            }

            return ipAddress;
        }
    }
}