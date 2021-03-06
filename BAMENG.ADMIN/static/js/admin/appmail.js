﻿/// <reference path="../fastclick.js" />
/// <reference path="../plugins/sweetalert/sweetalert.min.js" />
/// <reference path="../jquery.min.js" />
/// <reference path="../plugins/hot/Jquery.util.js" />
/// <reference path="shareConfig.js" />
/// <reference path="../jquery.min.js" />
/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2016. All rights reserved. 
**/

var mailsHelper = {
    idt: hotUtil.getQuery("idt"),
    pageIndex: 1,
    pageSize: 10,
    loading: false, //状态标记
    sendtype: 1,
    load: function () {
        var self = this;
        var postData = {
            auth: hotUtil.auth(),
            articleId: hotUtil.getQuery("articleId")
        }
        //$.showLoading();
        hotUtil.ajaxCall("/handler/mailinfo.ashx", postData, function (ret, err) {
            if (ret) {
                if (ret.status == 200) {
                    document.title = ret.data.Title;
                    $(".demos-title").text(ret.data.Title);
                    $("#articleTime").text(ret.data.time);
                    $("#articleInfo").html(ret.data.BodyContent);
                    $(".authorName").text(ret.data.AuthorName);
                    mailsHelper.sendtype = ret.data.SendType;
                    $(".bodyContent").show();
                }
                else
                    $.alert(ret.statusText);
            }
            // $.hideLoading();
        });
    },
    replylist: function (page) {
        var param = {
            action: "GetReplyMailList",
            pageIndex: page,
            pageSize: mailsHelper.pageSize,
            mailid: hotUtil.getQuery("articleId")
        }
        $.showLoading();
        hotUtil.ajaxCall("/handler/app.ashx", param, function (ret, err) {
            if (ret) {
                if (ret.status == 200) {
                    if (ret.data) {
                        var listhtml = "";
                        var total = ret.data.Total;
                        $.each(ret.data.Rows, function (i, item) {
                            listhtml += $("#templateList").html();
                            //listhtml = listhtml.replace("{NO}", page * mailsHelper.pageSize - mailsHelper.pageSize + (i + 1));
                            listhtml = listhtml.replace("{NO}", (total - ((page - 1) * mailsHelper.pageSize) - i));
                            listhtml = listhtml.replace("{url}", item.CoverUrl);
                            listhtml = listhtml.replace("{authorname}", item.AuthorName);
                            listhtml = listhtml.replace("{content}", item.BodyContent);
                            listhtml = listhtml.replace("{time}", item.time);

                        });
                        if (!hotUtil.isNullOrEmpty(listhtml)) {
                            if (page == 1)
                                $("#j_hotlist,#j_hotlist2").html(listhtml);
                            else
                                $("#j_hotlist,#j_hotlist2").append(listhtml);

                            mailsHelper.pageIndex = page;
                            $(".f_hot_cmnt").show();
                            if (page < ret.data.PageCount) {
                                $(".f_hot_more").show();
                            }
                        }
                        mailsHelper.nextPage(page, ret.data.PageCount, mailsHelper.replylistall);
                    }
                }
                else {
                    $.alert(ret.statusText);
                }
                $.hideLoading();
            }
        });
    },
    replylistall: function (page) {
        var param = {
            action: "GetReplyMailList",
            pageIndex: page,
            pageSize: mailsHelper.pageSize,
            mailid: hotUtil.getQuery("articleId")
        }
        $.showLoading();
        hotUtil.ajaxCall("/handler/app.ashx", param, function (ret, err) {
            if (ret) {
                if (ret.status == 200) {
                    if (ret.data) {
                        var listhtml = "";
                        var total = ret.data.Total;
                        $.each(ret.data.Rows, function (i, item) {
                            listhtml += $("#templateList").html();
                            listhtml = listhtml.replace("{NO}", (total - ((page - 1) * mailsHelper.pageSize) - i));
                            listhtml = listhtml.replace("{url}", item.CoverUrl);
                            listhtml = listhtml.replace("{authorname}", item.AuthorName);
                            listhtml = listhtml.replace("{content}", item.BodyContent);
                            listhtml = listhtml.replace("{time}", item.time);

                        });
                        if (!hotUtil.isNullOrEmpty(listhtml)) {
                            if (page == 1)
                                $("#j_hotlist2").html(listhtml);
                            else
                                $("#j_hotlist2").append(listhtml);
                        }
                        mailsHelper.nextPage(page, ret.data.PageCount, mailsHelper.replylistall);
                    }
                }
                else {
                    $.alert(ret.statusText);
                }
                $.hideLoading();
            }
        });
    },
    showAll: function () {
        $("html").css({ "overflow": "hidden" });
        //this.replylistall(2);
    },
    showReplyModal: function () {
        $("html").addClass("overflowEnable");
    },
    closeReplyModal: function () {
        $("html").removeClass("overflowEnable");
    },
    nextPage: function (currentPageIndex, PageCount, callback) {
        loading = false;
        if (currentPageIndex >= PageCount) {
            $("#alllist").destroyInfinite();
            $(".weui-infinite-scroll").hide();
        }
        else {
            $(".weui-infinite-scroll").show();
            $("#alllist").infinite().on("infinite", function () {
                if (loading) return;
                loading = true;
                callback(currentPageIndex + 1);
            });
        }
    },
    addReply: function () {

        if (hotUtil.isNullOrEmpty($("#j_cmnt_input").val())) {
            $.toast("请输入回复内容", "cancel");
            return false;
        }

        var postData = {
            action: "addReply",
            auth: hotUtil.auth(),
            mailid: hotUtil.getQuery("articleId"),
            content: hotUtil.encode($("#j_cmnt_input").val()),
            sendtype: mailsHelper.sendtype,
            title: hotUtil.encode($(".demos-title").text()),
            pm: hotUtil.encode(hotUtil.getQuery("pm", ""))
        };
        $.showLoading();
        hotUtil.ajaxCall("/handler/app.ashx", postData, function (ret, err) {
            $.hideLoading();
            if (ret) {
                if (ret.status == 200) {
                    $.closePopup();
                    $("html").removeClass("overflowEnable");
                    $("#j_cmnt_input").val("");
                    $.toast("回复成功", function () {
                        mailsHelper.replylist(1);
                    });
                }
                else //if (ret.status == 70035)
                    $.alert(ret.statusText);
            }
        });
    }
}

$(function () {
    mailsHelper.load();
    mailsHelper.replylist(1);
    //FastClick.attach(document.body);
});
