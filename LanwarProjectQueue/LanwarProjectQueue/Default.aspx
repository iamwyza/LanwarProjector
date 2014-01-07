<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LanwarProjectQueue.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanwar Projector Request System</title>
    <script src="Scripts/jquery-1.8.2.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.24.js"></script>
    <script src="Scripts/jquery.signalR-2.0.1.js"></script>
    <script src="signalr/hubs"></script>
    <script src="Scripts/dateFormat.min.js"></script>
    <script src="Scripts/common.js"></script>
    <link href="Content/themes/base/jquery-ui.css" rel="stylesheet" />
    <link href="Content/common.css" rel="stylesheet" />
    <script type="text/javascript">
        jQuery(function ($) {
            var conn = $.connection.queryHub;
            console.log("admin=" + getParameterByName("admin") + "&ip=" + getParameterByName("ip"));
            $.connection.hub.qs = "admin=" + getParameterByName("admin") + "&ip=" + getParameterByName("ip");

            conn.client.addVideo = function (video, hasVoted) {
                addVideo($('#pending'), video, hasVoted);
                $('#pending').children().sortElements(rankSortDesc);
            };

            conn.client.deleteVideo = function (id) {
                $('#' + id).remove();
            }

            conn.client.updateRank = function (id, rank) {
                $('#' + id).effect('highlight').children('.urlVotes').html(rank);
                $('#pending').children().sortElements(rankSortDesc);
            };

            conn.client.updateVideoStatus = function (video) {
                $('#' + video.Id).appendTo(getStatusDiv(video.Status));
                $('#' + video.Id).children('.videoNote').first().html(video.Message);
            }

            conn.client.status = statusUpdate;

            conn.client.displayAll = function (videos, votedList) {
                console.log(votedList);
                $.each(videos, function (i, video) {
                    addVideo(getStatusDiv(video.Status), video, $.inArray(video.Id, votedList) > -1);
                });
                $('#pending').children().sortElements(rankSortDesc);
                $('#banned').children().sortElements(watchTimeSort);
                $('#completed').children().sortElements(watchTimeSort);
            }

            $.connection.hub.start().done(function () {
                $('#add').click(function () {
                    conn.server.submitUrl($('#url').val());
                    $('#url').val('');
                });

                $('#url').on('keydown', function (e) {
                    if (e.keyCode == 13) {
                        conn.server.submitUrl($('#url').val());
                        $('#url').val('');
                        return false;
                    }
                });

                $('#main').on('click', '.urlVoteUp,.urlVoteDown', function (event) {
                    var id = $(this).parents('div').first().attr('id');
                    var up;
                    if ($(this).hasClass('urlVoteUp')) {
                        up = true;
                    } else if ($(this).hasClass('urlVoteUpDisabled')) {
                        return;
                    } else if ($(this).hasClass('urlVoteDown')) {
                        up = false;
                    } else if ($(this).hasClass('urlVoteDownDisabled')) {
                        return;
                    }

                    var voteUp = $('#' + id).children('.urlVoteUp');
                    var voteDown = $('#' + id).children('.urlVoteDown');

                    if (voteUp.length > 0)
                        voteUp.first().removeClass('urlVoteUp').addClass('urlVoteUpDisabled');

                    if (voteDown.length > 0)
                        voteDown.first().removeClass('urlVoteDown').addClass('urlVoteDownDisabled');

                    conn.server.vote(id, up);
                });

                conn.server.requestAll();
            });

            function addVideo(div, video, voted) {
                div.append($('<div></div>').addClass('url').attr('id', video.Id).data('add_time', video.AddTime).data('watch_time', video.WatchTime)
                            .append($('<span></span>').addClass('urlTitle')
                                .append($('<a></a>').attr({ 'href': video.Url, 'target': '_blank' }).html(video.Title)))
                            .append($('<span></span>').addClass('videoNote').html(video.Message))
                            .append($('<span></span>').addClass('urlVotes').html(video.Rank))
                            .append($('<div></div>').addClass(voted ? 'urlVoteUpDisabled' : 'urlVoteUp'))
                            .append($('<div></div>').addClass(voted ? 'urlVoteDownDisabled' : 'urlVoteDown'))
                            );
            }

            function getStatusDiv(status) {
                switch (status) {
                    case 0: return $('#banned');
                        break;
                    case 1: return $('#pending');
                        break;
                    case 2: return $('#completed');
                        break;
                }
            }

        });
    </script>
    <style type="text/css">


        #pending, #banned, #completed {
            width: 400px !important;
            float: left !important;
            list-style-type: none;
            list-style-position: inside;
            padding-left: 0px;
            padding: 10px;
        }

        #pending li {
            list-style-type: none;
            height: 30px;
            border-bottom: 1px solid #000000;
            text-align: left;
            margin-left: 0;
            padding-left: 0;
            width: 100%;
        }

        .url {
            height: 80px;
            background-color: #e4e4e4;
            border-radius: 4px;
            margin-bottom: 5px;
            padding: 5px;
        }

        #banned .urlVoteUp, #banned .urlVoteUpDisabled, #banned .urlVoteDown, #banned .urlVoteDownDisabled, #banned .urlVotes {
            display:none;
        }

        #banned .videoNote {
            color: #FF0000;
        }

        #banned .urlTitle {
            width:395px;
        }

        .urlVoteUp, .urlVoteUpDisabled {
            float: right;
            clear: both;
            width: 32px;
            height: 32px;
            background-image: url('Content/Up.png');
        }

        .urlVoteDown, .urlVoteDownDisabled {
            float: right;
            width: 32px;
            height: 32px;
            background-image: url('Content/Down.png');
        }

        .urlVoteUpDisabled {
            background-image: url('Content/UpGrey.png') !important;
        }

        .urlVoteDownDisabled {
            background-image: url('Content/DownGrey.png') !important;
        }

        .urlTitle {
            width: 325px;
            display: inline-block;
            float: left;
        }

        .urlActual {
            float: left;
            clear: left;
        }

        .urlVotes {
            float: right;
            clear: right;
            font-size: 40px;
        }

        .urlVoteUp, .urlVoteDown {
            cursor: pointer;
        }

        #actionbar {
            height: 75px;
            float: left;
            clear: both;
            border: 1px solid #cacaca;
            border-radius: 4px;
            width: 100% !important;
            padding-top: 7px;
            background-color: #fafafa;
        }

        #byUrl, #byUpload, #byExisting {
            border-right: 1px solid #acacac;
            padding: 10px;
            float: left;
            height: 48px;
            width: 400px;
        }

        #byExisting {
            border-right: none !important;
        }

        #actionbar span {
            font-size: 20px;
        }

        #url {
            width: 300px;
        }


    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="infoHolder">
            <div id="info">
            </div>
        </div>
        <div id="main">
            <div id="actionbar">
                <div id="byUrl">
                    <span>By URL:</span>
                    <br />
                    Url:
                    <input type="text" id="url" />
                    <input type="button" id="add" value="Add" />
                </div>
                <div id="byUpload">
                    <span>By Upload:</span>
                </div>
                <div id="byExisting">
                    <span>Search Existing Libraries (Lanwar Media):</span>
                </div>
            </div>
            <div id="pending">
                Submitted Videos:
            </div>
            <div id="banned">
                Not a Chance:
            </div>
            <div id="completed">
                Been There Done That (Or will be soon):
            </div>
        </div>

    </form>
</body>
</html>
