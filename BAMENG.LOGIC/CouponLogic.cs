﻿/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2016. All rights reserved.
 * author guomw
**/


using BAMENG.MODEL;
using HotCoreUtils.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMENG.LOGIC
{
    /// <summary>
    /// Class CouponLogic.
    /// </summary>
    public class CouponLogic
    {
        /// <summary>
        /// 添加现金券
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static int AddCashCoupon(CashCouponModel model)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.AddCashCoupon(model);
            }
        }

        /// <summary>
        /// 删除现金券
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool DeleteCashCoupon(int couponId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.DeleteCashCoupon(couponId);
            }
        }

        /// <summary>
        /// 修改现金券
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool UpdateCashCoupon(CashCouponModel model)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.UpdateCashCoupon(model);
            }
        }


        /// <summary>
        /// 编辑优惠券
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public static bool EditCashCoupon(CashCouponModel model)
        {
            if (model.CouponId > 0)
                return UpdateCashCoupon(model);
            else
                return AddCashCoupon(model) > 0;
        }


        /// <summary>
        /// 获取现金券列表
        /// </summary>
        /// <param name="shopId">门店ID</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static ResultPageModel GetCashCouponList(int shopId, SearchModel model)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.GetCashCouponList(shopId, model);
            }
        }

        /// <summary>
        /// 修改现金券的获取状态
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <param name="couponNo">The coupon no.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool UpdateGetStatus(int couponId, string couponNo)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.UpdateGetStatus(couponId, couponNo);
            }
        }

        /// <summary>
        /// 修改现金券使用状态
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool UpdateUseStatus(int couponId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.UpdateUseStatus(couponId);
            }
        }

        /// <summary>
        /// 设置优惠券启用和禁用
        /// </summary>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public static bool SetCouponEnable(int couponId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.SetCouponEnable(couponId);
            }
        }


        /// <summary>
        /// 我的现金券数量
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.Int32.</returns>
        public static int GetMyCashCouponCount(int userId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.GetMyCashCouponCount(userId);
            }
        }


        /// <summary>
        /// 获得现金卷列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static List<CashCouponModel> getEnabledCashCouponList(int shopId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.getEnabledCashCouponList(shopId);
            }
        }

        /// <summary>
        /// 根据盟友ID，获取盟友的现金券列表
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List&lt;CashCouponModel&gt;.</returns>
        public static List<CashCouponModel> GetEnableCashCouponListByUserId(int userId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.GetEnableCashCouponListByUserId(userId);
            }
        }


        /// <summary>
        /// 获得优惠卷发送列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<CouponSendModel> getCouponSendList(int userId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.getCouponSendList(userId);
            }
        }


        /// <summary>
        /// 添加优惠券发送记录
        /// </summary>
        /// <param name="userId">发送人ID</param>
        /// <param name="Int32">The int32.</param>
        /// <param name="userIdentity">用户身份，0盟友  1盟主.</param>
        /// <param name="sendToUserId">接收用户ID,如果是自己转发，则为0</param>
        /// <param name="couponId">优惠券ID</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public static bool AddSendCoupon(int userId, int userIdentity, int sendToUserId, int couponId)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                bool flag = dal.AddSendCoupon(userId, sendToUserId, couponId);

                //如果盟主自己分享或发送给盟友，则创建现金券记录
                if (flag && userIdentity == 1)
                {
                    CashCouponModel couponModel = dal.GetModel(couponId);
                    if (couponModel != null)
                    {
                        dal.CreateUserCashCouponLog(new CashCouponLogModel()
                        {
                            CouponId = couponId,
                            CouponNo = StringHelper.CreateCheckCode(10).ToLower(),
                            UserId = sendToUserId > 0 ? sendToUserId : userId,
                            IsShare = sendToUserId > 0 ? 1 : 0,
                            Money = couponModel.Money,
                            StartTime = couponModel.StartTime,
                            EndTime = couponModel.EndTime,
                            ShopId = couponModel.ShopId
                        });
                    }
                }

                return flag;
            }
        }

        /// <summary>
        /// 创建用户现金券(盟主分享现金券时调用)
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public int CreateUserCashCouponLog(CashCouponLogModel model)
        {
            using (var dal = FactoryDispatcher.CouponFactory())
            {
                return dal.CreateUserCashCouponLog(model);
            }
        }
    }
}
