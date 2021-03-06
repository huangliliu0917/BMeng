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
    /// <summary>
    /// 优惠券相关接口
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ICouponDAL : IDisposable
    {
        /// <summary>
        /// 获取现金券列表
        /// </summary>
        /// <param name="shopId">门店ID</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        ResultPageModel GetCashCouponList(int shopId, SearchModel model);


        /// <summary>
        /// 添加现金券
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        int AddCashCoupon(CashCouponModel model);


        /// <summary>
        /// 修改现金券
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateCashCoupon(CashCouponModel model);


        /// <summary>
        /// 删除现金券
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool DeleteCashCoupon(int couponId);




        /// <summary>
        ///设置优惠券启用和禁用
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool SetCouponEnable(int couponId);



        /// <summary>
        /// 创建用户现金券(盟主分享现金券时调用)
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        int CreateUserCashCouponLog(CashCouponLogModel model);

        /// <summary>
        /// 更新现金券的领取记录
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateUserCashCouponGetLog(CashCouponLogModel model);



        bool DeleteUserCashCoupon(string couponNo, int couponId, int userId);
        /// <summary>
        /// 更新现金券的领取记录
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateUserCashCouponUseStatus(int cashlogId);

        /// <summary>
        /// 获取现金券领取列表
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        ResultPageModel GetUserCashCouponLogList(int couponId, SearchModel model);

        CashCouponLogModel getEnableCashCouponLogModel(string mobile, string cashNo);



        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <param name="isValid">是否只获取有效的优惠券</param>
        /// <returns>CashCouponModel.</returns>
        CashCouponModel GetModel(int couponId, bool isValid = false);


        /// <summary>
        /// 我的现金券数量
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.Int32.</returns>
        int GetMyCashCouponCount(int userId);


        /// <summary>
        /// 根据用户ID和优惠券ID，获取优惠券记录ID
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>System.Int32.</returns>
        CashCouponLogModel GetCashCouponLogIDByUserID(int userId, int couponId);


        /// <summary>
        /// 获得现金卷列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        List<CashCouponModel> getEnabledCashCouponList(int shopId);

        /// <summary>
        /// 根据盟友ID，获取盟友的现金券列表
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List&lt;CashCouponModel&gt;.</returns>
        List<CashCouponModel> GetEnableCashCouponListByUserId(int userId);

        /// <summary>
        /// 获得优惠卷发送列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<CouponSendModel> getCouponSendList(int userId);




        /// <summary>
        /// 添加优惠券发送记录
        /// </summary>
        /// <param name="userId">发送人ID</param>
        /// <param name="sendToUserId">接收用户ID,如果是自己转发，则为0</param>
        /// <param name="couponId">优惠券ID</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool AddSendCoupon(int userId, int sendToUserId, int couponId);


        /// <summary>
        /// 更新优惠券分享状态(只有盟友身份时才操作)
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        bool UpdateCouponShareStatus(int userId, int couponId);


        /// <summary>
        ///判断用户是否已转发
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if [is send coupon by user identifier] [the specified user identifier]; otherwise, false.</returns>
        bool IsSendCouponByUserId(int userId, int couponId);
    }
}
