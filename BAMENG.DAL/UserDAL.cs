﻿/*
    版权所有:杭州火图科技有限公司
    地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
    (c) Copyright Hangzhou Hot Technology Co., Ltd.
    Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
    2013-$today.year. All rights reserved.
**/


using BAMENG.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAMENG.MODEL;
using HotCoreUtils.DB;
using System.Data.SqlClient;
using System.Transactions;
using BAMENG.CONFIG;
using System.Data;
using HotCoreUtils.Helper;

namespace BAMENG.DAL
{
    public class UserDAL : AbstractDAL, IUserDAL
    {

        private static OrderDAL order = new OrderDAL();
        private static CustomerDAL cus = new CustomerDAL();

        /// <summary>
        /// 获取用户基本信息SQL 语句
        /// </summary>
        //private const string APP_USER_SELECT = @"select ue.UserId,ue.UserIdentity,U.UB_UserCity as UserCity,U.UB_UserGender as UserGender,ue.MerchantID,ue.ShopId,ue.IsActive,ue.Score,ue.ScoreLocked,ue.MengBeans,ue.MengBeansLocked,ue.CreateTime
        //                    ,U.UB_UserLoginName as LoginName,U.UB_UserRealName as RealName,U.UB_UserNickName as NickName,U.UB_UserMobile as UserMobile,U.UB_WxHeadImg as UserHeadImg
        //                    ,S.ShopName,S.ShopProv,S.ShopCity,L.UL_LevelName as LevelName,U.UB_BelongOne as BelongOne,S.ShopType,S.ShopBelongId,ue.CustomerAmount,ue.OrderSuccessAmount,S.IsActive as ShopActive,'' as BelongOneUserName
        //                     from BM_User_extend ue
        //                    inner join Hot_UserBaseInfo U with(nolock) on U.UB_UserID =ue.UserId
        //                    left join BM_ShopManage S with(nolock) on S.ShopID=ue.ShopId
        //                    left join Mall_UserLevel L on L.UL_ID=U.UB_LevelID 
        //                    where 1=1 and  U.UB_IsDelete=0 ";
        private const string APP_USER_SELECT = @"select ue.UserId,ue.UserIdentity,U.UB_UserCity as UserCity,U.UB_UserGender as UserGender,ue.MerchantID,ue.ShopId,ue.IsActive,ue.Score,ue.ScoreLocked,ue.MengBeans,ue.MengBeansLocked,ue.CreateTime
                            ,U.UB_UserLoginName as LoginName,U.UB_UserRealName as RealName,U.UB_UserNickName as NickName,U.UB_UserMobile as UserMobile,U.UB_WxHeadImg as UserHeadImg
                            ,S.ShopName,S.ShopProv,S.ShopCity,L.UL_LevelName as LevelName,U.UB_BelongOne as BelongOne,S.ShopType,S.ShopBelongId,ue.CustomerAmount,ue.OrderSuccessAmount,S.IsActive as ShopActive,U2.UB_UserRealName as BelongOneUserName
                             from BM_User_extend ue
                            inner join Hot_UserBaseInfo U with(nolock) on U.UB_UserID =ue.UserId
                            left join Hot_UserBaseInfo U2 with(nolock) on U2.UB_UserID =U.UB_BelongOne
                            left join BM_ShopManage S with(nolock) on S.ShopID=ue.ShopId
                            left join Mall_UserLevel L on L.UL_ID=U.UB_LevelID 
                            where 1=1 and  U.UB_IsDelete=0 ";

        private const string APP_USER_ALLY_SELECT = @"select ue.UserId,ue.UserIdentity,U.UB_UserCity as UserCity,U.UB_UserGender as UserGender,ue.MerchantID,ue.ShopId,ue.IsActive,ue.Score,ue.ScoreLocked,ue.MengBeans,ue.MengBeansLocked,ue.CreateTime
                            ,U.UB_UserLoginName as LoginName,U.UB_UserRealName as RealName,U.UB_UserNickName as NickName,U.UB_UserMobile as UserMobile,U.UB_WxHeadImg as UserHeadImg
                            ,S.ShopName,S.ShopProv,S.ShopCity,L.UL_LevelName as LevelName,U.UB_BelongOne as BelongOne,S.ShopType,S.ShopBelongId,ue.CustomerAmount,ue.OrderSuccessAmount,S.IsActive as ShopActive,U2.UB_UserRealName as BelongOneUserName
                             from BM_User_extend ue
                            inner join Hot_UserBaseInfo U with(nolock) on U.UB_UserID =ue.UserId
                            left join Hot_UserBaseInfo U2 with(nolock) on U2.UB_UserID =U.UB_BelongOne
                            left join BM_ShopManage S with(nolock) on S.ShopID=ue.ShopId
                            left join Mall_UserLevel L on L.UL_ID=U.UB_LevelID 
                            where 1=1 and  U.UB_IsDelete=0 ";




        /// <summary>
        /// 添加用户（盟主或盟友）
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="loginname"></param>
        /// <param name="username"></param>
        /// <param name="belongOne"></param>
        /// <returns></returns>
        public int AddUserInfo(UserRegisterModel model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                int userId = AddUserBaseInfoModel(model);
                if (userId <= 0)
                    return userId;
                string strSql = "insert into BM_User_extend(UserId,UserIdentity,MerchantID,ShopId,BelongShopId) values(@UserId,@UserIdentity,@MerchantID,@ShopId,@BelongShopId);select @@IDENTITY";

                var param = new[] {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@UserIdentity",model.UserIdentity),
                        new SqlParameter("@MerchantID",model.storeId),
                        new SqlParameter("@ShopId", model.ShopId),
                        new SqlParameter("@BelongShopId", model.BelongShopId>0?model.BelongShopId:model.ShopId)
                        };
                object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param);
                int flag = 0;
                if (obj == null)
                    flag = 0;
                else
                    flag = Convert.ToInt32(obj);
                if (flag > 0)
                {
                    scope.Complete();
                    return userId;
                }
                else
                    return 0;
            }
        }


        public bool IsExist(string loginName)
        {
            string strSql = "select COUNT(1) from Hot_UserBaseInfo U with(nolock) where U.UB_UserLoginName=@UB_UserLoginName and U.UB_CustomerID=@UB_CustomerID";
            var param = new[] {
                new SqlParameter("@UB_UserLoginName",loginName),
                new SqlParameter("@UB_CustomerID",ConstConfig.storeId),
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param)) > 0;
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="ShopId"></param>
        /// <param name="UserIdentity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultPageModel GetUserList(int ShopId, int UserIdentity, SearchModel model)
        {
            ResultPageModel result = new ResultPageModel();
            if (model == null)
                return result;
            string strSql = (UserIdentity == 1 ? APP_USER_SELECT : APP_USER_ALLY_SELECT) + " and U.UB_CustomerID=" + ConstConfig.storeId;

            if (!string.IsNullOrEmpty(model.key))
            {
                switch (model.searchType)
                {
                    case (int)SearchType.姓名:
                        strSql += string.Format(" and U.UB_UserRealName like '%{0}%' ", model.key);
                        break;
                    case (int)SearchType.昵称:
                        strSql += string.Format(" and U.UB_UserNickName like '%{0}%' ", model.key);
                        break;
                    case (int)SearchType.手机:
                        strSql += " and U.UB_UserMobile=@UserMobile ";
                        break;
                    case (int)SearchType.门店:
                        strSql += string.Format(" and S.ShopName like '%{0}%' ", model.key);
                        break;
                    default:
                        break;
                }
            }
            strSql += " and ue.UserIdentity=@UserIdentity ";
            if (ShopId > 0)
            {
                if (model.type == 1)
                    strSql += " and (ue.ShopId=@ShopId or ue.BelongShopId=@ShopId) ";
                else
                    strSql += " and ue.ShopId=@ShopId ";
            }
            if (!string.IsNullOrEmpty(model.startTime))
                strSql += " and CONVERT(nvarchar(10),ue.CreateTime,121)>=CONVERT(nvarchar(10),@startTime,121) ";
            if (!string.IsNullOrEmpty(model.endTime))
                strSql += " and CONVERT(nvarchar(10),ue.CreateTime,121)<=CONVERT(nvarchar(10),@endTime,121) ";
            var param = new[] {
                new SqlParameter("@startTime", model.startTime),
                new SqlParameter("@endTime", model.endTime),
                new SqlParameter("@UserMobile", model.key),
                new SqlParameter("@UserIdentity", UserIdentity),
                new SqlParameter("@ShopId",ShopId)
            };
            //生成sql语句
            return getPageData<UserModel>(model.PageSize, model.PageIndex, strSql, "ue.CreateTime", param);
        }



        /// <summary>
        /// 登录流水
        /// </summary>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        public ResultPageModel GetUserLoginList(int shopId, int userIdentity, SearchModel model)
        {

            string strSql = @"select l.LoginTime,s.ShopName,u.UB_UserRealName from BM_UserLoginLog l with(nolock)
                                left join BM_ShopManage s with(nolock) on s.ShopID=l.ShopId
                                left join Hot_UserBaseInfo u with(nolock) on u.UB_UserID=l.UserId
                                where 
                                CONVERT(nvarchar(10),l.LoginTime,121)>=@startTime
                                and CONVERT(nvarchar(10),l.LoginTime,121)<=@endTime
                                and l.UserIdentity=1 
                            ";

            if (userIdentity == 1)
                strSql += " and l.BelongShopId=@ShopId";
            else if (userIdentity == 2)
                strSql += " and l.ShopId=@ShopId";

            var param = new[] {
                new SqlParameter("@startTime",model.startTime),
                new SqlParameter("@endTime",model.endTime),
                new SqlParameter("@ShopId",shopId)
            };
            //生成sql语句
            return getPageData<UserLoginModel>(model.PageSize, model.PageIndex, strSql, "l.LoginTime", param);
        }


        /// <summary>
        /// 获取签到流水
        /// </summary>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        public ResultPageModel GetSignLoginList(int shopId, int userIdentity, SearchModel model)
        {

            string strSql = @"select u.UB_UserRealName,s.CreateTime as LoginTime,s.UserAddress as ShopName from BM_UserSignLog s
                                left join Hot_UserBaseInfo u with(nolock) on u.UB_UserID=s.UserId
                                where 
                                CONVERT(nvarchar(10),s.CreateTime,121)>=@startTime
                                and CONVERT(nvarchar(10),s.CreateTime,121)<=@endTime                                 
                            ";

            if (userIdentity == 1)
                strSql += " and s.BelongOneShopId=@ShopId";
            else if (userIdentity == 2)
                strSql += " and s.ShopId=@ShopId";

            var param = new[] {
                new SqlParameter("@startTime",model.startTime),
                new SqlParameter("@endTime",model.endTime),
                new SqlParameter("@ShopId",shopId)
            };
            //生成sql语句
            return getPageData<UserLoginModel>(model.PageSize, model.PageIndex, strSql, "s.CreateTime", param);
        }



        /// <summary>
        /// 获取用户实体信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public UserModel GetUserModel(int UserId)
        {
            string strSql = APP_USER_SELECT + " and ue.UserId=@UserId";
            var param = new[] {
                        new SqlParameter("@UserId", UserId)
                        };
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, param))
            {
                var data = DbHelperSQLP.GetEntity<UserModel>(dr);
                return data;
            }

        }



        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if the specified user identifier is exist; otherwise, false.</returns>
        public bool IsExist(int userId)
        {
            string strSql = "select COUNT(1) from BM_User_extend where UserId=@UserId";
            var param = new[] {
                        new SqlParameter("@UserId", userId)
                        };

            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param)) > 0;
        }

        /// <summary>
        /// 添加商城用户
        /// 作者：郭孟稳
        /// 时间：2016.07.11
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <param name="belongOne"></param>
        /// <returns></returns>
        private int AddUserBaseInfoModel(UserRegisterModel regModel, int levelId = 0)
        {
            if (UserExist(regModel.loginName, regModel.storeId))
                return -1;
            UserBaseInfoModel model = new UserBaseInfoModel();
            model.UB_UserLoginName = regModel.loginName;
            model.UB_UserLoginPassword = regModel.loginPassword;
            model.UB_CustomerID = regModel.storeId;
            model.UB_UserMobile = regModel.mobile;
            model.UB_UserType = regModel.UserIdentity;
            model.UB_GroupId = 0;
            model.UB_RebateEnabled = 0;
            model.WxNickName = regModel.nickname;//QQ昵称对应微信昵称
            model.UB_UserRealName = regModel.username;
            model.UB_UserNickName = regModel.nickname;
            model.UB_UserGender = regModel.userGender;


            model.UB_ShareCount = 0; //分享机会赠送
            model.UB_InviteCount = 0;
            model.UB_UserEmail = "";
            model.UB_UserCardID = "";
            model.UB_UserCity = "";
            model.UB_UserProvince = "";
            model.BelongName = "";
            model.LevelName = "";
            model.ParentName = "";
            model.PayPassword = "";
            model.UB_SourcePath = "";
            model.UB_StoreAddr = "";
            model.UB_UserAddress = "";
            model.UB_UserArea = "";
            model.UB_UserBirthday = "";
            model.UB_UserFace = "";
            model.UB_UserIncome = "";
            model.UserAllArea = "";
            model.WxHeadImg = "";
            model.UB_AccountSrc = 0;
            model.UB_MobileToBeBind = 0;
            model.UB_ShareTaskID = 0;
            model.UB_ShareTaskType = 0;

            UserRelationViewEntity sourceModel = null;
            if (regModel.belongOne > 0)
            {
                model.UB_SourceID = regModel.belongOne;
                model.UB_SourceDesc = "我引导注册";
                sourceModel = GetRelationInfoPlus(regModel.belongOne);
            }
            else
            {
                model.UB_SourceDesc = "管理员后台添加";
            }
            if (sourceModel != null)
            {
                model.UB_BelongOne = sourceModel.UserId;
                model.UB_BelongTwo = sourceModel.BelongOne;
                model.UB_BelongThree = sourceModel.BelongTwo;
                model.UB_SourceID = sourceModel.UserId;
                model.UB_ParentID = 0;
            }
            if (levelId == 0)
                model.UB_LevelID = GetMinLevelID(regModel.storeId, model.UB_UserType);
            else
                model.UB_LevelID = levelId;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into Hot_UserBaseInfo(");
                strSql.Append("UB_UserMobile,UB_UserEmail,UB_UserCardID,UB_UserProvince,UB_UserCity,UB_UserArea,UB_UserAddress,UB_UserLoginPassword,UB_UserNickName,UB_UserFace,UB_UserRealName,UB_UserAge,UB_UserIncome,UB_IsDelete,UB_CustomerID,UB_UserBirthday,UB_HasCard,UB_UserLoginName,UB_LevelID,UB_UserType,UB_BelongTo,UB_SourceID,UB_SourceDesc,UB_ShareCount,UB_InviteCount,UB_ShareTaskID,UB_ShareTaskType,UB_StoreAddr,UB_IsStore,UB_ParentID,UB_SourcePath,UB_SourceDepth,UB_BelongOne,UB_BelongTwo,UB_BelongThree,UB_UserGroupId,UB_AccountSrc,UB_MobileToBeBind,UB_WxNickName,UB_WxHeadImg,UB_UserGender");
                strSql.Append(") values (");
                strSql.Append("@UB_UserMobile,@UB_UserEmail,@UB_UserCardID,@UB_UserProvince,@UB_UserCity,@UB_UserArea,@UB_UserAddress,@UB_UserLoginPassword,@UB_UserNickName,@UB_UserFace,@UB_UserRealName,@UB_UserAge,@UB_UserIncome,@UB_IsDelete,@UB_CustomerID,@UB_UserBirthday,@UB_HasCard,@UB_UserLoginName,@UB_LevelID,@UB_UserType,@UB_BelongTo,@UB_SourceID,@UB_SourceDesc,@UB_ShareCount,@UB_InviteCount,@UB_ShareTaskID,@UB_ShareTaskType,@UB_StoreAddr,@UB_IsStore,@UB_ParentID,@UB_SourcePath,@UB_SourceDepth,@UB_BelongOne,@UB_BelongTwo,@UB_BelongThree,@UB_UserGroupId,@UB_AccountSrc,@UB_MobileToBeBind,@UB_WxNickName,@UB_WxHeadImg,@UB_UserGender");
                strSql.Append(") ");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
                        new SqlParameter("@UB_UserMobile", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserEmail", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserCardID", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserProvince", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserCity", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserArea", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserAddress", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserLoginPassword", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserNickName", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserFace", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserRealName", SqlDbType.NVarChar) ,
                        new SqlParameter("@UB_UserAge", SqlDbType.Int) ,
                        new SqlParameter("@UB_UserIncome", SqlDbType.NVarChar),
                        new SqlParameter("@UB_IsDelete",SqlDbType.Int),
                        new SqlParameter("@UB_CustomerID",SqlDbType.Int),
                        new SqlParameter("@UB_UserBirthday",SqlDbType.NVarChar),
                        new SqlParameter("@UB_HasCard",SqlDbType.Int),
                        new SqlParameter("@UB_UserLoginName",SqlDbType.NVarChar),
                        new SqlParameter("@UB_LevelID",SqlDbType.Int),
                        new SqlParameter("@UB_UserType",SqlDbType.Int),
                        new SqlParameter("@UB_BelongTo",SqlDbType.Int),
                        new SqlParameter("@UB_SourceID",SqlDbType.Int),
                        new SqlParameter("@UB_SourceDesc",SqlDbType.NVarChar),
                        new SqlParameter("@UB_ShareCount",SqlDbType.Int),
                        new SqlParameter("@UB_InviteCount",SqlDbType.Int),
                        new SqlParameter("@UB_ShareTaskID",SqlDbType.Int),
                        new SqlParameter("@UB_ShareTaskType",SqlDbType.Int),
                        new SqlParameter("@UB_StoreAddr",SqlDbType.NVarChar),
                        new SqlParameter("@UB_IsStore",SqlDbType.Int),
                        new SqlParameter("@UB_ParentID",SqlDbType.Int),
                        new SqlParameter("@UB_SourcePath",SqlDbType.NVarChar),
                        new SqlParameter("@UB_SourceDepth",SqlDbType.Int),
                        new SqlParameter("@UB_BelongOne", SqlDbType.Int),
                        new SqlParameter("@UB_BelongTwo", SqlDbType.Int),
                        new SqlParameter("@UB_BelongThree", SqlDbType.Int),
                        new SqlParameter("@UB_UserGroupId", SqlDbType.Int),
                        new SqlParameter("@UB_AccountSrc", SqlDbType.Int),
                        new SqlParameter("@UB_MobileToBeBind", SqlDbType.Int),
                        new SqlParameter("@UB_WxNickName", SqlDbType.VarChar),
                        new SqlParameter("@UB_WxHeadImg", SqlDbType.VarChar),
                        new SqlParameter("@UB_UserGender",model.UB_UserGender)
                };
                parameters[0].Value = model.UB_UserMobile;
                parameters[1].Value = model.UB_UserEmail;
                parameters[2].Value = model.UB_UserCardID;
                parameters[3].Value = model.UB_UserProvince;
                parameters[4].Value = model.UB_UserCity;
                parameters[5].Value = model.UB_UserArea;
                parameters[6].Value = model.UB_UserAddress;
                parameters[7].Value = model.UB_UserLoginPassword;
                parameters[8].Value = model.UB_UserNickName;
                parameters[9].Value = model.UB_UserFace;
                parameters[10].Value = model.UB_UserRealName;
                parameters[11].Value = model.UB_UserAge;
                parameters[12].Value = model.UB_UserIncome;
                parameters[13].Value = model.UB_IsDelete;
                parameters[14].Value = model.UB_CustomerID;
                parameters[15].Value = model.UB_UserBirthday;
                parameters[16].Value = model.UB_HasCard;
                parameters[17].Value = model.UB_UserLoginName;
                parameters[18].Value = model.UB_LevelID;
                parameters[19].Value = model.UB_UserType;
                parameters[20].Value = model.UB_BelongTo;
                parameters[21].Value = model.UB_SourceID;
                parameters[22].Value = model.UB_SourceDesc;
                parameters[23].Value = model.UB_ShareCount;
                parameters[24].Value = model.UB_InviteCount;
                parameters[25].Value = model.UB_ShareTaskID;
                parameters[26].Value = model.UB_ShareTaskType;
                parameters[27].Value = model.UB_StoreAddr;
                parameters[28].Value = model.UB_IsStore;
                parameters[29].Value = model.UB_ParentID;
                parameters[30].Value = model.UB_SourcePath;
                parameters[31].Value = model.UB_SourceDepth;
                parameters[32].Value = model.UB_BelongOne;
                parameters[33].Value = model.UB_BelongTwo;
                parameters[34].Value = model.UB_BelongThree;
                parameters[35].Value = model.UB_UserGroupId;
                parameters[36].Value = model.UB_AccountSrc;
                parameters[37].Value = model.UB_MobileToBeBind;
                parameters[38].Value = model.WxNickName;
                parameters[39].Value = model.WxHeadImg;

                object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    int userId = Convert.ToInt32(obj);

                    MemberChangeLogModel changeLog = new MemberChangeLogModel()
                    {
                        Member_Id = userId,
                        Change_Type = 5,
                        Remark = "会员注册",
                        Add_Time = DateTime.Now,
                        Customer_Id = regModel.storeId,
                        BelongOne = regModel.belongOne,
                        BelongTwo = model.UB_BelongTwo,
                        BelongThree = model.UB_BelongThree,
                        ParentId = model.UB_ParentID,
                        GroupId = 0,
                        LevelId = model.UB_LevelID,
                        BeforeBelongOne = 0,
                        BeforeBelongTwo = 0,
                        BeforeBelongThree = 0,
                        BeforeParentId = 0,
                        BeforeGroupId = 0,
                        BeforeLevelId = 0,
                        Reason = "邀请注册"
                    };

                    if (userId > 0)
                        AddRegisterLog(changeLog);
                    return userId;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("AddUserBaseInfoModel:{0}", ex), LogHelperTag.ERROR, WebConfig.debugMode());
                return 0;
            }
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool UserExist(string mobile, int storeId)
        {
            string strSql = "select COUNT(1) from Hot_UserBaseInfo with(nolock) where UB_UserLoginName=@LoginName and UB_CustomerID=@CustomerID";
            var param = new[] {
                new SqlParameter("@LoginName",mobile),
                new SqlParameter("@CustomerID",storeId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param)) > 0;
        }
        /// <summary>
        /// 获取最小商城等级
        /// 作者：郭孟稳        
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetMinLevelID(int customerid, int type)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT TOP 1 UL_ID FROM Mall_UserLevel WHERE UL_CustomerID={0} and UL_Type={1}  order by UL_MemberNum", customerid, type);
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, sql.ToString());
            if (obj == null)
                return 0;
            else
                return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 获取用户关联信息新版
        /// </summary>
        /// <param name="userId">会员ID</param>
        /// <returns></returns>
        public UserRelationViewEntity GetRelationInfoPlus(int userId)
        {
            string sql = @"select 
                           UB_UserLoginName,
                           ISNULL(UB_BelongOne,0) AS UB_BelongOne,
                           ISNULL(UB_BelongTwo,0) as UB_BelongTwo ,
                           ISNULL(UB_BelongThree,0) AS UB_BelongThree,         
                           UB_CustomerId
                           from Hot_UserBaseInfo  with(nolock) where UB_UserID=@UB_UserID";
            var parameters = new[] {
                    new SqlParameter("@UB_UserID", userId)};

            DataTable dt = DbHelperSQLP.GetDataTable(WebConfig.getConnectionString(), CommandType.Text, sql, parameters);
            if (dt.Rows.Count == 0)
            {
                return new UserRelationViewEntity() { UserId = -1 };
            }
            UserRelationViewEntity entity = new UserRelationViewEntity();
            entity.UserId = userId;
            entity.LoginName = dt.Rows[0]["UB_UserLoginName"].ToString();
            entity.BelongOne = Convert.ToInt32(dt.Rows[0]["UB_BelongOne"].ToString());
            entity.BelongTwo = Convert.ToInt32(dt.Rows[0]["UB_BelongTwo"].ToString());
            entity.BelongThree = Convert.ToInt32(dt.Rows[0]["UB_BelongThree"].ToString());
            entity.CustomerId = Convert.ToInt32(dt.Rows[0]["UB_CustomerId"]);
            entity.UB_CustomerID = Convert.ToInt32(dt.Rows[0]["UB_CustomerID"]);
            return entity;
        }

        /// <summary>
        /// 增加一条注册日志数据
        /// 作者：郭孟稳        
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="userId"></param>
        /// <param name="LevelId"></param>
        /// <param name="belongOne"></param>
        /// <returns></returns>
        private int AddRegisterLog(MemberChangeLogModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"insert into Mall_Member_ChangeLog (Member_Id,Change_Type,Remark,Add_Time,Customer_Id
                       ,BelongOne,BelongTwo,BelongThree,BeforeBelongOne,BeforeBelongTwo,BeforeBelongThree,ParentId,BeforeParentId,Reason)
                        values (@Member_Id,@Change_Type,@Remark,@Add_Time,@Customer_Id
                        ,@BelongOne,@BelongTwo,@BelongThree,@BeforeBelongOne,@BeforeBelongTwo,@BeforeBelongThree,@ParentId,@BeforeParentId,@Reason) 
                        select @@IDENTITY");
            var parm = new[] {
                new SqlParameter("@Member_Id", model.Member_Id),
                new SqlParameter("@Change_Type", model.Change_Type),
                new SqlParameter("@Remark", model.Remark),
                new SqlParameter("@Add_Time", model.Add_Time),
                new SqlParameter("@Customer_Id", model.Customer_Id),
                new SqlParameter("@BelongOne", model.BelongOne),
                new SqlParameter("@BelongTwo", model.BelongTwo),
                new SqlParameter("@BelongThree", model.BelongThree),
                new SqlParameter("@BeforeBelongOne", model.BeforeBelongOne),
                new SqlParameter("@BeforeBelongTwo", model.BeforeBelongTwo),
                new SqlParameter("@BeforeBelongThree", model.BeforeBelongThree),
                new SqlParameter("@ParentId", model.ParentId),
                new SqlParameter("@BeforeParentId", model.BeforeParentId),
                new SqlParameter("@Reason", model.Reason)
            };

            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parm);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserInfo(UserRegisterModel model)
        {
            string strSql = "update Hot_UserBaseInfo set UB_UserRealName=@UB_UserRealName,UB_UserNickName=@UB_UserNickName,UB_UserMobile=@UB_UserMobile";
            if (!string.IsNullOrEmpty(model.loginPassword))
                strSql += ",UB_UserLoginPassword =@UB_UserLoginPassword";
            strSql += " where UB_UserID=@UserID";
            var parm = new[] {
                new SqlParameter("@UB_UserRealName", model.username),
                new SqlParameter("@UB_UserNickName", model.nickname),
                new SqlParameter("@UB_UserMobile", model.mobile),
                new SqlParameter("@UB_UserLoginPassword", model.loginPassword),
                new SqlParameter("@UserID", model.UserId),
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parm) > 0;
        }

        public bool UpdateUserInfo(UserPropertyOptions opt, UserModel model)
        {
            string strSql = "update Hot_UserBaseInfo set ";

            switch (opt)
            {
                case UserPropertyOptions.USER_1:
                    strSql += " UB_WxHeadImg=@UB_WxHeadImg";
                    break;
                case UserPropertyOptions.USER_2:
                    strSql += " UB_UserNickName=@UB_UserNickName";
                    break;
                case UserPropertyOptions.USER_3:
                    strSql += " UB_UserMobile=@UB_UserMobile";
                    break;
                case UserPropertyOptions.USER_4:
                    strSql += " UB_UserRealName=@UB_UserRealName";
                    break;
                case UserPropertyOptions.USER_5:
                    strSql += " UB_UserGender=@UB_UserGender";
                    break;
                case UserPropertyOptions.USER_6:
                    strSql += " UB_UserCity=@UB_UserCity";
                    break;
                default:
                    strSql += " UB_UserAge=UB_UserAge";
                    break;
            }
            strSql += " where UB_UserID=@UserID";
            var parm = new[] {
                new SqlParameter("@UB_WxHeadImg", model.UserHeadImg),
                new SqlParameter("@UB_UserNickName", model.NickName),
                new SqlParameter("@UB_UserMobile", model.UserMobile),
                new SqlParameter("@UB_UserRealName", model.RealName),
                new SqlParameter("@UB_UserCity", model.UserCity),
                new SqlParameter("@UB_UserGender", model.UserGender),
                new SqlParameter("@UserID", model.UserId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parm) > 0;
        }


        /// <summary>
        /// 冻结/解冻账户
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool UpdateUserActive(int userId, int active)
        {
            string strSql = "update BM_User_extend set IsActive=@IsActive where UserId=@UserId";
            var param = new[] {
                        new SqlParameter("@IsActive",active),
                        new SqlParameter("@UserId",userId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public bool DeleltUserInfo(int userId)
        {
            string strSql = "update Hot_UserBaseInfo set UB_IsDelete=1 where UB_UserID=@UB_UserID";
            var param = new[] {
                        new SqlParameter("@UB_UserID",userId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }


        /// <summary>
        ///获取用户等级名称
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.String.</returns>
        public string GetUserLevelName(int userId)
        {
            string strSql = @"select l.UL_LevelName from Hot_UserBaseInfo u with(nolock)
                                left join Mall_UserLevel l on u.UB_LevelID = l.UL_ID
                                where u.UB_UserID = @UB_UserID";
            var param = new[] {
                        new SqlParameter("@UB_UserID",userId)
            };
            return DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param).ToString();
        }


        /// <summary>
        /// 获取他的盟友列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultPageModel GetAllyList(SearchModel model)
        {
            ResultPageModel result = new ResultPageModel();
            string strSql = APP_USER_SELECT;

            strSql += " and ue.UserIdentity=0 and U.UB_BelongOne=@UB_BelongOne";


            string orderFiled = "ue.CreateTime";
            switch (model.orderbyCode)
            {
                case 0:
                    orderFiled = "U.UB_LevelID";
                    break;
                case 1:
                    orderFiled = "ue.CustomerAmount";
                    break;
                case 2:
                    orderFiled = "ue.OrderSuccessAmount";
                    break;
                default:
                    break;
            }
            bool isDesc = model.IsDesc ? false : true;

            var param = new[] {
                new SqlParameter("@UB_BelongOne",model.UserId),
            };

            //生成sql语句
            return getPageData<UserModel>(model.PageSize, model.PageIndex, strSql, orderFiled, param, (items =>
            {
                items.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.UserHeadImg))
                        item.UserHeadImg = WebConfig.reswebsite() + item.UserHeadImg;

                    item.UserGender = item.UserGender.ToUpper();
                });
            }), isDesc);
        }









        /// <summary>
        /// 获取等级列表
        /// 作者：郭孟稳        
        /// </summary>
        ///<param name="storeId"></param>
        ///<param name="type">0盟友，1盟主</param>
        /// <returns></returns>
        public ResultPageModel GetLevelList(int storeId, int type)
        {
            string strSql = @"select UL.* from Mall_UserLevel UL where UL.UL_CustomerID=@UL_CustomerID and UL.UL_Type=@UL_Type ";
            var param = new[] {
                    new SqlParameter("@UL_CustomerID", storeId),
                    new SqlParameter("@UL_Type", type)
            };
            return getPageData<MallUserLevelModel>(50, 1, strSql, "UL.UL_MemberNum", true, param);
        }


        /// <summary>
        /// 添加等级
        /// 作者：郭孟稳
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertLevel(MallUserLevelModel model)
        {
            string strsql = @"insert into Mall_UserLevel (UL_Level,UL_LevelName,UL_Type,UL_CustomerID,UL_Description,UL_DefaultLevel,UL_Integral
                                ,UL_MemberNum,UL_DirectTeamNum,UL_IndirectTeamNum,UL_Money,UL_OpenLevel_One,UL_BelongOne_Content,UL_OpenLevel_Two,UL_BelongTwo_Content,UL_GuidetLevel,UL_Gold)
                                values (@UL_Level,@UL_LevelName,@UL_Type,@UL_CustomerID,@UL_Description,@UL_DefaultLevel,@UL_Integral
                                ,@UL_MemberNum,@UL_DirectTeamNum,@UL_IndirectTeamNum,@UL_Money,@UL_OpenLevel_One,@UL_BelongOne_Content,@UL_OpenLevel_Two,@UL_BelongTwo_Content,@UL_GuidetLevel,@UL_Gold)
                                select @@IDENTITY";
            SqlParameter[] parm = {
                    new SqlParameter("@UL_Level", model.UL_Level),
                    new SqlParameter("@UL_LevelName", model.UL_LevelName),
                    new SqlParameter("@UL_Type", model.UL_Type),
                    new SqlParameter("@UL_CustomerID", model.UL_CustomerID),
                    new SqlParameter("@UL_Description", model.UL_Description),
                    new SqlParameter("@UL_DefaultLevel", model.UL_DefaultLevel),
                    new SqlParameter("@UL_Integral", model.UL_Integral),
                    new SqlParameter("@UL_MemberNum", model.UL_MemberNum),
                    new SqlParameter("@UL_DirectTeamNum", model.UL_DirectTeamNum),
                    new SqlParameter("@UL_IndirectTeamNum", model.UL_IndirectTeamNum),
                    new SqlParameter("@UL_Money", model.UL_Money),
                    new SqlParameter("@UL_OpenLevel_One", model.UL_OpenLevel_One),
                    new SqlParameter("@UL_BelongOne_Content", model.UL_BelongOne_Content),
                    new SqlParameter("@UL_OpenLevel_Two", model.UL_OpenLevel_Two),
                    new SqlParameter("@UL_BelongTwo_Content", model.UL_BelongTwo_Content),
                    new SqlParameter("@UL_GuidetLevel", model.UL_GuidetLevel),
                    new SqlParameter("@UL_Gold",model.UL_Gold)
                    };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strsql, parm));
        }


        /// <summary>
        /// 修改等级
        /// 作者：郭孟稳
        /// 时间：2016.07.13
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateLevel(MallUserLevelModel model)
        {
            string strsql = @"update Mall_UserLevel set UL_LevelName=@UL_LevelName,UL_Type=@UL_Type,UL_CustomerID=@UL_CustomerID,UL_Description=@UL_Description,UL_DefaultLevel=@UL_DefaultLevel,UL_Integral=@UL_Integral,UL_MemberNum=@UL_MemberNum,UL_DirectTeamNum=@UL_DirectTeamNum,UL_IndirectTeamNum=@UL_IndirectTeamNum,UL_Money=@UL_Money,UL_OpenLevel_One=@UL_OpenLevel_One,UL_BelongOne_Content=@UL_BelongOne_Content,UL_OpenLevel_Two=@UL_OpenLevel_Two,UL_BelongTwo_Content=@UL_BelongTwo_Content,UL_GuidetLevel=@UL_GuidetLevel,UL_Gold=@UL_Gold where UL_ID=@UL_ID";
            SqlParameter[] parm = {
                    new SqlParameter("@UL_ID", model.UL_ID),
                    new SqlParameter("@UL_LevelName", model.UL_LevelName),
                    new SqlParameter("@UL_Type", model.UL_Type),
                    new SqlParameter("@UL_CustomerID", model.UL_CustomerID),
                    new SqlParameter("@UL_Description", model.UL_Description),
                    new SqlParameter("@UL_DefaultLevel", model.UL_DefaultLevel),
                    new SqlParameter("@UL_Integral", model.UL_Integral),
                    new SqlParameter("@UL_MemberNum", model.UL_MemberNum),
                    new SqlParameter("@UL_DirectTeamNum", model.UL_DirectTeamNum),
                    new SqlParameter("@UL_IndirectTeamNum", model.UL_IndirectTeamNum),
                    new SqlParameter("@UL_Money", model.UL_Money),
                    new SqlParameter("@UL_OpenLevel_One", model.UL_OpenLevel_One),
                    new SqlParameter("@UL_BelongOne_Content", model.UL_BelongOne_Content),
                    new SqlParameter("@UL_OpenLevel_Two", model.UL_OpenLevel_Two),
                    new SqlParameter("@UL_BelongTwo_Content", model.UL_BelongTwo_Content),
                    new SqlParameter("@UL_GuidetLevel", model.UL_GuidetLevel),
                    new SqlParameter("@UL_Gold",model.UL_Gold),

                    };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strsql, parm) == 1;
        }

        /// <summary>
        /// 删除等级
        /// 作者：郭孟稳
        /// 时间：2016.07.13
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool DeleteLevel(int levelId, int storeId)
        {
            string sql1 = "delete from Mall_UserLevel where UL_ID=@levelId";
            var param = new[] {
                new SqlParameter("@levelId",levelId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, sql1, param) > 0;
        }
        /// <summary>
        /// 获取等级信息
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="storeId">商户ID</param>
        /// <returns></returns>
        public MallUserLevelModel GetLevelModel(int levelId, int storeId)
        {
            string strsql = @"select UL.* from Mall_UserLevel UL                             
                            where UL.UL_ID=@UL_ID and UL_CustomerID=@UL_CustomerID";
            SqlParameter[] parm = {
                   new SqlParameter("@UL_ID", levelId),
                   new SqlParameter("@UL_CustomerID", storeId)
                    };

            MallUserLevelModel model = null;
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strsql, parm))
            {
                model = DbHelperSQLP.GetEntity<MallUserLevelModel>(dr);
            }
            return model;
        }
        /// <summary>
        /// 获取当前最大等级级别
        /// 作者：郭孟稳
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public int GetMaxLevel(int storeId, int type)
        {
            string strSql = "select top 1 UL_Level from Mall_UserLevel where UL_CustomerID=@UL_CustomerID  and UL_Type=@UL_Type order by UL_Level desc";
            SqlParameter[] parm = {
                   new SqlParameter("@UL_CustomerID", storeId),
                   new SqlParameter("@UL_Type", type)
            };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm);

            return obj == null ? 0 : Convert.ToInt32(obj);
        }
        /// <summary>
        /// 获取商户的等级数量
        /// 作者：郭孟稳
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public int GetLevelCount(int storeId, int type)
        {
            string strSql = "select COUNT(*) from Mall_UserLevel where UL_CustomerID=@UL_CustomerID and UL_Type=@UL_Type";
            SqlParameter[] parm = {
                   new SqlParameter("@UL_CustomerID", storeId),
                   new SqlParameter("@UL_Type", type)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm));
        }




        /// <summary>
        /// 后台登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPassword"></param>
        /// <param name="IsShop">是否是门店登录</param>
        /// <returns></returns>
        public AdminLoginModel Login(string loginName, string loginPassword, bool IsShop)
        {
            string strSql = string.Empty;
            if (!IsShop)
                strSql = "select ID,LoginName,LoginPassword,0 as ShopBelongId,RoleId,UserName,UserMobile,UserStatus,UserEmail,LastLoginTime,CreateTime,0 as UserIndentity from BM_Manager where LoginName=@LoginName and LoginPassword=@LoginPassword";
            else
                strSql = "select ShopID as ID,LoginName,LoginPassword,ShopBelongId,0 as ReloId,ShopName as UserName,ContactWay as UserMobile,IsActive as UserStatus,'' as UserEmail ,CreateTime,ShopType as UserIndentity from BM_ShopManage where LoginName =@LoginName and LoginPassword =@LoginPassword";
            SqlParameter[] parm = {
                   new SqlParameter("@LoginName", loginName),
                   new SqlParameter("@LoginPassword", loginPassword)
            };
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parm))
            {
                return DbHelperSQLP.GetEntity<AdminLoginModel>(dr);
            }
        }




        /// <summary>
        /// 前端登录
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="loginPassword">The login password.</param>
        /// <returns>UserModel.</returns>
        public UserModel Login(string loginName, string loginPassword)
        {
            string strSql = APP_USER_SELECT + " and U.UB_UserLoginName=@LoginName and U.UB_UserLoginPassword=@LoginPassword";
            SqlParameter[] parm = {
                   new SqlParameter("@LoginName", loginName),
                   new SqlParameter("@LoginPassword", loginPassword)
            };
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parm))
            {
                return DbHelperSQLP.GetEntity<UserModel>(dr);
            }
        }

        /// <summary>
        /// 修改最后登录时间
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateLastLoginTime(int userId)
        {
            string strSql = "update BM_User_extend set LastLoginTime=getdate() where UserId=@UserId";
            SqlParameter[] parm = {
                   new SqlParameter("@UserId", userId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parm) > 0;
        }
        /// <summary>
        /// 添加用户授权token
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Token">The token.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool AddUserAuthToken(int UserId, string Token)
        {
            string strSql = "insert into BM_AuthToken(UserId,Token) values(@UserId,@Token)";
            SqlParameter[] parm = {
                   new SqlParameter("@UserId", UserId),
                   new SqlParameter("@Token", Token)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parm) > 0;
        }

        /// <summary>
        /// 更新用户授权token
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Token">The token.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool UpdateUserAuthToken(int UserId, string Token)
        {
            string strSql = "update BM_AuthToken set Token=@Token,UpdateTime=getdate() where UserId=@UserId";
            SqlParameter[] parm = {
                   new SqlParameter("@UserId", UserId),
                   new SqlParameter("@Token", Token)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parm) > 0;
        }

        /// <summary>
        /// 判断授权token是否存在
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>true if [is authentication token exist] [the specified user identifier]; otherwise, false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsAuthTokenExist(int UserId)
        {
            string strSql = "select COUNT(1) from BM_AuthToken where UserId=@UserId";
            SqlParameter[] parm = {
                   new SqlParameter("@UserId", UserId),
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm)) > 0;
        }

        /// <summary>
        /// 根据用户ID，获取用户token
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string GetAuthTokenByUserId(int UserId)
        {
            string strSql = "select Token from BM_AuthToken where UserId=@UserId";
            SqlParameter[] parm = {
                   new SqlParameter("@UserId", UserId),
            };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm);
            if (obj != null)
                return obj.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// 根据用户token，获取用户ID
        /// </summary>
        /// <param name="Token">The token.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetUserIdByAuthToken(string Token)
        {
            string strSql = "select UserId from BM_AuthToken where Token=@Token";
            SqlParameter[] parm = {
                   new SqlParameter("@Token", Token),
            };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm);
            if (obj != null)
                return Convert.ToInt32(obj);
            else
                return 0;
        }


        /// <summary>
        /// 判断用户账户和所属门店是否激活
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if [is user active] [the specified user identifier]; otherwise, false.</returns>
        public bool IsUserActive(int userId)
        {
            string strSql = @"select COUNT(1) from BM_User_extend u
                                LEFT join BM_ShopManage s on s.ShopID=u.ShopId
                                where UserId=@UserId and u.IsActive=1 and s.IsActive=1";
            SqlParameter[] parm = {
                new SqlParameter("@UserId", userId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parm)) > 0;
        }


        /// <summary>
        /// 添加盟友奖励设置
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddRewardSetting(RewardsSettingModel model)
        {
            string strSql = "insert into BM_RewardsSetting(UserId,CustomerReward,OrderReward,ShopReward,ExtraReward,ShopId) values(@UserId,@CustomerReward,@OrderReward,@ShopReward,@ExtraReward,@ShopId)";
            SqlParameter[] param = {
                new SqlParameter("@UserId", model.UserId),
                new SqlParameter("@CustomerReward", model.CustomerReward),
                new SqlParameter("@OrderReward", model.OrderReward),
                new SqlParameter("@ShopReward", model.ShopReward),
                new SqlParameter("@ExtraReward", model.ExtraReward),
                new SqlParameter("@ShopId", model.ShopId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param);
        }

        /// <summary>
        /// 更新盟友奖励设置
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateRewardSetting(RewardsSettingModel model)
        {
            string strSql = "update  BM_RewardsSetting set CustomerReward=@CustomerReward,OrderReward=@OrderReward,ShopReward=@ShopReward,ExtraReward=@ExtraReward,UpdateTime=@UpdateTime where ShopId=@ShopId";
            SqlParameter[] param = {
                new SqlParameter("@ShopId", model.ShopId),
                new SqlParameter("@CustomerReward", model.CustomerReward),
                new SqlParameter("@OrderReward", model.OrderReward),
                new SqlParameter("@ShopReward", model.ShopReward),
                new SqlParameter("@ExtraReward", model.ExtraReward),
                new SqlParameter("@UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }

        /// <summary>
        ///获取盟友奖励信息
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>RewardsSettingModel.</returns>
        public RewardsSettingModel GetRewardModel(int userId)
        {
            string strSql = "select top 1 UserId,CustomerReward,OrderReward,ShopReward,ExtraReward,UpdateTime,CreateTime from BM_RewardsSetting where ShopId=@ShopId";
            SqlParameter[] param = {
                new SqlParameter("@ShopId", userId)
            };

            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, param))
            {
                return DbHelperSQLP.GetEntity<RewardsSettingModel>(dr);
            }
        }

        /// <summary>
        /// 判断当前盟主的盟友奖励是否存在
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if [is rewar exist] [the specified user identifier]; otherwise, false.</returns>
        public bool IsRewarExist(int userId)
        {
            string strSql = "select COUNT(1) from BM_RewardsSetting where ShopId=@ShopId";
            SqlParameter[] param = {
                new SqlParameter("@ShopId", userId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param)) > 0;
        }


        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="mobile">The mobile.</param>
        /// <param name="password">The password.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool ForgetPwd(string mobile, string password)
        {
            string strSql = "update Hot_UserBaseInfo set UB_UserLoginPassword=@UB_UserLoginPassword where UB_CustomerID=@UB_CustomerID and UB_UserMobile=@UB_UserMobile ";
            SqlParameter[] param = {
                new SqlParameter("@UB_UserLoginPassword",password),
                new SqlParameter("@UB_CustomerID", ConstConfig.storeId),
                new SqlParameter("@UB_UserMobile", mobile)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">The user identifier.</param>        
        /// <param name="oldPassword">The old password.</param>
        /// <param name="password">The password.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool ChanagePassword(int userId, string oldPassword, string password)
        {
            string strSql = "update Hot_UserBaseInfo set UB_UserLoginPassword=@UB_UserLoginPassword where UB_CustomerID=@UB_CustomerID and UB_UserID=@UB_UserID and UB_UserLoginPassword=@OldPassword ";
            SqlParameter[] param = {
                new SqlParameter("@UB_UserLoginPassword",password),
                new SqlParameter("@UB_CustomerID", ConstConfig.storeId),
                new SqlParameter("@UB_UserID", userId),
                new SqlParameter("@OldPassword",oldPassword)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }

        /// <summary>
        /// 注册申请保存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        /// <param name="userName"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public int SaveApplyFriend(int userId, string mobile, string password
            , string nickname, string userName
            , int sex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_ApplyFriend(");
            strSql.Append("UserId,UserName,Sex,Mobile,Status,CreateTime,NickNname,Password)");
            strSql.Append(" values (");
            strSql.Append("@UserId,@UserName,@Sex,@Mobile,@Status,@CreateTime,@NickNname,@Password)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@UserName",userName),
                    new SqlParameter("@Sex", sex),
                    new SqlParameter("@Mobile", mobile),
                    new SqlParameter("@Status", "0"),
                    new SqlParameter("@CreateTime", DateTime.Now),
                    new SqlParameter("@NickNname", nickname),
                    new SqlParameter("@Password", password)
            };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 存在申请
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool ExistApplyFriend(string mobile)
        {
            string strSql = "select COUNT(1) from BM_ApplyFriend with(nolock) where Mobile=@Mobile and Status=0";
            var param = new[] {
                new SqlParameter("@Mobile",mobile)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param)) > 0;
        }




        /// <summary>
        /// 添加用户余额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public int addUserMoney(int userId, decimal money)
        {
            string strSql = "update BM_User_extend set MengBeans=MengBeans+@money where UserId=@UserId";
            var parms = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@money",money)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }


        /// <summary>
        /// 增加用户锁定盟豆
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public int addMengBeansLocked(int userId, decimal money)
        {
            string strSql = "update BM_User_extend set MengBeansLocked=MengBeansLocked+@money where UserId=@UserId";
            var parms = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@money",money)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }


        /// <summary>
        /// 获得用户等级列表
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<MallUserLevelModel> GeUserLevelList(int storeId, int type)
        {
            List<MallUserLevelModel> list = new List<MallUserLevelModel>();
            string strSql = @"select * from Mall_UserLevel where UL_CustomerID=@UL_CustomerID and UL_Type=@UL_Type order by UL_MemberNum desc";
            var param = new[] {
                    new SqlParameter("@UL_CustomerID", storeId),
                    new SqlParameter("@UL_Type", type)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, param))
            {
                list = DbHelperSQLP.GetEntityList<MallUserLevelModel>(dr);
            }
            return list;
        }


        /// <summary>
        /// 更新用户等级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public int updateUserLevel(int userId, int levelId)
        {
            string strSql = "update Hot_UserBaseInfo set UB_LevelID=@UB_LevelID where UB_UserID=@UB_UserID";
            var parms = new[] {
                new SqlParameter("@UB_UserID",userId),
                new SqlParameter("@UB_LevelID",levelId)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }


        /// <summary>
        /// 获得用户的下线数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int countByBelongOne(int userId)
        {
            string strSql = "select count(*) from Hot_UserBaseInfo where UB_BelongOne=@UserId";
            var param = new[] {
                new SqlParameter("@UserId",userId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param));

        }

        /// <summary>
        /// 添加盟豆兑换
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userMasterId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int insertBeansConvert(int userId, int userMasterId, decimal amount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_BeansConvert(");
            strSql.Append("UserId,UserMasterId,Amount,Status,UpdateTime,CreateTime)");
            strSql.Append(" values (");
            strSql.Append("@UserId,@UserMasterId,@Amount,@Status,@UpdateTime,@CreateTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@UserId",userId),
                    new SqlParameter("@UserMasterId", userMasterId),
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@Status", "0"),
                    new SqlParameter("@UpdateTime",DateTime.Now),
                    new SqlParameter("@CreateTime", DateTime.Now)};


            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);

        }


        /// <summary>
        /// 获取兑换列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        public List<BeansConvertModel> getBeansConvertListModel(int userId, int lastId)
        {
            string strSql = "select top 10 c.*,UB_UserRealName as UserRealName,UB_WxHeadImg as HeadImg from BM_BeansConvert as c "
                + " left join Hot_UserBaseInfo as u on u.UB_UserID=c.UserId  where c.UserId=@UserId";
            if (lastId > 0) strSql += " and id<" + lastId;
            strSql += " order by id desc";
            var parms = new[] {
                   new SqlParameter("@UserId",userId)
            };

            List<BeansConvertModel> list = new List<BeansConvertModel>();
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parms))
            {
                list = DbHelperSQLP.GetEntityList<BeansConvertModel>(dr);
            }
            return list;
        }

        /// <summary>
        /// 获取兑换列表
        /// </summary>
        /// <param name="userMasterId">The user master identifier.</param>
        /// <param name="lastId">The last identifier.</param>
        /// <param name="type">0未处理，1亿处理</param>
        /// <returns>List&lt;BeansConvertModel&gt;.</returns>
        public List<BeansConvertModel> getBeansConvertListByMasterModel(int userMasterId, int lastId, int type)
        {
            string strSql = "select top 10 c.*,UB_UserRealName as UserRealName,UB_WxHeadImg as HeadImg from BM_BeansConvert as c "
                + " left join Hot_UserBaseInfo as u on u.UB_UserID=c.UserId where UserMasterId=@UserMasterId";
            if (lastId > 0) strSql += " and id< @lastId ";

            if (type == 1) strSql += " and c.Status<>0 ";
            else strSql += " and c.Status=@Status ";

            strSql += " order by c.id desc";
            var parms = new[] {
                   new SqlParameter("@UserMasterId",userMasterId),
                   new SqlParameter("@lastId",lastId),
                   new SqlParameter("@Status",type)
            };

            List<BeansConvertModel> list = new List<BeansConvertModel>();
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parms))
            {
                list = DbHelperSQLP.GetEntityList<BeansConvertModel>(dr);
            }
            return list;
        }

        public int updateBeansConvertStatus(int id, int status)
        {
            string strSql = "update BM_BeansConvert set Status=@Status where Id=@Id";
            var parms = new[] {
                  new SqlParameter("@Status",status),
                  new SqlParameter("@Id",id)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }


        /// <summary>
        /// 获取已兑换盟豆
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.Int32.</returns>
        public int GetAllConvertTotal(int userId)
        {
            string strSql = "select ISNULL(SUM(Amount),0) from BM_BeansConvert B where B.Status=1 and UserId=@UserId";
            var parms = new[] {
                  new SqlParameter("@UserId",userId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parms));
        }


        public BeansConvertModel getBeansConvertModel(int id)
        {
            string strSql = "select * from BM_BeansConvert where id=@id";
            var parms = new[] {
                   new SqlParameter("@id",id)
            };

            BeansConvertModel model = null;
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parms))
            {
                model = DbHelperSQLP.GetEntity<BeansConvertModel>(dr);
            }
            return model;
        }

        public ApplyFriendModel getApplyFriendModel(int id)
        {
            string strSql = "select ID,UserId,UserName,Sex,Mobile,Status,CreateTime,NickNname as NickName,Password from BM_ApplyFriend where id=@id";
            var parms = new[] {
                   new SqlParameter("@id",id)
            };
            ApplyFriendModel model = null;
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, parms))
            {
                model = DbHelperSQLP.GetEntity<ApplyFriendModel>(dr);
            }
            return model;
        }

        public int updateApplyFriendStatus(int id, int status)
        {
            string strSql = "update BM_ApplyFriend set status=@status where id=@id";
            var parms = new[] {
                   new SqlParameter("@id",id),
                    new SqlParameter("@status",status)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }


        public int getUserShopId(int userId)
        {
            string strSql = "select ShopId from BM_User_extend  where UserId=@UserId";
            var parms = new[] {
                   new SqlParameter("@UserId",userId)
            };
            return int.Parse(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, parms).ToString());
        }

        /// <summary>
        /// 获取兑换数量(只对盟主)
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="status">状态 0,未审核 1已审核 2,拒绝</param>
        /// <returns>System.Int32.</returns>
        public int GetConvertCount(int userid, int status)
        {
            string strSql = "select COUNT(1) from BM_BeansConvert where UserMasterId=@UserId and Status=@Status";
            var param = new[] {
                new SqlParameter("@UserId",userid),
                new SqlParameter("@Status",status)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param));
        }


        /// <summary>
        /// 获得盟豆流水记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        public List<BeansRecordsModel> getBeansRecordsList(int userId, int lastId, int LogType)
        {
            List<BeansRecordsModel> list = new List<BeansRecordsModel>();
            string strSql = "select * from BM_BeansRecords where UserId=@UserId and LogType=@LogType";
            if (lastId > 0) strSql += " and id<" + lastId;
            strSql += " order by id desc";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                 new SqlParameter("@LogType",LogType)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, param))
            {
                list = DbHelperSQLP.GetEntityList<BeansRecordsModel>(dr);
            }
            return list;
        }

        public decimal countBeansMoney(int userId, int LogType, int income)
        {
            string strSql = "select sum(Amount) from BM_BeansRecords where UserId=@UserId and LogType=@LogType and Income=@Income";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@LogType",LogType),
                new SqlParameter("@Income",income)
            };

            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param).ToString();
            if (obj != null && obj.ToString() != "")
                return Convert.ToDecimal(obj);
            else
                return 0;
        }



        public decimal countTempBeansMoney(int userId, int LogType, int income)
        {
            string strSql = "select sum(Amount) from BM_TempBeansRecords where UserId=@UserId and LogType=@LogType and Income=@Income";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@LogType",LogType),
                new SqlParameter("@Income",income)
            };

            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param).ToString();
            if (obj != null && obj.ToString() != "")
                return Convert.ToDecimal(obj);
            else
                return 0;
        }


        /// <summary>
        /// 获取待结算盟豆
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="LogType">Type of the log.</param>
        /// <returns>System.Decimal.</returns>
        public decimal countTempBeansMoney(int userId, int LogType)
        {
            string strSql = "select sum(Amount) from BM_TempBeansRecords where UserId=@UserId and LogType=@LogType";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@LogType",LogType)
            };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param).ToString();
            if (obj != null && obj.ToString() != "")
                return Convert.ToDecimal(obj);
            else
                return 0;
        }



        /// <summary>
        /// 获得盟豆流水记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        public List<TempBeansRecordsModel> getTempBeansRecordsList(int userId, int lastId, int LogType)
        {
            List<TempBeansRecordsModel> list = new List<TempBeansRecordsModel>();
            string strSql = "select * from BM_TempBeansRecords where UserId=@UserId and LogType=@LogType";
            if (lastId > 0) strSql += " and id<" + lastId;
            strSql += " order by id desc";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@LogType",LogType)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql, param))
            {
                list = DbHelperSQLP.GetEntityList<TempBeansRecordsModel>(dr);
            }
            return list;
        }

        public int AddTempBeansRecords(TempBeansRecordsModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_TempBeansRecords(");
            strSql.Append("LogType,UserId,Income,Amount,Remark,CreateTime,Status,OrderId)");
            strSql.Append(" values (");
            strSql.Append("@LogType,@UserId,@Income,@Amount,@Remark,@CreateTime,@Status,@OrderId)");
            strSql.Append(";select @@IDENTITY");
            var parameters = new[] {
                    new SqlParameter("@LogType", SqlDbType.Int),
                    new SqlParameter("@UserId", SqlDbType.Int),
                    new SqlParameter("@Income", SqlDbType.Int),
                    new SqlParameter("@Amount", SqlDbType.Decimal),
                    new SqlParameter("@Remark", SqlDbType.NVarChar),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime),
                    new SqlParameter("@Status", SqlDbType.Int),
                    new SqlParameter("@OrderId", model.OrderId)
            };
            parameters[0].Value = model.LogType;
            parameters[1].Value = model.UserId;
            parameters[2].Value = model.Income;
            parameters[3].Value = model.Amount;
            parameters[4].Value = model.Remark;
            parameters[5].Value = model.CreateTime;
            parameters[6].Value = model.Status;

            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
        }

        public int AddBeansRecords(BeansRecordsModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_BeansRecords(");
            strSql.Append("LogType,UserId,Income,Amount,Remark,CreateTime,OrderId)");
            strSql.Append(" values (");
            strSql.Append("@LogType,@UserId,@Income,@Amount,@Remark,@CreateTime,@OrderId)");
            strSql.Append(";select @@IDENTITY");
            var parameters = new[] {
                    new SqlParameter("@LogType", model.LogType),
                    new SqlParameter("@UserId",model.UserId),
                    new SqlParameter("@Income", model.Income),
                    new SqlParameter("@Amount", model.Amount),
                    new SqlParameter("@Remark", model.Remark),
                    new SqlParameter("@CreateTime", model.CreateTime),
                    new SqlParameter("@OrderId", model.OrderId)
            };

            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
        }





        /// <summary>
        /// 添加签到日志
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddUserSignLog(UserSignLogModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_UserSignLog(");
            strSql.Append("UserId,BelongOneUserId,ShopId,BelongOneShopId,Amount,Reward,Remark,UserAddress,AppSystem)");
            strSql.Append(" values (");
            strSql.Append("@UserId,@BelongOneUserId,@ShopId,@BelongOneShopId,@Amount,@Reward,@Remark,@UserAddress,@AppSystem)");
            var parameters = new[] {
                    new SqlParameter("@BelongOneUserId", model.BelongOneUserId),
                    new SqlParameter("@UserId",model.UserId),
                    new SqlParameter("@ShopId", model.ShopId),
                    new SqlParameter("@BelongOneShopId", model.BelongOneShopId),
                    new SqlParameter("@Amount", model.Amount),
                    new SqlParameter("@Reward", model.Reward),
                    new SqlParameter("@Remark", model.Remark),
                    new SqlParameter("@UserAddress", model.UserAddress),
                    new SqlParameter("@AppSystem", model.AppSystem)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
        }




        /// <param name="userId">The user identifier.</param>
        /// <returns>MemberSignModel.</returns>
        public MemberSignModel GetMemberSignModel(int userId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,UserId,lastSignTime,SignCount,CreateTime,TotalSignIntegral,TotalSignDays from BM_UserSign ");
            strSql.Append(" where UserId=@UserId");
            SqlParameter[] parameters = {
                    new SqlParameter("@UserId",userId)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters))
            {
                return DbHelperSQLP.GetEntity<MemberSignModel>(dr);
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddMemberSignInfo(MemberSignModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into BM_UserSign(");
            strSql.Append("UserId,lastSignTime,SignCount,TotalSignIntegral,TotalSignDays)");
            strSql.Append(" values (");
            strSql.Append("@UserId,@lastSignTime,@SignCount,@TotalSignIntegral,@TotalSignDays)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@lastSignTime",model.lastSignTime),
                    new SqlParameter("@SignCount", model.SignCount),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@TotalSignIntegral", model.TotalSignIntegral),
                    new SqlParameter("@TotalSignDays",model.TotalSignDays)
                                        };
            object obj = DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 更新会员签到信息
        /// </summary>
        public bool UpdateMemberSignInfo(MemberSignModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update BM_UserSign set ");
            strSql.Append("lastSignTime=@lastSignTime,");
            strSql.Append("SignCount=@SignCount, ");
            strSql.Append("TotalSignIntegral=@TotalSignIntegral, ");
            strSql.Append("TotalSignDays=@TotalSignDays ");
            strSql.Append(" where ID=@Id and UserId=@UserId");
            SqlParameter[] parameters = {
                    new SqlParameter("@lastSignTime",model.lastSignTime),
                    new SqlParameter("@SignCount", model.SignCount),
                    new SqlParameter("@Id", model.ID),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@TotalSignIntegral", model.TotalSignIntegral),
                    new SqlParameter("@TotalSignDays",model.TotalSignDays)
                                        };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), parameters) > 0;
        }


        /// <summary>
        /// 添加用户积分
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="money">The money.</param>
        /// <returns>System.Int32.</returns>
        public int addUserIntegral(int userId, decimal money)
        {
            string strSql = "update BM_User_extend set Score=Score+@money where UserId=@UserId";
            var parms = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@money",money)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }

        /// <summary>
        /// 添加用户锁定积分
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="money">The money.</param>
        /// <returns>System.Int32.</returns>
        public int addUserLockedIntegral(int userId, decimal money)
        {
            string strSql = "update BM_User_extend set ScoreLocked=ScoreLocked+@money where UserId=@UserId";
            var parms = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@money",money)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, parms);
        }

        /// <summary>
        /// 获取盟友申请列表
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        public ResultPageModel GetApplyFriendList(SearchModel model)
        {
            ResultPageModel result = new ResultPageModel();
            string strSql = "select ID,UserId,UserName,Sex,Mobile,Status,CreateTime,NickNname as NickName,Password from BM_ApplyFriend where 1=1 and UserId=@UserId and Status=0";

            var param = new[] {
                new SqlParameter("@UserId",model.UserId),
            };

            //生成sql语句
            return getPageData<ApplyFriendModel>(model.PageSize, model.PageIndex, strSql, "CreateTime", param, (items) =>
            {
                items.ForEach((item) =>
                {
                    if (item.Status == 0)
                        item.StatusName = "未审核";
                    else
                        item.StatusName = "已拒绝";

                    if (item.Sex == 0)
                        item.UserGender = "F";
                    if (item.Sex == 1)
                        item.UserGender = "M";
                    else
                        item.UserGender = "未知";

                });
            });
        }



        /// <summary>
        ///添加用户客户提交量
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool AddUserCustomerAmount(int userId)
        {
            string strSql = "update BM_User_extend set CustomerAmount=CustomerAmount+1 where UserId=@UserId";
            var param = new[] {
                new SqlParameter("@UserId",userId),
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }

        /// <summary>
        /// 添加用户订单成交量
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool AddUserOrderSuccessAmount(int userId)
        {
            string strSql = "update BM_User_extend set OrderSuccessAmount=OrderSuccessAmount+1 where UserId=@UserId";
            var param = new[] {
                new SqlParameter("@UserId",userId),
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }

        /// <summary>
        /// 获取盟友数量
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>System.Int32.</returns>
        public int GetAllyCount(int userid)
        {
            string strSql = @"select COUNT(1) from BM_User_extend UE
                                inner join Hot_UserBaseInfo U with(nolock) on U.UB_UserID=UE.UserId
                                where UserIdentity=0  and U.UB_BelongOne=@UserId";
            var param = new[] {
                new SqlParameter("@UserId",userid)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql, param));
        }

        /// <summary>
        /// 获取排名
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="belongOne">The belong one.</param>
        /// <returns>MyAllyIndexModel.</returns>
        public MyAllyIndexModel GetUserRank(int userId, int belongOne)
        {
            string strSql = @"select CustomerRank,OrderRank from (
                                SELECT UserId, RANK() over(order by CustomerAmount desc) CustomerRank, RANK() over(order by OrderSuccessAmount desc) OrderRank 
                                FROM BM_User_extend D WITH(NOLOCK)
                                inner join Hot_UserBaseInfo U with(nolock) on U.UB_UserID=D.UserId and U.UB_BelongOne=@BelongUserId
                                where D.UserIdentity=0
                                ) as temp where temp.UserId = @UserId";
            var param = new[] {
                new SqlParameter("@UserId",userId),
                new SqlParameter("@BelongUserId",belongOne)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), param))
            {
                return DbHelperSQLP.GetEntity<MyAllyIndexModel>(dr);
            }
        }



        /// <summary>
        /// 获取盟友申请得审核的数量
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.Int32.</returns>
        public int AllyApplyCount(int userId)
        {
            string strSql = "select COUNT(1) from BM_ApplyFriend where UserId=@UserId and Status=0";
            var param = new[] {
                new SqlParameter("@UserId",userId)
            };
            return Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), param));
        }



        /// <summary>
        /// 获取用户工作汇报列表
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ResultPageModel.</returns>
        public ResultPageModel GetAppUserReportList(int UserId, int pageIndex, int pageSize)
        {
            string strSql = "select ID,ReportTitle,CreateTime from BM_UserReport where UserId=@UserId ";
            string orderbyField = "CreateTime";
            var param = new[] {
                new SqlParameter("@UserID",UserId),
            };
            //生成sql语句
            return getPageData<AppUserReportListModel>(pageSize, pageIndex, strSql, orderbyField, param, (items =>
            {
                items.ForEach(item =>
                {
                    item.time = StringHelper.GetConvertFriendlyTime(item.CreateTime.ToString(), 7);
                    item.reportUrl = WebConfig.articleDetailsDomain() + "/app/reportdetail.html?workid=" + item.ID;
                });
            }));
        }


        /// <summary>
        /// 获取工作汇报实体
        /// </summary>
        /// <param name="workid">The workid.</param>
        /// <returns>UserReportModel.</returns>
        public UserReportModel GetUserReportModel(int workid)
        {
            string strSql = "select ID,UserId,ShopId,ReportTitle,Addr,JsonContent,CreateTime from BM_UserReport where ID=@ID";
            var param = new[] {
                new SqlParameter("@ID",workid)
            };
            using (IDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, strSql.ToString(), param))
            {
                var data = DbHelperSQLP.GetEntity<UserReportModel>(dr);
                if (data != null)
                {
                    data.time = StringHelper.GetConvertFriendlyTime(data.CreateTime.ToString(), 7);
                }
                return data;
            }
        }


        /// <summary>
        /// 删除工作汇报
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool DeleteUserReport(int ID)
        {
            string strSql = "delete from BM_UserReport where ID=@ID";
            var param = new[] {
                new SqlParameter("@ID",ID)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param) > 0;
        }


        /// <summary>
        /// 获取工作汇报
        /// </summary>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns>ResultPageModel.</returns>
        public ResultPageModel GetUserReportList(int shopId, SearchModel model)
        {
            string strSql = @"select u.UB_UserRealName as UserName,u.UB_UserMobile as UserMobile,r.ID,r.UserId,r.ShopId,r.ReportTitle,r.Addr,r.JsonContent,r.CreateTime from BM_UserReport r
                              left join Hot_UserBaseInfo u with(nolock) on u.UB_UserID=r.UserId
                                where r.ShopId=@ShopId
                            ";


            if (!string.IsNullOrEmpty(model.key))
            {
                strSql += string.Format(" and (u.UB_UserRealName like '{0}' or r.ReportTitle like '%{0}%' or u.UB_UserMobile='{0}')", model.key);
            }

            if (!string.IsNullOrEmpty(model.startTime))
                strSql += " and CONVERT(nvarchar(10),r.CreateTime,121)>=CONVERT(nvarchar(10),@startTime,121) ";
            if (!string.IsNullOrEmpty(model.endTime))
                strSql += " and CONVERT(nvarchar(10),r.CreateTime,121)<=CONVERT(nvarchar(10),@endTime,121) ";


            string orderbyField = "r.CreateTime";
            var param = new[] {
                new SqlParameter("@ShopId",shopId),
                new SqlParameter("@startTime",model.startTime),
                new SqlParameter("@endTime",model.endTime)
            };
            //生成sql语句
            return getPageData<UserReportModel>(model.PageSize, model.PageIndex, strSql, orderbyField, param, (items =>
            {
                items.ForEach(item =>
                {
                    item.time = StringHelper.GetConvertFriendlyTime(item.CreateTime.ToString(), 7);
                });
            }));
        }


        /// <summary>
        /// 添加用户工作汇报
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddAppUserReport(UserReportModel model)
        {
            string strSql = "insert into BM_UserReport(UserId,ShopId,ReportTitle,Addr,JsonContent,CreateTime) values(@UserId,@ShopId,@ReportTitle,@Addr,@JsonContent,@CreateTime)";
            var param = new[] {
                new SqlParameter("@UserId",model.UserId),
                new SqlParameter("@ShopId",model.ShopId),
                new SqlParameter("@ReportTitle",model.ReportTitle),
                new SqlParameter("@Addr",model.Addr),
                new SqlParameter("@JsonContent",model.JsonContent),
                new SqlParameter("@CreateTime",DateTime.Now)
            };
            return DbHelperSQLP.ExecuteNonQuery(WebConfig.getConnectionString(), CommandType.Text, strSql, param);

        }



    }
}
