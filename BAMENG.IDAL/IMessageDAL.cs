﻿/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2016. All rights reserved.
 * author guomw
**/

using BAMENG.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMENG.IDAL
{
    public interface IMessageDAL : IDisposable
    {
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="shopId">门店ID 0总后台，</param>
        /// <param name="AuthorIdentity">作者身份 0总后台，1总店，2分店</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        ResultPageModel GetMessageList(int shopId, int AuthorIdentity, SearchModel model);


        /// <summary>
        /// 修改消息
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateMessageInfo(MessageModel model);

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        int AddMessageInfo(MessageModel model);

        /// <summary>
        /// 添加消息发送目标
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="SendTargetShopId">如果是总店往总后台发布信息通知，则值为-1</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool AddMessageSendTarget(int messageId, int SendTargetShopId);
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Int32.</returns>
        bool DeleteMessageInfo(int messageId, int shopId, int type);


        /// <summary>
        /// 获取消息信息
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>MessageModel.</returns>
        MessageModel GetModel(int messageId);


        /// <summary>
        /// 修改阅读状态
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateReadStatus(int messageId, int shopId);

        /// <summary>
        /// 修改总后台阅读状态
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateReadStatus(int messageId);
    }
}
