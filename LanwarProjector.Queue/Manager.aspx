<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manager.aspx.cs" Inherits="LanwarProjector.Queue.Manager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanwar Video Manager</title>

    <script src="Scripts/jquery-1.8.2.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.24.js"></script>
    <script src="Scripts/jquery.signalR-2.0.1.js"></script>
    <script src="signalr/hubs"></script>
    <script src="Scripts/dateFormat.min.js"></script>
    <script src="Scripts/common.js"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/jquery.dataTables.min.js"></script>
    <link href="Content/themes/base/jquery-ui.css" rel="stylesheet" />
    <link href="//ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables_themeroller.css" rel="stylesheet" />
    <link href="Content/common.css" rel="stylesheet" />

    <script type="text/javascript">
        jQuery(function ($) {
            var conn = $.connection.queryHub;
            var connman = $.connection.managerHub;

            $.connection.hub.qs = "admin=" + getParameterByName("admin");

            conn.client.addVideo = function (video) {
                addVideo(video);
            };

            conn.client.updateRank = function (id, rank) {
                var table = $('#' + id).parents('table').dataTable();
                var row = table.fnGetPosition($('#' + id).get(0));
                var video = table.fnGetData(row);
                video.Rank = rank;
                table.fnUpdate(video, row);
                $('#' + id).effect('highlight');
            };

            connman.client.setup = function (Config) {
                $.each(Config.Videos, function (i, video) {
                    addVideo(video);
                });

                $.each(Config.ConnectionInfos, function (i, connInfo) {
                    addStorage(connInfo);
                });
            };

            connman.client.deleteVideo = function (id) {
                var tr = $('#' + id).get(0);
                $('#' + id).parents('table').dataTable().fnDeleteRow(tr, true);
            };

            connman.client.updateVideoStatus = function (video) {
                var tr = $('#' + video.Id).get(0);
                $('#' + video.Id).parents('table').dataTable().fnDeleteRow(tr, true);

                addVideo(video);
            };

            connman.client.addStorageComplete = function (connInfo) {
                addStorage(connInfo, "OK!");
            };

            connman.client.storageUpdate = function (connInfo) {
                var row = tblStorage.fnGetPosition($('#' + connInfo.Id).get(0));
                tblStorage.fnUpdate(connInfo, row);
                $('#' + connInfo.Id).effect('highlight');
            };

            conn.client.status = statusUpdate;
            connman.client.status = statusUpdate;

            $.connection.hub.start().done(function () {
                $('#btnStatusUpdate').click(function () {
                    connman.server.sendMessage($('#StatusUpdate').val(), $('#ddlStatusUpdateLevel').val(), $('#ddlStatusUpdateDuration').val());
                    $('#StatusUpdate').val('');
                    $('#ddlStatusUpdateLevel').val('status');
                    $('#ddlStatusUpdateDuration').val(5);
                });

                $('#StatusUpdate').on('keydown', function (e) {
                    if (e.keyCode == 13) {
                        connman.server.sendMessage($('#StatusUpdate').val(), $('#ddlStatusUpdateLevel').val(), $('#ddlStatusUpdateDuration').val());
                        $('#StatusUpdate').val('');
                        $('#ddlStatusUpdateLevel').val('status');
                        $('#ddlStatusUpdateDuration').val(5);
                        e.preventDefault();
                    }
                });

                $('#tblPending, #tblBanned, #tblQueued').on('click', '.icons li', function (e) {

                    var table = $(e.delegateTarget).dataTable();
                    var id = table.fnGetData($(this).parents('td').get(0));
                    //var tr = $(this).parents('tr').get(0);

                    switch ($(this).data('action')) {
                        case "ban":
                            ban(id);
                            break;
                        case "delete":
                            confirm("Are you sure you want to delete?", "Confirm", function () {
                                connman.server.deleteVideo(id);
                            });
                            break;
                        case "undoban":
                            connman.server.unBanVideo(id);
                            break;
                        default:
                            break;
                    }
                    e.preventDefault();
                });

                $('#addStorageButton').click(function (e) {
                    e.preventDefault();
                    if ($("#addStorageName").val().trim() == "") {
                        statusUpdate("Storage Name Must Not Be Blank", 5, "error");
                        return;
                    }
                    if ($("#addStorageNetworkPath").val().trim() == "") {
                        statusUpdate("Network Path Must Not Be Blank", 5, "error");
                        return;
                    }
                    if ($("#addStorageUsername").val().trim() == "") {
                        statusUpdate("Username Must Not Be Blank", 5, "error");
                        return;
                    }
                    if ($("#addStoragePassword").val().trim() == "") {
                        statusUpdate("Password Must Not Be Blank", 5, "error");
                        return;
                    }
                    connman.server.addStorage($("#addStorageNetworkPath").val().trim(), $("#addStorageUsername").val().trim(), $("#addStoragePassword").val().trim(), $("#addStorageName").val().trim(), $('#addStorageWritable').prop('checked'));
                });

                connman.server.setup();

            });

            function ban(id) {
                $("<div></div>").dialog({
                    resizable: false,
                    height: 200,
                    width: 500,
                    modal: true,
                    closeOnEscape: false,
                    title: "Enter Ban Reason",
                    buttons: {
                        "Ban": function () {
                            if ($('#tbBanReason').val().trim() == "")
                                return;

                            connman.server.banVideo(id, $('#tbBanReason').val().trim());
                            $(this).dialog("close").remove();
                        },
                        Cancel: function () {
                            $(this).dialog("close").remove();
                        }
                    }
                })
                .append($('<span></span>').html('Enter Reason for Ban: '))
                .append($('<input />').attr({ 'type': 'text', 'size': 25, 'id': 'tbBanReason' }).on('keydown', function (e) {
                    if (e.keyCode == 13) {
                        $(this).parent().dialog().next(".ui-dialog-buttonpane").find("button:contains('Ban')").click();
                        e.preventDefault();
                    }
                }));
            }

            function addVideo(video) {
                switch (video.Status) {
                    case 0:
                        tblBanned.fnAddData(video);
                        break;
                    case 1:
                        tblPending.fnAddData(video);
                        break;
                    case 2:
                        tblQueued.fnAddData(video);
                        break;
                }
            }

            function addStorage(connInfo) {
                tblStorage.fnAddData(connInfo);
            }


            //Setup Interface

            $('#main').tabs();
            $('#videos').tabs();
            $("#manage").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
            $("#manage li").removeClass("ui-corner-top").addClass("ui-corner-left");
            $('#addStorageButton').button({ icons: { primary: "ui-icon-plus" } });

            var tblPending = $('#tblPending').dataTable({
                bJQueryUI: true,
                "iDisplayLength": 50,
                "aaSorting": [[3, "desc"]],
                "aoColumns": [
                    {
                        "mData": "Title"
                    },
                    {
                        "mData": "VideoType",
                        "sWidth": "25px"
                    },
                    {
                        "mData": function (source, type, val) {

                            switch (source.VideoType) {
                                case 0:
                                    return '<a href="' + source.Url + '" target="_blank">' + source.Url + "</a>";
                                    break;
                                default:
                                    return source.Path;
                                    break;
                            }

                        },

                    },
                    {
                        "mData": "Rank",
                        "sWidth": "50px"
                    },
                    {
                        "mData": "Id",
                        "sWidth": "100px",
                        "bSortable": false
                    }
                ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    var temp = "<ul class='ui-widget ui-helper-clearfix icons'><li class='ui-state-default ui-corner-all' data-action='delete'><span class='ui-icon ui-icon-trash' title='Delete Video'></span></li><li class='ui-state-default ui-corner-all' data-action='ban'><span class='ui-icon ui-icon-notice' title='Ban'></span></li></ul>"

                    $('td:eq(4)', nRow).html(temp);
                    $(nRow).attr('id', aData.Id);
                }
            });

            var tblBanned = $('#tblBanned').dataTable({
                bJQueryUI: true,
                "iDisplayLength": 50,
                "aoColumns": [
                    {
                        "mData": "Title"
                    },
                    {
                        "mData": "VideoType",
                        "sWidth": "25px"
                    },
                    {
                        "mData": function (source, type, val) {

                            switch (source.VideoType) {
                                case 0:
                                    return '<a href="' + source.Url + '" target="_blank">' + source.Url + "</a>";
                                    break;
                                default:
                                    return source.Path;
                                    break;
                            }

                        },

                    },
                    {
                        "mData": "Message",
                        "sWidth": "150px"
                    },
                    {
                        "mData": "Id",
                        "sWidth": "50px"
                    }
                ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    var temp = "<ul class='ui-widget ui-helper-clearfix icons'><li class='ui-state-default ui-corner-all' data-action='delete'><span class='ui-icon ui-icon-trash' title='Delete Video'></span></li><li class='ui-state-default ui-corner-all' data-action='undoban'><span class='ui-icon ui-icon-arrowreturnthick-1-w' title='Undo Ban'></span></li></ul>";
                    $('td:eq(4)', nRow).html(temp);
                    $(nRow).attr('id', aData.Id);
                }
            });

            var tblQueued = $('#tblQueued').dataTable({
                bJQueryUI: true,
                "iDisplayLength": 50
            });

            var tblStorage = $('#tblStorage').dataTable({
                bJQueryUI: true,
                "aoColumns": [
                    {
                        "mData": "Name"
                    },
                    {
                        "mData": "Path"
                    },
                    {
                        "mData": "Username"
                    },
                    {
                        "mData": function (source, type, val) {
                            return source.Status + "(" + $.format.date(source.Statustime, "ddd hh:mm a") + ")"
                        }
                    },
                    {
                        "mData": function (source, type, val) {
                            return "";
                        }
                    }
                ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    $(nRow).attr('id', aData.Id);
                }
            });

            function getQueueOptions() {
                return [
                    $('<option></option>').html('Stub').val('Stub'),
                    $('<option></option>').html('Stub2').val('Stub2'),
                ];
            }
        });
    </script>
    <style type="text/css">
        .icons {
            margin: 2px;
            position: relative;
            padding: 4px 0;
            cursor: pointer;
            float: left;
            list-style: none;
        }

            .icons li {
                float: left;
                padding: 3px;
                margin-right: 3px;
            }

        .ui-tabs-vertical {
            width: 100%;
        }

            .ui-tabs-vertical .ui-tabs-nav {
                padding: .2em .1em .2em .2em;
                float: left;
                width: 16.1em;
            }

                .ui-tabs-vertical .ui-tabs-nav li {
                    clear: left;
                    width: 100%;
                    border-bottom-width: 1px !important;
                    border-right-width: 0 !important;
                    margin: 0 -1px .2em 0;
                }

                    .ui-tabs-vertical .ui-tabs-nav li a {
                        display: block;
                        width: 16em;
                    }

                    .ui-tabs-vertical .ui-tabs-nav li.ui-tabs-active {
                        padding-bottom: 0;
                        padding-right: .1em;
                        border-right-width: 1px !important;
                    }

            .ui-tabs-vertical .ui-tabs-panel {
                padding: 1em;
                float: right;
                width: 1000px;
            }

        #addStorageButton {
            padding: 6px 0px;
            width: 30px;
        }

        #tblStorage thead th {
            text-align:left;
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
            <ul>
                <li><a href="#videos">Videos</a></li>
                <li><a href="#manage">Management</a></li>
            </ul>
            <div id="videos">
                <ul>
                    <li><a href="#pending">Pending</a></li>
                    <li><a href="#banned">Banned</a></li>
                    <li><a href="#queued">Queued</a></li>
                </ul>
                <div id="pending">
                    <table id="tblPending">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Type</th>
                                <th>Url/Path</th>
                                <th>Votes</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div id="banned">
                    <table id="tblBanned">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Type</th>
                                <th>Url/Path</th>
                                <th>Reason For Ban</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div id="queued">
                    <table id="tblQueued">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Type</th>
                                <th>Url/Path</th>
                                <th>Votes</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
            <div id="manage">

                <ul>
                    <li><a href="#StatusUpdateDiv">Send A Message To Everyone</a></li>
                    <li><a href="#ManageQueuesDiv">Queues</a></li>
                    <li><a href="#ManageStorage">Storage</a></li>
                </ul>

                <div id="StatusUpdateDiv">
                    <h3>Send A Message To Everyone</h3>
                    Message:<input type="text" id="StatusUpdate" />
                    <select id="ddlStatusUpdateLevel">
                        <option value="status">Status</option>
                        <option value="error">Error</option>
                        <option value="critical">Critical</option>
                    </select>
                    <select id="ddlStatusUpdateDuration">
                        <option value="5">5 Seconds</option>
                        <option value="10">10 Seconds</option>
                        <option value="15">15 Seconds</option>
                        <option value="30">30 Seconds</option>
                        <option value="60">60 Seconds</option>
                        <option value="120">2 Minutes</option>
                        <option value="300">5 Minutes</option>
                        <option value="600">10 Minutes</option>
                        <option value="900">15 Minutes</option>
                        <option value="1800">30 Minutes</option>
                        <option value="3600">60 Minutes</option>
                        <option value="14400">4 hours</option>
                    </select>
                    <input type="button" id="btnStatusUpdate" value="Send" />
                </div>
                <div id="ManageQueuesDiv">
                </div>
                <div id="ManageStorage">
                    <h3>Storage Management</h3>
                    Name:
                    <input type="text" size="10" id="addStorageName" />
                    Network Path:
                    <input type="text" size="10" id="addStorageNetworkPath" />
                    Username:
                    <input type="text" size="10" id="addStorageUsername" />
                    Password:
                    <input type="password" id="addStoragePassword" />
                    <label for="addStorageWritable" title="Can this storage be used to store files downloaded from youtube and queued up.">Writeable:</label>
                    <input type="checkbox" id="addStorageWritable" />
                    <button id="addStorageButton"></button>
                    <br />
                    <h4>Existing Connections</h4>
                    <table id="tblStorage">
                        <thead>
                            <tr>
                                <th style="width:100px">Name</th>
                                <th style="width:350px">Path</th>
                                <th style="width:170px">Username</th>
                                <th style="width:200px">Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

        </div>
    </form>
    <div id="deny" runat="server">
        Nope.
    </div>
</body>
</html>
