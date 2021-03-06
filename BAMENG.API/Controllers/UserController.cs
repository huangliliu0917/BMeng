﻿using BAMENG.CONFIG;
using BAMENG.LOGIC;
using BAMENG.MODEL;
using HotCoreUtils.Helper;
using HotCoreUtils.Uploader;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BAMENG.API.Controllers
{
    /// <summary>
    /// 用户相关接口
    /// </summary>
    public class UserController : BaseController
    {

        /// <summary>
        /// 登陆接口 POST:  user/login
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="password">The password.</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize(AuthLogin = false)]
        public ActionResult Login(string loginName, string password)
        {
            try
            {
                ApiStatusCode apiCode = ApiStatusCode.OK;
                UserModel userData = AppServiceLogic.Instance.Login(loginName, password, OS, ref apiCode);
                return Json(new ResultModel(apiCode, userData));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("Login user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 签到  POST: user/signin
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{score:10}}]]></returns>
        [ActionAuthorize]
        public ActionResult SignIn()
        {
            try
            {
                ApiStatusCode apiCode = ApiStatusCode.OK;
                int Integral = UserLogic.SignIn(GetUserData(), this.OS, Addr, ref apiCode);
                Dictionary<string, int> data = new Dictionary<string, int>();
                data["score"] = Integral;
                if (apiCode == ApiStatusCode.OK)
                    return Json(new ResultModel(apiCode, string.Format("积分+{0}", Integral), data));
                else
                    return Json(new ResultModel(apiCode));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("SignIn user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }

        }

        /// <summary>
        /// 忘记密码   POST: user/forgetpwd
        /// </summary>
        /// <param name="mobile">The mobile.</param>
        /// <param name="password">The password.</param>
        /// <param name="verifyCode">The verify code.</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize(AuthLogin = false)]
        public ActionResult ForgetPwd(string mobile, string password, string verifyCode)
        {
            try
            {
                if (SmsLogic.IsPassVerify(mobile, verifyCode))
                {
                    if (UserLogic.ForgetPwd(mobile, password))
                        return Json(new ResultModel(ApiStatusCode.OK, "密码设置成功"));
                    else
                        return Json(new ResultModel(ApiStatusCode.找回密码失败));
                }
                return Json(new ResultModel(ApiStatusCode.无效验证码));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ForgetPwd user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 待结算盟豆列表 POST: user/tempsettlebeanlist
        /// </summary>
        /// <returns><![CDATA[{ status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult TempSettleBeanList(int lastId)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getTempBeansRecordsList(userId, lastId);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["list"] = data;
                dict["TempMengBeans"] = lastId == 0 ? UserLogic.countTempBeansMoney(userId, 0) : 0;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("TempSettleBeanList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 兑换盟豆 POST: user/ConvertToBean
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [ActionAuthorize]
        public ActionResult ConvertToBean(int amount)
        {
            try
            {
                int userId = GetAuthUserId();
                ApiStatusCode code = ApiStatusCode.OK;
                UserLogic.ConvertToBean(userId, amount, ref code);
                if (code == ApiStatusCode.OK)
                    return Json(new ResultModel(code, "提交成功"));
                else
                    return Json(new ResultModel(code));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ConvertToBean user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }


        /// <summary>
        /// 兑换记录流水 POST: user/ConvertFlow
        /// </summary>
        /// <param name="lastId"></param>
        /// <returns></returns>
        [ActionAuthorize]
        public ActionResult ConvertFlow(int lastId)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getConvertFlow(userId, lastId);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["list"] = data;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ConvertFlow user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 盟友列表 POST: user/allylist
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="orderbyCode">排序类型，-1 默认  0盟友等级，1客户信息，2订单成交</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public JsonResult allylist(int pageIndex, int pageSize, int orderbyCode, int isDesc)
        {
            try
            {
                var data = UserLogic.GetAllyList(new SearchModel()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    UserId = GetAuthUserId(),
                    orderbyCode = orderbyCode,
                    IsDesc = isDesc == 1
                });
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("allylist user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 兑换审核列表 POST: user/ConvertAuditList
        /// </summary>
        /// <param name="lastId">The last identifier.</param>
        /// <param name="type"> 0 未处理 1已处理</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult ConvertAuditList(int lastId, int type = 0)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getMasterConvertFlow(userId, lastId, type);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["list"] = data;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ConvertAuditList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 兑换审核 POST: user/ConvertAudit
        /// </summary>
        /// <param name="id">兑换转换记录Id</param>
        /// <param name="status">1同意2拒绝</param>
        /// <returns></returns>
        [ActionAuthorize]
        public ActionResult ConvertAudit(int id, int status)
        {
            try
            {
                ApiStatusCode code = ApiStatusCode.OK;
                int userId = GetAuthUserId();
                UserLogic.ConvertAudit(userId, id, status, ref code);
                return Json(new ResultModel(code));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ConvertAudit user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }


        /// <summary>
        /// 已兑换盟豆 POST: user/AlreadyConvertTotal
        /// </summary>
        /// <returns>ActionResult.</returns>
        [ActionAuthorize]
        public ActionResult AlreadyConvertTotal()
        {
            try
            {
                var user = GetUserData();
                int count = UserLogic.AlreadyConvertTotal(user.UserId);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["alreadyConverCount"] = count;
                dict["mengBeansCount"] = user.MengBeans;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AlreadyConvertTotal user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }


        /// <summary>
        /// 个人信息 POST: user/myinfo
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult MyInfo()
        {
            try
            {
                var data = GetUserData();
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("MyInfo user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 修改用户信息 POST: user/UpdateInfo
        /// </summary>
        /// <param name="type">修改内容类型</param>
        /// <param name="content">修改内容</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult UpdateInfo(int type, string content)
        {
            try
            {
                UserModel userInfo = new UserModel();
                userInfo.UserId = GetAuthUserId();
                Dictionary<string, object> data = new Dictionary<string, object>();
                switch (type)
                {
                    case (int)UserPropertyOptions.USER_1:
                        {
                            HttpPostedFileBase oFile = Request.Files.Count > 0 ? Request.Files[0] : null;
                            if (oFile == null)
                                return Json(new ResultModel(ApiStatusCode.请上传图片));
                            string fileName = GetUploadImagePath();
                            Stream stream = oFile.InputStream;
                            byte[] bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            // 设置当前流的位置为流的开始
                            stream.Seek(0, SeekOrigin.Begin);
                            if (FileUploadHelper.UploadFile(bytes, fileName))
                                userInfo.UserHeadImg = fileName;
                            else
                                return Json(new ResultModel(ApiStatusCode.请上传图片));


                            data["url"] = WebConfig.reswebsite() + userInfo.UserHeadImg;
                            content = userInfo.UserHeadImg;
                        }
                        break;
                    case (int)UserPropertyOptions.USER_2:
                        userInfo.NickName = content;
                        break;
                    case (int)UserPropertyOptions.USER_4:
                        userInfo.RealName = content;
                        break;
                    case (int)UserPropertyOptions.USER_5:
                        userInfo.UserGender = content.ToUpper();
                        break;
                    case (int)UserPropertyOptions.USER_6:
                        userInfo.UserCity = content;
                        break;
                }
                if (!string.IsNullOrEmpty(content))
                {
                    UserPropertyOptions opt;
                    bool flg = Enum.TryParse(type.ToString(), out opt);
                    UserLogic.UpdateUserInfo(opt, userInfo);
                    return Json(new ResultModel(ApiStatusCode.OK, "保存成功", data));
                }
                else
                {
                    return Json(new ResultModel(ApiStatusCode.内容不能为空));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("UpdateInfo user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 设置盟友奖励 POST: user/setallyraward
        /// </summary>
        /// <param name="creward">客户资料提交奖励</param>
        /// <param name="orderreward">订单成交奖励</param>
        /// <param name="shopreward">客户进店奖励</param>
        /// <param name="extrareward">额外奖励</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult setallyRaward(decimal creward, decimal orderreward, decimal shopreward, string extrareward)
        {
            return Json(new ResultModel(ApiStatusCode.无操作权限));
            //try
            //{
            //    var user = GetUserData();
            //    if (user.UserIdentity == 1)
            //    {
            //        bool flag = UserLogic.SetAllyRaward(user.UserId, creward, orderreward, shopreward, extrareward);
            //        return Json(new ResultModel(flag ? ApiStatusCode.OK : ApiStatusCode.保存失败, flag ? "保存成功" : "保存失败"));
            //    }
            //    else
            //        return Json(new ResultModel(ApiStatusCode.无操作权限));
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Log(string.Format("setallyRaward user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
            //    return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            //}
        }

        /// <summary>
        /// 获取奖励设置
        /// </summary>
        /// <returns>ActionResult.</returns>
        [ActionAuthorize]
        public ActionResult GetAllyReward()
        {
            try
            {
                var user = GetUserData();
                var data = UserLogic.GetRewardModel(user.ShopId);

                if (data == null)
                    data = new RewardsSettingModel();
                data.scoreConfig = ConfigLogic.GetScoreConfig();
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("GetAllyReward user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }

        }


        /// <summary>
        /// 积分列表 POST: user/scoreList
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult scoreList(int lastId)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getScoreList(userId, lastId);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["list"] = data;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("scoreList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }


        /// <summary>
        /// 盟豆流水列表 POST: user/BeanFlowList
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult BeanFlowList(int lastId)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getBeansRecordsList(userId, lastId);
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("BeanFlowList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 盟友详情 POST: user/AllyInfo
        /// </summary>
        /// <param name="userid">盟友用户ID</param>
        /// <returns>JsonResult.</returns>
        [ActionAuthorize]
        public JsonResult AllyInfo(int userid)
        {
            try
            {
                var data = UserLogic.GetModel(userid);
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyInfo user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 申请盟友接口 POST: user/AllyApply
        /// todo 
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult AllyApply(int userId, string mobile, string password
            , string nickname, string userName
            , int sex)
        {
            try
            {
                ApiStatusCode apiCode = ApiStatusCode.OK;
                UserLogic.AllyApply(userId, mobile, password, nickname, userName, sex, ref apiCode);
                return Json(new ResultModel(apiCode));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyInfo user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 盟友申请列表 POST: user/AllyApplylist
        /// </summary>
        /// <param name="type">0盟友申请列表，1我的盟友列表</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult AllyApplylist(int pageIndex, int pageSize)
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.GetApplyFriendList(new SearchModel()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    UserId = userId
                });
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyApplylist user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 盟友申请审核 POST: user/AllyApplyAudit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">1成功2拒绝</param>
        /// <returns></returns>
        [ActionAuthorize]
        public ActionResult AllyApplyAudit(int id, int status)
        {
            try
            {
                ApiStatusCode code = ApiStatusCode.OK;
                int userId = GetAuthUserId();
                UserLogic.AllyApplyAudit(userId, id, status, ref code);
                return Json(new ResultModel(ApiStatusCode.OK, code));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyApplyAudit user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
        /// <summary>
        /// 修改密码 POST: user/ChanagePassword
        /// </summary>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult ChanagePassword(string oldPassword, string newPassword)
        {
            try
            {
                var user = GetUserData();

                if (UserLogic.ChanagePassword(user.UserId, oldPassword, newPassword))
                    return Json(new ResultModel(ApiStatusCode.OK, "密码设置成功"));
                else
                    return Json(new ResultModel(ApiStatusCode.密码修改失败));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyApplyAudit user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }

        }

        /// <summary>
        /// 修改手机号 POST: user/ChanageMobile
        /// </summary>
        /// <param name="mobile">The mobile.</param>
        /// <param name="verifyCode">The verify code.</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult ChanageMobile(string mobile, string verifyCode)
        {
            try
            {
                if (SmsLogic.IsPassVerify(mobile, verifyCode))
                {
                    SmsLogic.UpdateVerifyCodeInvalid(mobile, verifyCode);
                    UserModel userInfo = new UserModel();
                    userInfo.UserId = GetAuthUserId();
                    userInfo.UserMobile = mobile;
                    if (UserLogic.UpdateUserInfo(UserPropertyOptions.USER_3, userInfo))
                        return Json(new ResultModel(ApiStatusCode.OK, "修改成功"));
                }
                return Json(new ResultModel(ApiStatusCode.无效验证码));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("ChanageMobile user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 我的现金券列表 POST: user/MyCashCouponList
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult MyCashCouponList()
        {
            try
            {
                int userId = GetAuthUserId();
                var data = UserLogic.getMyCashCouponList(userId);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["list"] = data;
                return Json(new ResultModel(ApiStatusCode.OK, dict));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("MyCashCouponList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }



        /// <summary>
        ///发送优惠券 POST: user/SendCashCoupon
        /// </summary>
        /// <param name="couponId">优惠券ID</param>
        /// <param name="toUserId">接收用户ID,如果是自己转发，则为0</param>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult SendCashCoupon(int couponId, int toUserId = 0)
        {
            try
            {
                var user = GetUserData();

                if (CouponLogic.IsSendCouponByUserId(user.UserId, couponId))
                    return Json(new ResultModel(ApiStatusCode.您已转发));

                bool flag = CouponLogic.AddSendCoupon(user.UserId, user.UserIdentity, toUserId, couponId);
                if (flag)
                    return Json(new ResultModel(ApiStatusCode.OK, "转发成功"));
                else
                    return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("MyCashCouponList user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }

        }


        /// <summary>
        /// 给盟友发送优惠券 post: user/SendAllyCashCoupon
        /// </summary>
        /// <param name="couponId">优惠券ID</param>
        /// <param name="ids">盟友ID，多个用|隔开</param>
        /// <returns>ActionResult.</returns>
        [ActionAuthorize]
        public ActionResult SendAllyCashCoupon(int couponId, string ids)
        {
            try
            {
                var user = GetUserData();
                string[] TargetIds = null;


                if (CouponLogic.IsSendCouponByUserId(user.UserId, couponId))
                    return Json(new ResultModel(ApiStatusCode.您已转发));

                //如果是盟主身份，则需要判断发送目标
                if (user.UserIdentity == 1 && !string.IsNullOrEmpty(ids))
                {
                    if (string.IsNullOrEmpty(ids))
                        return Json(new ResultModel(ApiStatusCode.缺少发送目标));

                    TargetIds = ids.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (TargetIds.Length <= 0)
                        return Json(new ResultModel(ApiStatusCode.缺少发送目标));


                    bool flag = CouponLogic.AddSendAllyCoupon(user.UserId, couponId, TargetIds);
                    if (flag)
                        return Json(new ResultModel(ApiStatusCode.OK, "转发成功"));
                    else
                        return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
                }
                else
                    return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("SendAllyCashCoupon user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }


        /// <summary>
        /// 我的业务 POST: user/MyBusiness         
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult MyBusiness()
        {
            try
            {
                var user = GetUserData();
                var data = UserLogic.MyBusinessAmount(user.UserId, user.UserIdentity);
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("MyBusiness user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 盟友首页汇总 POST: user/AllyHomeSummary
        /// </summary>
        /// <returns><![CDATA[{status:200,statusText:"OK",data:{}}]]></returns>
        [ActionAuthorize]
        public ActionResult AllyHomeSummary()
        {
            try
            {
                var user = GetUserData();

                MyAllyIndexModel data = UserLogic.GetUserRank(user.UserId, user.BelongOne);
                if (data == null) data = new MyAllyIndexModel();
                data.OrderSuccessAmount = user.OrderSuccessAmount;
                data.CustomerAmount = user.CustomerAmount;
                data.AllyAmount = UserLogic.GetAllyCount(user.BelongOne);
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AllyHomeSummary user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }

        }

        /// <summary>
        /// 消息提醒 POST: user/remind
        /// </summary>
        /// <returns>ActionResult.</returns>
        [ActionAuthorize]
        public ActionResult Remind()
        {
            try
            {
                var user = GetUserData();

                Dictionary<string, object> data = new Dictionary<string, object>();

                //我的留言未读数量
                data["messageCount"] = ArticleLogic.GetNotReadMessageCount(user.UserId, 2);
                //我的消息-发送消息未读数量
                data["messagePushCount"] = ArticleLogic.GetNotReadMessageCount(user.UserId, 0);
                //我的消息-接收消息未读数量
                data["messagePullCount"] = ArticleLogic.GetNotReadMessageCount(user.UserId, 1);

                bool isbusiness = false;
                if (user.UserIdentity == 1)
                {
                    //客户数量
                    int customerAmount = CustomerLogic.GetCustomerCount(user.UserId, user.UserIdentity, 0);
                    //兑换数量
                    int exchangeAmount = UserLogic.GetConvertCount(user.UserId, 0);

                    int allyApplyAmount = UserLogic.AllyApplyCount(user.UserId);
                    if (customerAmount > 0 || exchangeAmount > 0 || allyApplyAmount > 0)
                        isbusiness = true;
                }
                data["businessRemind"] = isbusiness;
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("Remind user:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }

        /// <summary>
        /// 我的工作汇报列表 POST: user/reportlist
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        [ActionAuthorize]
        public ActionResult reportlist(int pageIndex, int pageSize)
        {
            try
            {
                var userId = GetAuthUserId();
                var data = UserLogic.GetAppUserReportList(userId, pageIndex, pageSize);
                return Json(new ResultModel(ApiStatusCode.OK, data));
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("reportlist:message:{0},StackTrace:{1}", ex.Message, ex.StackTrace), LogHelperTag.ERROR);
                return Json(new ResultModel(ApiStatusCode.SERVICEERROR));
            }
        }
    }
}
